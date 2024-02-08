using CanDiagSupport;
using DevConfig.Utils;
using DevConfigSupp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;
using WeifenLuo.WinFormsUI.Docking;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using Message = CanDiagSupport.Message;

/*
--- List File ---
query ECmd_SD_Command, subcmd = 1, directory, 0x00
resp  ECmd_SD_Command, subcmd = 1, error(1), filecount(2)
resp  ECmd_SD_Command, subcmd = 2, error(1), idx(2), attr(1), size(4), time(4), filename(zbytek)
...

--- Get File ---
query ECmd_SD_Command, subcmd = 10, file_nr
resp  ECmd_SD_Command, subcmd = 11, error(1), file_pos(4), data(zbytek)
*/

namespace DevConfig
{
    public partial class SDCardCtrl : DockContent
    {
        const byte FX_DIR_ENTRY_DONE = 0x00;
        const byte FX_READ_ONLY = 0x01;
        const byte FX_HIDDEN = 0x02;
        const byte FX_SYSTEM = 0x04;
        const byte FX_VOLUME = 0x08;
        const byte FX_DIRECTORY = 0x10;
        const byte FX_ARCHIVE = 0x20;

        const byte ECmd_SD_Command = 0x65;

        const byte SD_SubCmd_ListFiles = 0x01;
        const byte SD_SubCmd_FileItemName = 0x02;
        const byte SD_SubCmd_GetFile = 0x10;
        const byte SD_SubCmd_GetFilePart = 0x11;

        byte address;
        MainForm MainForm;

        ///////////////////////////////////////////////////////////////////////////////////////////
        public SDCardCtrl(MainForm mainForm)
        {
            MainForm = mainForm;
            Debug.Assert(MainForm.InputPeriph != null);
            MainForm.InputPeriph.MessageReceived += InputPeriph_MessageReceived;
            InitializeComponent();
            lvwColumnSorter = new ListViewColumnSorter();
            listView1.ListViewItemSorter = lvwColumnSorter;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void SDCardCtrl_Load(object sender, EventArgs e)
        {
            Debug.Assert(MainForm.selectedDevice != null);
            address = MainForm.selectedDevice.Address;
            Debug.WriteLine($"CAN ID = {address}");
            Text += $" ({address:X2})";
            PopulateTreeView();
        }
        #region FORM COMMAND
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void btn_List_Click(object sender, EventArgs e)
        {
            PopulateTreeView();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void btn_Add_Click(object sender, EventArgs e)
        {

        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void btn_Get_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count > 0)
            {
                List<string> file_paths = new();

                foreach(ListViewItem item in listView1.SelectedItems)
                {
                    string path = MakePath((TreeNode)item.Tag);
                    path = Path.Combine(path, item.Text).Replace('\\', '/');
                    file_paths.Add(path);
                }

                CopyToLocal(file_paths);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void btn_Del_Click(object sender, EventArgs e)
        {

        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void treeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null && e.Node.Name != "SDCard")
            {
                e.Node.ImageKey = "FolderOpened.bmp";
                e.Node.SelectedImageKey = "FolderOpened.bmp";
                ExpandNode(e.Node, true);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void treeView1_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null && e.Node.Name != "SDCard")
            {
                e.Node.ImageKey = "FolderClosed.bmp";
                e.Node.SelectedImageKey = "FolderClosed.bmp";
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null)
            {
                listView1.Items.Clear();
                DirInfo dirInfo = ((DirInfo)e.Node.Tag);
                Debug.Assert(dirInfo.FileInfoList != null);
                foreach (FileInfo f in dirInfo.FileInfoList)
                {
                    if (!f.IsDirectory)
                    {
                        ListViewItem.ListViewSubItem[] subItems;
                        ListViewItem item = new ListViewItem(Path.GetFileName(f.Name), 1);
                        subItems = new ListViewItem.ListViewSubItem[]
                        {
                            new ListViewItem.ListViewSubItem(item, $"{f.ModifyTime}"),
                            new ListViewItem.ListViewSubItem(item, $"{f.Size}")
                        };
                        item.SubItems.AddRange(subItems);
                        var new_item = listView1.Items.Add(item);
                        new_item.Tag = e.Node;
                    }
                }
                listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
        }

        private ListViewColumnSorter lvwColumnSorter;

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.listView1.Sort();
        }

        
        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        ushort files_to_read = 0;
        uint file_len_req = 0;
        uint file_len_act = 0;
        List<Message> messages = new List<Message>();
        readonly ManualResetEvent sync_obj = new(false);
        Dictionary<uint, List<byte>> file_bytes_map = new();

        private void InputPeriph_MessageReceived(Message msg)
        {
            if (msg.SRC == address)
            {
                if (msg.CMD == ECmd_SD_Command)
                {
                    if (msg.Data.Count < 2)
                    {
                        Debug.WriteLine($"Bad msg length {msg.Data.Count} bytes");
                        return;
                    }
                    switch (msg.Data[0])
                    {
                        case SD_SubCmd_ListFiles:
                            byte error = msg.Data[1];
                            if (error == 0)
                            {
                                files_to_read = BitConverter.ToUInt16(msg.Data.Skip(2).Take(2).Reverse().ToArray());
                                Debug.WriteLine($"Get {files_to_read} files");
                            }
                            else
                            {
                                Debug.WriteLine($"Error {error}");
                            }
                            break;
                        case SD_SubCmd_FileItemName: // FileInfo file
                            lock (messages)
                                messages.Add(msg);
                            sync_obj.Set();
                            break;
                        case SD_SubCmd_GetFile:
                            if(msg.Data[1] != 0)
                            {
                                Debug.WriteLine($"Error {msg.Data[1]}");
                                return;
                            }
                            file_bytes_map.Clear();
                            file_len_req = BitConverter.ToUInt32(msg.Data.Skip(2).Take(4).Reverse().ToArray());
                            file_len_act = 0;
                            Debug.WriteLine($"Get file with {file_len_req} bytes");
                            sync_obj.Set();
                            break;
                        case SD_SubCmd_GetFilePart:
                            if (msg.Data[1] != 0)
                            {
                                Debug.WriteLine($"Error {msg.Data[1]}");
                                return;
                            }
                            uint file_pos = BitConverter.ToUInt32(msg.Data.Skip(2).Take(4).Reverse().ToArray());
                            file_bytes_map[file_pos] = msg.Data.Skip(6).ToList();
                            file_len_act += (uint)(msg.Data.Count - 6);
                            sync_obj.Set();
                            break;
                    }
                }
            }
        }
        #region LIST FILES
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void PopulateTreeView()
        {
            treeView1.Nodes.Clear();

            if (!treeView1.Nodes.ContainsKey("SDCard"))
            {
                DirInfo file_info = new DirInfo("/");

                TreeNode node = new TreeNode("SD Card");
                node.Name = "SDCard";
                node.ImageKey = "SDCard.png";
                node.SelectedImageKey = "SDCard.png";
                node.Tag = file_info;

                treeView1.Nodes.Add(node);
            }

            int node_ix = treeView1.Nodes.IndexOfKey("SDCard");
            TreeNode rootNode = treeView1.Nodes[node_ix];

            ExpandNode(rootNode, true);
            treeView1.SelectedNode = rootNode;
            treeView1.Focus();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void ExpandNode(TreeNode node, bool expand)
        {
            DirInfo dirInfo = ((DirInfo)node.Tag);

            if (dirInfo.FileInfoList == null)
            {
                string path = MakePath(node);

                dirInfo.FileInfoList = GetFileList(path);

                foreach (FileInfo file in dirInfo.FileInfoList)
                {
                    if (file.IsDirectory)
                    {
                        DirInfo dirInfo1 = new DirInfo(file.Name);
                        TreeNode new_node = new TreeNode(Path.GetFileName(file.Name));
                        new_node.ImageKey = "FolderClosed.bmp";
                        new_node.SelectedImageKey = "FolderClosed.bmp";
                        new_node.Tag = dirInfo1;
                        node.Nodes.Add(new_node);
                        if (expand)
                            ExpandNode(new_node, false);
                    }
                }
            }
            else if (expand)
            {
                foreach (TreeNode n in node.Nodes)
                    ExpandNode(n, false);
            }

            if (expand)
                node.Expand();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private string MakePath(TreeNode node)
        {
            List<string> list = new List<string>();
            do
            {
                DirInfo dirInfo = ((DirInfo)node.Tag);
                list.Add(dirInfo.Name);
                node = node.Parent;
            }
            while (node != null);

            list.Reverse();

            return Path.Combine(list.ToArray()).Replace('\\', '/');
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private List<FileInfo> GetFileList(string directory)
        {
            Cursor = Cursors.WaitCursor;
            Debug.WriteLine($"GetFileList({directory})");

            List<FileInfo> fileinfolist = new ();

            Message message = new Message();
            message.DEST = address;
            message.CMD = ECmd_SD_Command;

            message.Data = new List<byte>();
            message.Data.Add(SD_SubCmd_ListFiles);
            message.Data.AddRange(Encoding.ASCII.GetBytes(directory + "\0"));

            lock (messages)
                messages.Clear();
            sync_obj.Reset();

            MainForm.InputPeriph?.SendMsg(message);

            while (sync_obj.WaitOne(1000))
            {
                lock (messages)
                {
                    if (messages.Count == files_to_read)
                        break;
                }
                sync_obj.Reset();
            }

            foreach (Message msg in messages)
            {
                byte error = msg.Data[1];
                if (error == 0x00)
                {
                    ushort idx = BitConverter.ToUInt16(msg.Data.Skip(2).Take(2).Reverse().ToArray());
                    byte attr = msg.Data[4];
                    string filename = System.Text.Encoding.ASCII.GetString(msg.Data.Skip(13).ToArray());
                    if ((attr & FX_HIDDEN) != FX_HIDDEN)
                    {
                        FileInfo fileinfo = new FileInfo(filename);

                        if ((attr & FX_DIRECTORY) == FX_DIRECTORY)
                        {
                            Debug.WriteLine($"Dir {idx}, attr = {attr:X2}, {filename}");
                            if (filename != "." && filename != "..")
                            {
                                fileinfo.IsDirectory = true;
                                fileinfolist.Add(fileinfo);
                            }
                        }
                        else
                        {
                            fileinfo.IsDirectory = false;
                            fileinfo.Size = BitConverter.ToUInt32(msg.Data.Skip(5).Take(4).Reverse().ToArray());
                            uint time = BitConverter.ToUInt32(msg.Data.Skip(9).Take(4).Reverse().ToArray());

                            DateTimeOffset dateTime = DateTimeOffset.FromUnixTimeSeconds(time);
                            fileinfo.ModifyTime = dateTime.DateTime;//.ToLocalTime();

                            Debug.WriteLine($"File {idx}, attr = {attr:X2}, size = {fileinfo.Size}, time = {time}, {filename}");
                            fileinfolist.Add(fileinfo);
                        }

                    }
                }
                else
                {
                    Debug.WriteLine($"Error {error}");
                }
            }

            Cursor = Cursors.Default;
            return fileinfolist;

        }
        #endregion
        #region DRAG/DROP
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            // TODO DROP

        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // TODO DRAG

            List<string> file_paths = new();

            foreach (ListViewItem item in listView1.SelectedItems)
            {
                string path = MakePath((TreeNode)item.Tag);
                path = Path.Combine(path, item.Text).Replace('\\', '/');
                file_paths.Add(path);
            }

            var dob = new DataObject();
            dob.SetData(DataFormats.FileDrop, file_paths);
            DoDragDrop(dob, DragDropEffects.Copy);
            
        }

        #endregion

        #region GET FILE
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void CopyToLocal(List<string> file_paths, string? dest_path = null)
        {
            // Dotaz na adresar.
            if (dest_path == null)
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                if (dialog.ShowDialog() != DialogResult.OK)
                    return;
                dest_path = dialog.SelectedPath;
            }

            // Kontrola jmen.
            List<string> exist_files = new List<string>();
            foreach (string file_path in file_paths)
            {
                string local_file_path = Path.Combine(dest_path, Path.GetFileName(file_path));
                if (File.Exists(local_file_path))
                    exist_files.Add(local_file_path);
            }

            if (exist_files.Count > 0)
            {
                if (MessageBox.Show("One or more files exist in the destination directory.\nDo you want to replace the files?", "DevConfig - File exists", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                exist_files.ForEach(file_path => { File.Delete(file_path); });
            }

            // Kopirujeme
            Cursor = Cursors.WaitCursor;
            foreach (string file_path in file_paths)
            {
                string local_file_path = Path.Combine(@"D:\Vymaz", Path.GetFileName(file_path));
                File.Delete(local_file_path);
                CopyToLocal(file_path, local_file_path);
                Debug.WriteLine($"{file_path}");
            }
            MainForm.progressBar.Value = 0;
            Cursor = Cursors.Default;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void CopyToLocal(string file_path, string local_file_path)
        {
            Message message = new Message();
            message.DEST = address;
            message.CMD = ECmd_SD_Command;

            message.Data = new List<byte>();
            message.Data.Add(SD_SubCmd_GetFile);
            message.Data.AddRange(Encoding.ASCII.GetBytes(file_path + "\0"));

            sync_obj.Reset();
            MainForm.InputPeriph?.SendMsg(message);

            MainForm.progressBar.Minimum = 0;
            MainForm.progressBar.Maximum = 1;
            MainForm.progressBar.Value = MainForm.progressBar.Minimum;

            while (true)
            {
                if(sync_obj.WaitOne(1000))
                {
                    sync_obj.Reset();
                    if (file_len_req == file_len_act)
                    {
                        using FileStream file = File.OpenWrite(local_file_path);

                        foreach (uint pos in file_bytes_map.Keys)
                        {
                            file.Position = pos;
                            file.Write(file_bytes_map[pos].ToArray());
                        }

                        file.Close();
                        file.Dispose();

                        file_bytes_map.Clear();
                        break; 
                    }
                    else
                    {
                        if(MainForm.progressBar.Maximum < file_len_req)
                            MainForm.progressBar.Maximum = (int)file_len_req;
                        if (file_len_act > MainForm.progressBar.Maximum)
                            MainForm.progressBar.Value = MainForm.progressBar.Maximum;
                        else
                            MainForm.progressBar.Value = (int)file_len_act;
                    }
                }
                else
                {
                    Debug.WriteLine("Error Timeout");
                    break; 
                }
            }
        }
        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
    }

        class DirInfo
    {
        public string Name;
        public List<FileInfo>? FileInfoList = null;

        public DirInfo(string name)
        {
            Name = name;
        }
    }

    class FileInfo
    {
        public string Name;
        public DateTime ModifyTime;
        public bool IsDirectory;
        public uint Size;

        public FileInfo(string name)
        {
            Name = name;
        }
    }
}

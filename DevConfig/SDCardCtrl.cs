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
using WeifenLuo.WinFormsUI.Docking;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using Message = CanDiagSupport.Message;

/*
query ECmd_SD_GetList, subcmd = 1, directory, 0x00
resp  ECmd_SD_GetList, subcmd = 1, error(1), filecount(2)
resp  ECmd_SD_GetList, subcmd = 2, error(1), idx(2), attr(1), size(4), time(4), filename(zbytek)
...



query ECmd_SD_GetList, subcmd = 2, file_nr
resp  ECmd_SD_GetList, subcmd = 2, error, time, filename
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

        const byte ECmd_SD_UploadFile = 0x60;
        const byte ECmd_SD_UploadFileStart = 0x61;
        const byte ECmd_SD_DownloadFile = 0x62;
        const byte ECmd_SD_DownloadFileStart = 0x63;
        const byte ECmd_SD_DownloadFileOtherData = 0x64;
        const byte ECmd_SD_GetList = 0x65;
        const byte ECmd_SD_DeleteFile = 0x66;
        const byte ECmd_StringCommand = 0xA0;

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
            this.listView1.ListViewItemSorter = lvwColumnSorter;
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
                        listView1.Items.Add(item);
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
        //}
        //ListViewColumnSorter sorter = GetListViewSorter(e.Column);
        //    listView1.ListViewItemSorter = sorter;
        //    listView1.Sort();
        }

        
        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        ushort files_to_read = 0;
        List<Message> messages = new List<Message>();
        readonly ManualResetEvent sync_obj = new(false);

        private void InputPeriph_MessageReceived(Message msg)
        {
            if (msg.SRC == address)
            {
                switch (msg.CMD)
                {
                    case ECmd_SD_GetList:
                        switch (msg.Data[0])
                        {
                            case 0x01: // FileInfo count
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
                            case 0x02: // FileInfo file
                                lock (messages)
                                    messages.Add(msg);
                                sync_obj.Set();
                                break;
                        }
                        break;
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

            return Path.Combine(list.ToArray());
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private List<FileInfo> GetFileList(string directory)
        {
            Debug.WriteLine($"GetFileList({directory})");

            List<FileInfo> fileinfolist = new List<FileInfo>();

            Message message = new Message();
            message.DEST = address;
            message.CMD = ECmd_SD_GetList;

            message.Data = new List<byte>();
            message.Data.Add(0x01);
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
            //DoDragDrop(e.Item, DragDropEffects.Copy);

            var files = new string[1];
            files[0] = "full path to temporary file";
            var dob = new DataObject();
            dob.SetData(DataFormats.FileDrop, files);
            DoDragDrop(dob, DragDropEffects.Copy);

        }

        #endregion

        #region GET FILE
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

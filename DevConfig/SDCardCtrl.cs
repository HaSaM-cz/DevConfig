﻿using CanDiagSupport;
using DevConfig.Controls.ListViewExCtrl;
using DevConfig.Service;
using System.Data;
using System.Diagnostics;
using System.Text;
using WeifenLuo.WinFormsUI.Docking;
using static System.Windows.Forms.ListView;
using Message = CanDiagSupport.Message;

namespace DevConfig
{
    public partial class SDCardCtrl : DockContent
    {
        #region CONSTANT
        const byte FX_DIR_ENTRY_DONE = 0x00;
        const byte FX_READ_ONLY = 0x01;
        const byte FX_HIDDEN = 0x02;
        const byte FX_SYSTEM = 0x04;
        const byte FX_VOLUME = 0x08;
        const byte FX_DIRECTORY = 0x10;
        const byte FX_ARCHIVE = 0x20;

        const byte SD_SubCmd_ListFiles = 0x01;
        const byte SD_SubCmd_ListFilesPart = 0x02;
        const byte SD_SubCmd_GetFile = 0x10;
        const byte SD_SubCmd_GetFilePart = 0x11;
        const byte SD_SubCmd_PutFile = 0x20;
        const byte SD_SubCmd_PutFilePart = 0x21;
        const byte SD_SubCmd_DelFile = 0x30;
        const byte SD_SubCmd_RenFileOld = 0x31;
        const byte SD_SubCmd_RenFileNew = 0x32;
        const byte SD_SubCmd_DelDir = 0x40;
        const byte SD_SubCmd_RenDirOld = 0x41;
        const byte SD_SubCmd_RenDirNew = 0x42;
        const byte SD_SubCmd_CreateDir = 0x43;
        const byte SD_SubCmd_Format = 0x50;

        enum FxError
        {
            FX_SUCCESS = 0x00,
            FX_BOOT_ERROR = 0x01,
            FX_MEDIA_INVALID = 0x02,
            FX_FAT_READ_ERROR = 0x03,
            FX_NOT_FOUND = 0x04,
            FX_NOT_A_FILE = 0x05,
            FX_ACCESS_ERROR = 0x06,
            FX_NOT_OPEN = 0x07,
            FX_FILE_CORRUPT = 0x08,
            FX_END_OF_FILE = 0x09,
            FX_NO_MORE_SPACE = 0x0A,
            FX_ALREADY_CREATED = 0x0B,
            FX_INVALID_NAME = 0x0C,
            FX_INVALID_PATH = 0x0D,
            FX_NOT_DIRECTORY = 0x0E,
            FX_NO_MORE_ENTRIES = 0x0F,
            FX_DIR_NOT_EMPTY = 0x10,
            FX_MEDIA_NOT_OPEN = 0x11,
            FX_INVALID_YEAR = 0x12,
            FX_INVALID_MONTH = 0x13,
            FX_INVALID_DAY = 0x14,
            FX_INVALID_HOUR = 0x15,
            FX_INVALID_MINUTE = 0x16,
            FX_INVALID_SECOND = 0x17,
            FX_PTR_ERROR = 0x18,
            FX_INVALID_ATTR = 0x19,
            FX_CALLER_ERROR = 0x20,
            FX_BUFFER_ERROR = 0x21,
            FX_NOT_IMPLEMENTED = 0x22,
            FX_WRITE_PROTECT = 0x23,
            FX_INVALID_OPTION = 0x24,
            FX_SECTOR_INVALID = 0x89,
            FX_IO_ERROR = 0x90,
            FX_NOT_ENOUGH_MEMORY = 0x91,
            FX_ERROR_FIXED = 0x92,
            FX_ERROR_NOT_FIXED = 0x93,
            FX_NOT_AVAILABLE = 0x94,
            FX_INVALID_CHECKSUM = 0x95,
            FX_READ_CONTINUE = 0x96,
            FX_INVALID_STATE = 0x97,
            FX_NOT_INIT_TRANSFER = 0xF0,
            FX_CRC_ERROR = 0xF1,
        }
        #endregion
        int timeout = 1000;
        bool bContinue = true;
        byte DeviceAddress;
        MainForm MainForm;

        Encoding name_encoding = Encoding.ASCII;
        //Encoding name_encoding = Encoding.UTF8;
        //Encoding name_encoding = CodePagesEncodingProvider.Instance.GetEncoding(852);
        //Encoding name_encoding = CodePagesEncodingProvider.Instance.GetEncoding(1250);

        ///////////////////////////////////////////////////////////////////////////////////////////
        public SDCardCtrl()
        {
            MainForm = DevConfigService.Instance.MainForm;
            Debug.Assert(DevConfigService.Instance.InputPeriph != null);
            DevConfigService.Instance.InputPeriph.MessageReceived += InputPeriph_MessageReceived;
            InitializeComponent();
            lvwColumnSorter = new ListViewColumnSorter();
            listView1.ListViewItemSorter = lvwColumnSorter;
            MainForm.AbortEvent += MainForm_CancelEvent;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void SDCardCtrl_Load(object sender, EventArgs e)
        {
            Debug.Assert(DevConfigService.Instance.selectedDevice != null);
            DeviceAddress = DevConfigService.Instance.selectedDevice.Address;
            Debug.WriteLine($"CAN ID = {DeviceAddress}");
            PopulateTreeView();
        }
        #region FORM COMMAND
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
                        ListViewItem item = new ListViewItem(Path.GetFileName(f.Name), 1)
                        {
                            Tag = e.Node,
                        };
                        ListViewItem.ListViewSubItem[] subItems = new ListViewItem.ListViewSubItem[]
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
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        DateTime LvItemLastClikTime = DateTime.MinValue;
        ListViewItem? LvItemLastClik = null;
        private void listView1_SubItemClicked(object sender, SubItemEventArgs e)
        {
            if (e.SubItem == 0)
            {
                if (e.Item.Equals(LvItemLastClik) && (DateTime.Now - LvItemLastClikTime) <= new TimeSpan(0, 0, 2))
                {
                    LvItemLastClik = null;
                    listView1.StartEditing(textBox1, e.Item, e.SubItem);
                }
                else
                {
                    LvItemLastClikTime = DateTime.Now;
                    LvItemLastClik = e.Item;
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void listView1_SubItemEndEditing(object sender, SubItemEndEditingEventArgs e)
        {
            if (e.DisplayText != e.Item.Text)
            {
                Debug.WriteLine($"Rename {e.Item.Text} to {e.DisplayText}");
                RenameFile(e.Item.Text, e.DisplayText, e.Item);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (DevConfigService.Instance.ProcessLock())
            {
                DevConfigService.Instance.FreeProcessLock();
                switch ((Keys)e.KeyValue)
                {
                    case Keys.Insert:
                        AddFileMenuItem_Click(sender, new EventArgs());
                        break;

                    case Keys.Delete:
                        DelFileMenuItem_Click(sender, new EventArgs());
                        break;

                    case Keys.F2:
                        if (listView1.SelectedItems.Count == 1)
                            listView1.StartEditing(textBox1, listView1.SelectedItems[0], 0);
                        break;
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (DevConfigService.Instance.ProcessLock())
            {
                DevConfigService.Instance.FreeProcessLock();
                switch ((Keys)e.KeyValue)
                {
                    case Keys.Insert: AddDirMenuItem_Click(sender, new EventArgs()); break;
                    case Keys.Delete: DelDirMenuItem_Click(sender, new EventArgs()); break;
                    case Keys.F2:     RenDirMenuItem_Click(sender, new EventArgs()); break;
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label != null && e.Node.Text != e.Label)
            {
                if(!RenDirectory(e.Label))
                    e.CancelEdit = true;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public void GetFileMenuItem_Click(object sender, EventArgs e)
        {
            if (DevConfigService.Instance.ProcessLock())
            {
                DevConfigService.Instance.FreeProcessLock();
                if (listView1.SelectedItems.Count > 0)
                {
                    List<string> file_paths = new();

                    foreach (ListViewItem item in listView1.SelectedItems)
                    {
                        string path = MakePath((TreeNode)item.Tag!);
                        path = Path.Combine(path, item.Text).Replace('\\', '/');
                        file_paths.Add(path);
                    }

                    // Dotaz na adresar.
                    FolderBrowserDialog dialog = new FolderBrowserDialog();
                    if (dialog.ShowDialog() != DialogResult.OK)
                        return;
                    string dest_path = dialog.SelectedPath;

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

                    Task.Run(() =>
                    {
                        if (DevConfigService.Instance.ProcessLock())
                        {
                            CopyToLocal(file_paths, dest_path);
                            DevConfigService.Instance.FreeProcessLock();
                        }
                    });
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public void AddFileMenuItem_Click(object sender, EventArgs e)
        {
            if (DevConfigService.Instance.ProcessLock())
            {
                DevConfigService.Instance.FreeProcessLock();
                OpenFileDialog fileDialog = new OpenFileDialog()
                {
                    Multiselect = true,
                    Filter = "All files (*.*)|*.*",
                };

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    Task.Run(() =>
                    {
                        if (DevConfigService.Instance.ProcessLock())
                        {
                            AddFiles(fileDialog.FileNames);
                            DevConfigService.Instance.FreeProcessLock();
                        }
                    });
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public void RenFileMenuItem_Click(object sender, EventArgs e)
        {
            if (DevConfigService.Instance.ProcessLock())
            {
                DevConfigService.Instance.FreeProcessLock();
                if (listView1.SelectedItems.Count == 1)
                    listView1.StartEditing(textBox1, listView1.SelectedItems[0], 0);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public void DelFileMenuItem_Click(object sender, EventArgs e)
        {
            if (DevConfigService.Instance.ProcessLock())
            {
                DevConfigService.Instance.FreeProcessLock();
                if (listView1.SelectedItems.Count > 0)
                    DeleteFiles(listView1.SelectedItems);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void treeView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (DevConfigService.Instance.ProcessLock())
            {
                DevConfigService.Instance.FreeProcessLock();
                if (e.Button == MouseButtons.Right)
                    contextMenuTree.Show(Cursor.Position);
            }
        }

        public void BackupSdCardMenuItem_Click(object sender, EventArgs e)
        {
            if (DevConfigService.Instance.ProcessLock())
                SDBackup();
        }

        public void RestoreSdCardMenuItem_Click(object sender, EventArgs e)
        {
            if (DevConfigService.Instance.ProcessLock())
                SDRestore();
        }

        public void ReloadSdCardMenuItem_Click(object sender, EventArgs e)
        {
            if (DevConfigService.Instance.ProcessLock())
                PopulateTreeView();
        }

        public void FormatSdCardMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to format the SD card on your device?", "DevConfig - FORMAT", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                Task.Run(() => SDFormat(true) );
        }

        public void AddDirMenuItem_Click(object sender, EventArgs e)
        {
            AddDirGetName addDirGetName = new AddDirGetName();
            if (addDirGetName.ShowDialog() == DialogResult.OK)
            {
                string path = MakePath(treeView1.SelectedNode);
                if (!path.EndsWith('/'))
                    path += "/";
                path += addDirGetName.textBoxName.Text;

                if (AddDirectory(path))
                {
                    // OK vytvorime polozku ve stromu
                    DirInfo dirInfo1 = new DirInfo();
                    dirInfo1.FileInfoList = new List<FileInfo>();
                    TreeNode new_node = new TreeNode(Path.GetFileName(path));
                    new_node.ImageKey = "FolderClosed.bmp";
                    new_node.SelectedImageKey = "FolderClosed.bmp";
                    new_node.Tag = dirInfo1;
                    treeView1.SelectedNode.Nodes.Add(new_node);
                    ExpandNode(treeView1.SelectedNode, true);
                }
            }
        }

        public void RenDirMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Parent != null)
            {
                treeView1.LabelEdit = true;
                if (!treeView1.SelectedNode.IsEditing)
                    treeView1.SelectedNode.BeginEdit();
            }
/*
            AddDirGetName addDirGetName = new AddDirGetName();
            addDirGetName.textBoxName.Text = treeView1.SelectedNode.Text;
            if (addDirGetName.ShowDialog() == DialogResult.OK)
            {
                RenDirectory(addDirGetName.textBoxName.Text);
            }*/
        }

        public void DelDirMenuItem_Click(object sender, EventArgs e)
        {
            DelDirectory();
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void MainForm_CancelEvent()
        {
            bContinue = false;
            sync_obj.Set();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        ushort files_to_read = 0;
        uint file_len_req = 0;
        uint file_len_act = 0;
        uint file_crc32 = 0;
        uint file_timestamp = 0;
        byte file_attributes = 0;
        byte MessageFlag = 0;
        List<Message> messages = new List<Message>();
        readonly ManualResetEvent sync_obj = new(false);
        Dictionary<uint, List<byte>> file_bytes_map = new();

        private void InputPeriph_MessageReceived(Message msg)
        {
            if (msg.SRC == DeviceAddress)
            {
                if (msg.CMD == Command.ECmd_SD_Command)
                {
                    if (msg.Data.Count == 1 && msg.Data[0] == 0x0F)
                    {
                        bContinue = false;
                        MessageFlag = msg.Data[0];
                        sync_obj.Set();
                        return;
                    }
                    if (msg.Data.Count < 2)
                    {
                        bContinue = false;
                        MessageFlag = 0xFF;
                        sync_obj.Set();
                        return;
                    }
                    MessageFlag = msg.Data[1];
                    switch (msg.Data[0])
                    {
                        case SD_SubCmd_ListFiles:
                            if (MessageFlag == 0)
                            {
                                files_to_read = BitConverter.ToUInt16(msg.Data.Skip(2).Take(2).Reverse().ToArray());
                                Debug.WriteLine($"Get {files_to_read} files");
                            }
                            else
                            {
                                bContinue = false;
                                Debug.WriteLine($"Error {MessageFlag}");
                            }
                            sync_obj.Set();
                            break;
                        case SD_SubCmd_ListFilesPart: // FileInfo file
                            if (MessageFlag == 0)
                                messages.Add(msg);
                            else
                                bContinue = false;
                            sync_obj.Set();
                            break;
                        case SD_SubCmd_GetFile: // response zahájení čtení souboru
                            if (MessageFlag == 0)
                            {
                                file_bytes_map.Clear();
                                file_len_req = BitConverter.ToUInt32(msg.Data.Skip(2).Take(4).Reverse().ToArray());
                                file_crc32 = BitConverter.ToUInt32(msg.Data.Skip(6).Take(4).Reverse().ToArray());
                                file_timestamp = BitConverter.ToUInt32(msg.Data.Skip(10).Take(4).Reverse().ToArray());
                                // TODO file_attributes = msg.Data[14];
                                file_len_act = 0;
                                Debug.WriteLine($"Get file with {file_len_req} bytes");
                            }
                            else
                            {
                                bContinue = false;
                                Debug.WriteLine($"Get file Error {msg.Data[1]}");
                            }
                            sync_obj.Set();
                            break;
                        case SD_SubCmd_GetFilePart: // response čtení další části souboru
                            if (MessageFlag == 0)
                            {
                                uint file_pos = BitConverter.ToUInt32(msg.Data.Skip(2).Take(4).Reverse().ToArray());
                                if (file_bytes_map.ContainsKey(file_pos))
                                    file_len_act -= (uint)file_bytes_map[file_pos].Count;
                                file_bytes_map[file_pos] = msg.Data.Skip(6).ToList();
                                file_len_act += (uint)(msg.Data.Count - 6);
                            }
                            else
                            {
                                bContinue = false;
                                Debug.WriteLine($"Error {msg.Data[1]}");
                            }
                            sync_obj.Set();
                            break;
                        case SD_SubCmd_Format:
                        case SD_SubCmd_RenDirNew:
                        case SD_SubCmd_RenDirOld:
                        case SD_SubCmd_RenFileNew:
                        case SD_SubCmd_RenFileOld:
                        case SD_SubCmd_CreateDir:
                        case SD_SubCmd_DelFile:
                        case SD_SubCmd_DelDir:
                        case SD_SubCmd_PutFile: // response zahájení odeslání souboru
                        case SD_SubCmd_PutFilePart: // response odeslání další čísti souboru
                            if (MessageFlag != 0)
                            {
                                bContinue = false;
                                Debug.WriteLine($"Error {MessageFlag}");
                            }
                            sync_obj.Set();
                            break;
                    }
                }
            }
        }

        #region LIST FILES
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void ClearTree()
        {
            treeView1.Nodes.Clear();

            DirInfo file_info = new DirInfo(/*"/"*/);
            file_info.FileInfoList = new List<FileInfo>();

            TreeNode node = new TreeNode("SD Card");
            node.Name = "SDCard";
            node.ImageKey = "SDCard.png";
            node.SelectedImageKey = "SDCard.png";
            node.Tag = file_info;

            treeView1.Nodes.Add(node);

            treeView1.SelectedNode = node;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void PopulateTreeView(bool b_free_lock = true)
        {
            ClearTree();
            int node_ix = treeView1.Nodes.IndexOfKey("SDCard");
            TreeNode rootNode = treeView1.Nodes[node_ix];
            bContinue = true;
            ((DirInfo)rootNode.Tag).FileInfoList = null;
            ExpandNode(rootNode, true);
            treeView1.SelectedNode = rootNode;
            treeView1.Focus();
            if(b_free_lock)
                DevConfigService.Instance.FreeProcessLock();
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
                        DirInfo dirInfo1 = new DirInfo(/*file.Name*/);
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
        private string MakePath(TreeNode? node)
        {
            List<string> list = new List<string>();

            while (node != null)
            {
                if(node.Parent != null)
                    list.Add(node.Text);
                else
                    list.Add("/");
                node = node.Parent;
            }

            list.Reverse();

            return Path.Combine(list.ToArray()).Replace('\\', '/');
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private List<FileInfo> GetFileList(string directory)
        {
            uint err_cnt = 0;
            Cursor = Cursors.WaitCursor;
            MainForm.AppendToDebug($"GetFileList ({directory})");

            List<FileInfo> fileinfolist = new();

            Message message = new Message();
            message.DEST = DeviceAddress;
            message.CMD = Command.ECmd_SD_Command;

            message.Data = new List<byte>();
            message.Data.Add(SD_SubCmd_ListFiles);
            message.Data.AddRange(name_encoding.GetBytes(directory + "\0"));

            messages.Clear();

            while (bContinue)
            {
                sync_obj.Reset();
                DevConfigService.Instance.InputPeriph?.SendMsg(message);

                // Počkame na response požadavku na soubor.
                if (sync_obj.WaitOne(timeout))
                {
                    if (MessageFlag != 0)
                        bContinue = false;
                    break;
                }
                else
                {
                    if (++err_cnt == 3)
                    {
                        bContinue = false;
                        MainForm.AppendToDebug($"GetFileList TIMEOUT", true, false, Color.Red);
                        break;
                    }
                    else
                    {
                        MainForm.AppendToDebug($"GetFileList REPEAT", true, false, Color.Orange);
                    }
                }
            }

            err_cnt = 0;
            while (bContinue)
            {
                // Odešlšme požadavek na další soubor
                message.Data = new List<byte>() { SD_SubCmd_ListFilesPart };
                message.Data.AddRange(((ushort)messages.Count).GetBytes().Reverse());    // offset

                sync_obj.Reset();
                DevConfigService.Instance.InputPeriph?.SendMsg(message);

                if (sync_obj.WaitOne(timeout))
                {
                    err_cnt = 0;
                    if (MessageFlag == 0)
                    {
                        if (messages.Count == files_to_read)
                            break;
                    }
                    else
                    {
                        bContinue = false;
                        break;
                    }
                }
                else
                {
                    if (++err_cnt >= 3)
                    {
                        bContinue = false;
                        MainForm.AppendToDebug($"GetFileList TIMEOUT", true, false, Color.Red);
                        break;
                    }
                    else
                    {
                        MainForm.AppendToDebug($"GetFileList REPEAT", true, false, Color.Orange);
                    }
                }
            }

            if (bContinue)
            {
                foreach (Message msg in messages)
                {
                    byte error = msg.Data[1];
                    if (error == 0x00)
                    {
                        ushort idx = BitConverter.ToUInt16(msg.Data.Skip(2).Take(2).Reverse().ToArray());
                        byte attr = msg.Data[4];
                        string filename = name_encoding.GetString(msg.Data.Skip(13).ToArray());
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
                        MessageFlag = error;
                        break;
                    }
                }
            }

            if (MessageFlag != 0)
            {
                if(message.Data.Count == 3 && message.Data[0] == 0x1 && message.Data[1] == 0x2f && message.Data[2] == 0x0)
                    MainForm.AppendToDebug($"GetFileList Error - Device does not support SD card", true, false, Color.Red);
                else
                    MainForm.AppendToDebug($"GetFileList Error {(FxError)MessageFlag}", true, false, Color.Red);
            }
            else
                MainForm.AppendToDebug($"GetFileList OK", true, false, Color.DarkGreen);

            Cursor = Cursors.Default;
            return fileinfolist;

        }

#endregion
                
        #region BACKUP/RESTORE
        ///////////////////////////////////////////////////////////////////////////////////////////
        private bool SDFormat(bool init_progress)
        {
            bool bRet = false;
            const int progress_max = 60000;

            if (init_progress)
            {
                MainForm.Invoke(delegate
                {
                    MainForm.ProgressBar_Minimum = 0;
                    MainForm.ProgressBar_Maximum = progress_max;
                    MainForm.ProgressBar_Value = 0;
                });
            }

            MainForm.AppendToDebug("Format SD card");
            Message message = new ();
            message.DEST = DeviceAddress;
            message.CMD = Command.ECmd_SD_Command;
            message.Data = new List<byte>{ SD_SubCmd_Format, 0x12, 0x34 };
            bContinue = true;
            sync_obj.Reset();
            DevConfigService.Instance.InputPeriph?.SendMsg(message);

            int tim_cnt = progress_max;
            while (true)
            {
                if (sync_obj.WaitOne(1000))
                {
                    if (MessageFlag == (byte)FxError.FX_SUCCESS)
                    {
                        MainForm.AppendToDebug("Format SD card OK", true, false, Color.DarkGreen);
                        MainForm.Invoke(delegate { MainForm.ProgressBar_Value += tim_cnt; });
                        bRet = true;
                    }
                    else
                    {
                        MainForm.AppendToDebug($"Format SD card ERROR ({(FxError)MessageFlag})", true, false, Color.Red);
                    }
                    break;
                }
                else
                {
                    if (tim_cnt <= 0)
                    {
                        MainForm.AppendToDebugIf(bContinue, "Format SD card TIMEOUT", true, false, Color.Red);
                        break;
                    }
                    else
                    {
                        tim_cnt -= 1000;
                        MainForm.Invoke(delegate { MainForm.ProgressBar_Value += 1000; });
                    }
                }
            }

            if (init_progress)
                MainForm.Invoke(delegate { MainForm.ProgressBar_Value = 0; });

            return bRet;
        }

        Backup_t? BackupObj = null;

        private void SDBackup()
        {
            // Refrešneme data
            PopulateTreeView(false);

            // Vytvoříme backup object.
            BackupObj = Backup_t.Load(false);
            BackupPrepare(treeView1.TopNode);

            BackupForm backupForm = new BackupForm(BackupObj) { Tag = this, Text = "DevConfig - Backup" };
            if (backupForm.ShowDialog() == DialogResult.OK)
            {
                BackupObj.Save();

                // Backup spustime.
                Task.Run(() =>
                {
                    bContinue = true;

                    MainForm.AppendToDebug($"Backup", true, true);
                    MainForm.Invoke(delegate
                    {
                        MainForm.ProgressBar_Value = 0;
                        MainForm.ProgressBar_Minimum = 0;
                        MainForm.ProgressBar_Maximum = (int)BackupObj.size;
                    });

                    for (int i = 0; bContinue && i < BackupObj.files.Count; i++)
                    {
                        string src_path = BackupObj.files[i].Name;
                        string dst_path = Path.Combine(BackupObj.backup_destination, $"{src_path.TrimStart('/')}");

                        if (BackupObj.files[i].IsDirectory)
                        {
                            Directory.CreateDirectory(dst_path);
                        }
                        else
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(dst_path)!);
                            CopyToLocal(src_path, dst_path, false);
                        }
                    }

                    if (bContinue)
                        MainForm.AppendToDebug($"Backup OK", true, true, Color.DarkGreen);
                    else
                        MainForm.AppendToDebug($"Backup NOK", true, true, Color.Red);

                    MainForm.Invoke(delegate { MainForm.ProgressBar_Value = 0; });
                    DevConfigService.Instance.FreeProcessLock();
                });
            }
            else
                DevConfigService.Instance.FreeProcessLock();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public void BackupPrepare(TreeNode node)
        {
            Debug.Assert(BackupObj != null);

            // projdeme rekurzivne soubory
            var di = ((DirInfo)node.Tag);

            if(di.FileInfoList == null)
                ExpandNode(node, false);

            Debug.Assert(di.FileInfoList != null);

            foreach (var fi in di.FileInfoList)
            {
                FileInfo new_fi = new(fi);
                new_fi.Name = $"{MakePath(node).TrimEnd('/')}/{fi.Name}";

                if (fi.IsDirectory)
                {
                    BackupObj.dir_cnt++;
                    BackupObj.files.Add(new_fi);
                }
                else
                {
                    string ext = Path.GetExtension(fi.Name).ToLower();
                    var exclude = (from xx in BackupObj.exclude_ext where xx.CompareTo(ext) == 0 select xx).Count() != 0;
                    //Debug.WriteLine($"{exclude} {fi.Name}");
                    if (!exclude)
                    {
                        BackupObj.file_cnt++;
                        BackupObj.size += fi.Size;
                        BackupObj.files.Add(new_fi);
                    }
                }
            }

            // projdeme adresare
            foreach (var child_node in node.Nodes)
                BackupPrepare((TreeNode)child_node);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void SDRestore()
        {
            BackupObj = Backup_t.Load(true);
            
            if(!string.IsNullOrWhiteSpace(BackupObj.backup_destination))
                RestorePrepare(BackupObj.backup_destination);

            BackupForm backupForm = new BackupForm(BackupObj) { Tag = this };
            if (backupForm.ShowDialog() == DialogResult.OK)
            {
                BackupObj.Save();

                Task.Run(() =>
                {
                    MainForm.AppendToDebug("Restore", true, true);
                    bContinue = true;
                    if (SDFormat(true))
                    {
                        MainForm.Invoke(delegate
                        {
                            MainForm.ProgressBar_Minimum = 0;
                            MainForm.ProgressBar_Maximum = ((int)BackupObj.size);
                            MainForm.ProgressBar_Value = 0;
                            ClearTree();
                        });

                        for (int i = 0; bContinue && i < BackupObj.files.Count; i++)
                        {
                            if (BackupObj.files[i].IsDirectory)
                                AddDirectory(BackupObj.files[i].Name.Substring(BackupObj.backup_destination.Length));
                            else
                                AddFile(new System.IO.FileInfo(BackupObj.files[i].Name), BackupObj.backup_destination, false);
                        }

                        MainForm.Invoke(delegate 
                        {
                            PopulateTreeView(false);
                            MainForm.ProgressBar_Value = 0;
                        });
                    }
                    if (bContinue)
                        MainForm.AppendToDebug("Restore OK", true, true, Color.DarkGreen);
                    else
                        MainForm.AppendToDebug("Restore NOK", true, true, Color.Red);

                    DevConfigService.Instance.FreeProcessLock();
                });
            }
            else
            {
                DevConfigService.Instance.FreeProcessLock();
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        internal void RestorePrepare(string dir_name)
        {
            Debug.Assert(BackupObj != null);

            DirectoryInfo di = new DirectoryInfo(dir_name);

            Debug.WriteLine($"D {di.FullName}");
            FileInfo fi = new FileInfo(dir_name);
            fi.IsDirectory = true;
            fi.ModifyTime = di.LastWriteTime;
            BackupObj.dir_cnt++;
            BackupObj.files.Add(fi);

            foreach (var f in di.EnumerateFiles())
            {
                string ext = Path.GetExtension(f.Extension).ToLower();
                var exclude = (from xx in BackupObj.exclude_ext where xx.CompareTo(ext) == 0 select xx).Count() != 0;
                if (!exclude)
                {
                    Debug.WriteLine($"F {f.FullName}");
                    fi = new FileInfo(f.FullName);
                    fi.IsDirectory = false;
                    fi.ModifyTime = f.LastWriteTime;
                    fi.Size = (uint)f.Length;
                    BackupObj.file_cnt++;
                    BackupObj.size += (ulong)f.Length;
                    BackupObj.files.Add(fi);
                }
                else
                {
                    Debug.WriteLine($"E {f.FullName}");
                }
            }

            // Rekurze do podrizenych adresru.
            foreach (var d in di.EnumerateDirectories())
                RestorePrepare(d.FullName);
        }

        #endregion

        #region GET FILE
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void CopyToLocal(List<string> file_paths, string dest_path)
        {
            // Kopirujeme
            this.Invoke(delegate { Cursor = Cursors.WaitCursor; });

            bContinue = true;
            foreach (string file_path in file_paths)
            {
                if (!bContinue)
                    break;

                string local_file_path = Path.Combine(dest_path, Path.GetFileName(file_path));
                File.Delete(local_file_path);
                CopyToLocal(file_path, local_file_path, true);
                Debug.WriteLine($"{file_path}\n   {local_file_path}");
            }
            this.Invoke(delegate
            {
                Cursor = Cursors.Default;
                MainForm.ProgressBar_Value = 0;
            });
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void CopyToLocal(string file_path, string local_file_path, bool init_progress)
        {
            uint err_cnt = 0;
            int last_progress_val = 0;
            bool b_init_progress_bar = init_progress;

            MainForm.AppendToDebug($"Get File ({file_path})");

            // Odešleme požadavek na soubor.
            Message message = new Message();
            message.DEST = DeviceAddress;
            message.CMD = Command.ECmd_SD_Command;

            message.Data = new List<byte>() { SD_SubCmd_GetFile };
            message.Data.AddRange(name_encoding.GetBytes(file_path + "\0"));

            while (bContinue)
            {
                sync_obj.Reset();
                DevConfigService.Instance.InputPeriph?.SendMsg(message);

                // Počkame na response požadavku na soubor.
                if (sync_obj.WaitOne(timeout))
                {
                    if (MessageFlag != 0)
                    {
                        bContinue = false;
                        MainForm.AppendToDebug($"Get File ERROR ({(FxError)MessageFlag})", true, false, Color.Red);
                    }
                    break;
                }
                else
                {
                    if (++err_cnt == 3)
                    {
                        bContinue = false;
                        MainForm.AppendToDebug($"Get File TIMEOUT", true, false, Color.Red);
                        break;
                    }
                    else
                    {
                        MainForm.AppendToDebug($"Get File REPEAT", true, false, Color.Orange);
                    }
                }
            }

            // Přečteme data souboru
            while (bContinue)
            {
                // Odešlšme požadavek na další soubor
                message.Data = new List<byte>() { SD_SubCmd_GetFilePart };
                message.Data.AddRange(((uint)file_len_act).GetBytes().Reverse());    // offset

                sync_obj.Reset();
                DevConfigService.Instance.InputPeriph?.SendMsg(message);

                // Počkáme na přijaté data
                if (sync_obj.WaitOne(timeout))
                {
                    if (MessageFlag != 0)
                    {
                        bContinue = false;
                        MainForm.AppendToDebug($"Get File ERROR ({(FxError)MessageFlag})", true, false, Color.Red);
                        break;
                    }

                    err_cnt = 0;

                    if (file_len_req == file_len_act)
                    {
                        // Soubor je kompletní
                        byte[] file_data = new byte[file_len_req];

                        foreach (uint pos in file_bytes_map.Keys)
                            file_bytes_map[pos].CopyTo(file_data, (int)pos);

                        uint _crc32 = CRC.CRC32WideFast(file_data, 0, file_len_req);
                        if (_crc32 != file_crc32)
                        {
                            MainForm.AppendToDebug($"Get File CRC ERROR", true, false, Color.Red);
                        }
                        else
                        {
                            File.WriteAllBytes(local_file_path, file_data);
                            MainForm.AppendToDebug($"Get File OK", true, false, Color.DarkGreen);
                            if (init_progress)
                            {
                                MainForm.Invoke(delegate { MainForm.ProgressBar_Value = 0; });
                            }
                            else
                            {
                                int diff = (int)file_len_act - last_progress_val;
                                last_progress_val += diff;
                                MainForm.Invoke(delegate { MainForm.ProgressBar_Value += diff; });
                            }
                        }
                        break;
                    }
                    else
                    {
                        // Máme další část souboru
                        MainForm.Invoke(delegate
                        {
                            if (b_init_progress_bar)
                            {
                                b_init_progress_bar = false;
                                MainForm.ProgressBar_Minimum = 0;
                                MainForm.ProgressBar_Maximum = (int)file_len_req;
                            }

                            int diff = (int)file_len_act - last_progress_val;
                            MainForm.ProgressBar_Value += diff;
                            last_progress_val += diff;
                        });
                    }
                }
                else
                {
                    if(++err_cnt >= 3)
                    {
                        bContinue = false;
                        MainForm.AppendToDebug($"Get File TIMEOUT", true, false, Color.Red);
                        break;
                    }
                    else
                    {
                        MainForm.AppendToDebug($"Get File REPEAT", true, false, Color.Orange);
                    }
                }
            }
            lock (file_bytes_map)
                file_bytes_map.Clear();
        }
        #endregion

        #region ADD FILE
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void AddFiles(string[] fileNames)
        {
            this.Invoke(delegate { Cursor = Cursors.WaitCursor; });

            bContinue = true;
            for (int i = 0; bContinue && i < fileNames.Length; i++)
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileNames[i]);

                if (AddFile(fileInfo))
                {
                    FileInfo fi = new FileInfo(Path.GetFileName(fileNames[i]))
                    {
                        IsDirectory = false,
                        ModifyTime = fileInfo.LastWriteTime,
                        Size = (uint)fileInfo.Length
                    };

                    MainForm.Invoke(delegate
                    {
                        ListViewItem item = new ListViewItem(Path.GetFileName(fi.Name), 1)
                        {
                            Tag = treeView1.SelectedNode,
                        };

                        ListViewItem.ListViewSubItem[] subItems = new ListViewItem.ListViewSubItem[]
                        {
                                new ListViewItem.ListViewSubItem(item, $"{fi.ModifyTime}"),
                                new ListViewItem.ListViewSubItem(item, $"{fi.Size}")
                        };
                        item.SubItems.AddRange(subItems);

                        listView1.Items.Add(item);
                        ((DirInfo)treeView1.SelectedNode.Tag).FileInfoList?.Add(fi);
                    });
                }
            }


            this.Invoke(delegate
            {
                Cursor = Cursors.Default;
                MainForm.ProgressBar_Value = 0;
                listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            });
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        private bool AddFile(System.IO.FileInfo fileInfo, string? abs_dir = null, bool init_progress = true)
        {
            bool bRet = false;
            int progress_val = 0;
            string dest_file_name = string.Empty;

            this.Invoke(delegate
            {
                if (init_progress)
                {
                    MainForm.ProgressBar_Minimum = 0;
                    MainForm.ProgressBar_Maximum = (int)fileInfo.Length;
                    MainForm.ProgressBar_Value = progress_val;
                }

                if (abs_dir == null)
                    dest_file_name = Path.Combine(MakePath(treeView1.SelectedNode), Path.GetFileName(fileInfo.FullName));
                else
                    dest_file_name = Path.Combine(fileInfo.FullName.Substring(abs_dir.Length));
            });

            dest_file_name = dest_file_name.Replace("\\", "/");

            MainForm.AppendToDebug($"Copy file '{fileInfo.FullName}', to '{dest_file_name}', len = {fileInfo.Length}");

            // fileInfo.LastWriteTime je localtime ale potrebujeme pro prevod utc time
            var dt = DateTime.SpecifyKind(fileInfo.LastWriteTime, DateTimeKind.Utc);
            var dateTimeOffset = new DateTimeOffset(dt);
            var timestamp = dateTimeOffset.ToUnixTimeSeconds();

            byte[] file_bytes = File.ReadAllBytes(fileInfo.FullName);

            Message message = new Message();
            message.DEST = DeviceAddress;
            message.CMD = Command.ECmd_SD_Command;

            message.Data = new List<byte> { SD_SubCmd_PutFile };
            message.Data.AddRange(((uint)fileInfo.Length).GetBytes().Reverse());    // size
            message.Data.AddRange(((uint)timestamp).GetBytes().Reverse());          // timestamp
            Debug.WriteLine($"timestamp = {timestamp}({timestamp:X8}) {fileInfo.LastWriteTime}");
            uint crc32 = CRC.CRC32WideFast(file_bytes, 0, (uint)fileInfo.Length);
            message.Data.AddRange(((uint)crc32).GetBytes().Reverse());              // crc32
            Debug.WriteLine($"crc32 = {crc32}({crc32:X8})");
            message.Data.AddRange(name_encoding.GetBytes(dest_file_name + "\0"));  // file name

            sync_obj.Reset();
            DevConfigService.Instance.InputPeriph?.SendMsg(message);

            using MemoryStream fs = new MemoryStream(file_bytes);

            byte[] data = new byte[240];

            while (bContinue)
            {
                if (sync_obj.WaitOne(timeout))
                {
                    if (MessageFlag == 0)
                    {
                        message.Data = new List<byte>() { SD_SubCmd_PutFilePart };
                        int readed = fs.Read(data, 0, data.Length);
                        MainForm.Invoke(delegate { MainForm.ProgressBar_Value += readed; }); // MainForm.Invoke(delegate { MainForm.ProgressBar_Value = (progress_val += readed); });
                        Debug.WriteLine($"readed {readed}");
                        if (readed <= 0)
                        {
                            bRet = true;
                            MainForm.AppendToDebug($"Add file OK", true, false, Color.DarkGreen);
                            break;
                        }
                        message.Data.AddRange(data.Take(readed));

                        sync_obj.Reset();
                        DevConfigService.Instance.InputPeriph?.SendMsg(message);
                    }
                    else
                    {
                        bContinue = false;
                        MainForm.AppendToDebug($"Add file ERROR ({(FxError)MessageFlag})", true, false, Color.Red);
                        break;
                    }
                }
                else
                {
                    bContinue = false;
                    MainForm.AppendToDebug($"Add file TIMEOUT", true, false, Color.Red);
                    break;
                }
            }

            if (init_progress)
                MainForm.Invoke(delegate { MainForm.ProgressBar_Value = 0; });

            return bRet;
        }

        #endregion

        #region DEL,REN FILE
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void DeleteFiles(SelectedListViewItemCollection selectedItems)
        {
            Cursor = Cursors.WaitCursor;

            foreach (ListViewItem item in listView1.SelectedItems)
            {
                string path = MakePath((TreeNode)item.Tag);
                path = Path.Combine(path, item.Text).Replace('\\', '/');
                if (DeleteFile(path))
                {
                    ((DirInfo)((TreeNode)item.Tag).Tag).RemoveFileInfo(Path.GetFileName(path));
                    listView1.Items.Remove(item);
                }

            }
            Cursor = Cursors.Default;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private bool DeleteFile(string file_path)
        {
            bool bRet = false;

            MainForm.AppendToDebug($"Delete File ({file_path})");

            Message message = new Message();
            message.DEST = DeviceAddress;
            message.CMD = Command.ECmd_SD_Command;

            message.Data = new List<byte>();
            message.Data.Add(SD_SubCmd_DelFile);
            message.Data.AddRange(name_encoding.GetBytes(file_path + "\0"));

            sync_obj.Reset();
            DevConfigService.Instance.InputPeriph?.SendMsg(message);

            if (sync_obj.WaitOne(timeout))
            {
                if (MessageFlag == (byte)FxError.FX_SUCCESS)
                {
                    bRet = true;
                    MainForm.AppendToDebug("Delete File OK", true, false, Color.DarkGreen);
                }
                else
                {
                    MainForm.AppendToDebugIf(bContinue, $"Delete File ERROR ({(FxError)MessageFlag})", true, false, Color.Red);
                }
            }
            else
            {
                MainForm.AppendToDebugIf(bContinue, "Delete File TIMEOUT", true, false, Color.Red);
            }
            return bRet;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void RenameFile(string old_file_name, string new_file_name, ListViewItem item)
        {
            string path = MakePath(treeView1.SelectedNode);
            if (!path.EndsWith('/'))
                path += "/";
            if (MoveFile(path + old_file_name, path + new_file_name))
            {
                item.Text = Path.GetFileName(new_file_name);
                ((DirInfo)((TreeNode)item.Tag).Tag).RenameFileInfo(old_file_name, new_file_name);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private bool MoveFile(string old_file_name, string new_file_name)
        {
            bool bRet = true;
            bContinue = true;

            MainForm.AppendToDebug($"Rename File ({old_file_name}) to ({new_file_name})");

            Message[] msg_arr = new Message[2];

            msg_arr[0] = new Message();
            msg_arr[0].DEST = DeviceAddress;
            msg_arr[0].CMD = Command.ECmd_SD_Command;
            msg_arr[0].Data = new List<byte>() { SD_SubCmd_RenFileOld };
            msg_arr[0].Data.AddRange(name_encoding.GetBytes(old_file_name + "\0"));

            msg_arr[1] = new Message();
            msg_arr[1].DEST = DeviceAddress;
            msg_arr[1].CMD = Command.ECmd_SD_Command;
            msg_arr[1].Data = new List<byte> { SD_SubCmd_RenFileNew };
            msg_arr[1].Data.AddRange(name_encoding.GetBytes(new_file_name + "\0"));

            for (int i = 0; bContinue && i < msg_arr.Length; i++)
            {
                sync_obj.Reset();
                DevConfigService.Instance.InputPeriph?.SendMsg(msg_arr[i]);

                if (sync_obj.WaitOne(timeout))
                {
                    if (MessageFlag != (byte)FxError.FX_SUCCESS)
                    {
                        bRet = false;
                        bContinue = false;
                        MainForm.AppendToDebug($"Rename File ERROR ({(FxError)MessageFlag})", true, false, Color.Red);
                        break;
                    }
                }
                else
                {
                    bRet = false;
                    bContinue = false;
                    MainForm.AppendToDebug("Rename File TIMEOUT", true, false, Color.Red);
                    break;
                }

            }

            if (bRet)
                MainForm.AppendToDebug("Rename File OK", true, false, Color.DarkGreen);

            return bRet;
        }

        #endregion

        #region DIRECTORY
        ///////////////////////////////////////////////////////////////////////////////////////////
        private bool AddDirectory(string path)
        {
            bool bRet = false;

            path.Replace("\\", "/").Replace("//", "/");

            if (string.IsNullOrWhiteSpace(path) || path == "/")
                return false;

            MainForm.AppendToDebug($"Create Directory ({path})");

            var dt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            var dateTimeOffset = new DateTimeOffset(dt);
            var timestamp = dateTimeOffset.ToUnixTimeSeconds();

            Message message = new Message();
            message.DEST = DeviceAddress;
            message.CMD = Command.ECmd_SD_Command;
            message.Data = new List<byte>();
            message.Data.Add(SD_SubCmd_CreateDir);                                  // sub cmd
            message.Data.AddRange(((uint)timestamp).GetBytes().Reverse());          // timestamp
            message.Data.AddRange(name_encoding.GetBytes(path + "\0"));            // dir name

            sync_obj.Reset();
            bContinue = true;
            DevConfigService.Instance.InputPeriph?.SendMsg(message);
            if (sync_obj.WaitOne(timeout))
            {
                if (MessageFlag == (byte)FxError.FX_SUCCESS)
                {
                    MainForm.AppendToDebug("Create Directory OK", true, false, Color.DarkGreen);
                    bRet = true;
                }
                else
                {
                    MainForm.AppendToDebug($"Create Directory ERROR ({(FxError)MessageFlag})", true, false, Color.Red);
                }
            }
            else
            {
                if (bContinue)
                    MainForm.AppendToDebug("Create Directory TIMEOUT", true, false, Color.Red);
            }
            return bRet;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void DelDirectory()
        {
            if (treeView1.SelectedNode.Level > 0)
            {
                string path = MakePath(treeView1.SelectedNode);

                MainForm.AppendToDebug($"Delete Directory ({path})");

                Message message = new Message();
                message.DEST = DeviceAddress;
                message.CMD = Command.ECmd_SD_Command;
                message.Data = new List<byte>();
                message.Data.Add(SD_SubCmd_DelDir);
                message.Data.AddRange(name_encoding.GetBytes(path + "\0"));
                sync_obj.Reset();
                bContinue = true;
                DevConfigService.Instance.InputPeriph?.SendMsg(message);
                if (sync_obj.WaitOne(timeout))
                {
                    if (MessageFlag == (byte)FxError.FX_SUCCESS)
                    {
                        MainForm.AppendToDebug("Delete Directory OK", true, false, Color.DarkGreen);
                        // OK odstranime polozku ze stromu
                        treeView1.Nodes.Remove(treeView1.SelectedNode);
                    }
                    else
                    {
                        MainForm.AppendToDebug($"Delete Directory ERROR ({(FxError)MessageFlag})", true, false, Color.Red);
                    }
                }
                else
                {
                    if (bContinue)
                        MainForm.AppendToDebug("Delete Directory TIMEOUT", true, false, Color.Red);
                }
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        private bool RenDirectory(string text)
        {
            bool bRet = false;
            if (treeView1.SelectedNode.Level > 0)
            {
                string path = MakePath(treeView1.SelectedNode);

                string new_path = MakePath(treeView1.SelectedNode.Parent);
                if (!new_path.EndsWith('/'))
                    new_path += "/";
                new_path += text;

                MainForm.AppendToDebug($"Rename Directory ({path}) to ({new_path})");

                Message[] msg_arr = new Message[2];

                msg_arr[0] = new Message();
                msg_arr[0].DEST = DeviceAddress;
                msg_arr[0].CMD = Command.ECmd_SD_Command;
                msg_arr[0].Data = new List<byte>() { SD_SubCmd_RenDirOld };
                msg_arr[0].Data.AddRange(name_encoding.GetBytes(path + "\0"));

                msg_arr[1] = new Message();
                msg_arr[1].DEST = DeviceAddress;
                msg_arr[1].CMD = Command.ECmd_SD_Command;
                msg_arr[1].Data = new List<byte> { SD_SubCmd_RenDirNew };
                msg_arr[1].Data.AddRange(name_encoding.GetBytes(new_path + "\0"));

                bContinue = true;
                foreach (Message message in msg_arr)
                {
                    if (!bContinue)
                        break;

                    sync_obj.Reset();
                    DevConfigService.Instance.InputPeriph?.SendMsg(message);

                    if (sync_obj.WaitOne(timeout))
                    {
                        if (MessageFlag != (byte)FxError.FX_SUCCESS)
                        {
                            bContinue = false;
                            MessageFlag = 0xFF;
                            MainForm.AppendToDebug($"Rename Directory ERROR ({(FxError)MessageFlag})", true, false, Color.Red);
                            break;
                        }
                    }
                    else
                    {
                        bContinue = false;
                        MessageFlag = 0xFF;
                        MainForm.AppendToDebug("Rename Directory TIMEOUT", true, false, Color.Red);
                        break;
                    }

                }

                if (MessageFlag == (byte)FxError.FX_SUCCESS)
                {
                    bRet = true;
                    MainForm.AppendToDebug("Rename Directory OK", true, false, Color.DarkGreen);
                    treeView1.SelectedNode.Text = Path.GetFileName(new_path);
                    //((DirInfo)treeView1.SelectedNode.Tag).Name = Path.GetFileName(new_path);
                }
            }
            return bRet;
        }
        #endregion
    }
}

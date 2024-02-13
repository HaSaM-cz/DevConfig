using CanDiagSupport;
using DevConfig.Service;
using DevConfig.Utils;
using System.Data;
using System.Diagnostics;
using System.Text;
using WeifenLuo.WinFormsUI.Docking;
using static DevConfig.Service.DevConfigService;
using Message = CanDiagSupport.Message;

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
        const byte SD_SubCmd_PutFile = 0x20;
        const byte SD_SubCmd_PutFilePart = 0x21;
        const byte SD_SubCmd_DelFile = 0x30;
        const byte SD_SubCmd_RenFileOld = 0x31;
        const byte SD_SubCmd_RenFileNew = 0x32;
        const byte SD_SubCmd_DelDir = 0x40;
        const byte SD_SubCmd_RenDirOld = 0x41;
        const byte SD_SubCmd_RenDirNew = 0x42;
        const byte SD_SubCmd_CreateDir = 0x43;
        const byte SD_SubCmd_Abort = 0xF0;
        const byte SD_SubCmd_Format = 0xF1;

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

        bool bSendBreak = false;
        bool bContinue = true;
        byte DeviceAddress;
        MainForm MainForm;

        Encoding name_encoding = Encoding.ASCII;
        //Encoding name_encoding = Encoding.UTF8;
        //Encoding name_encoding = CodePagesEncodingProvider.Instance.GetEncoding(852);
        //Encoding name_encoding = CodePagesEncodingProvider.Instance.GetEncoding(1250);

        ///////////////////////////////////////////////////////////////////////////////////////////
        public SDCardCtrl(MainForm mainForm)
        {
            MainForm = mainForm;
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
            Text += $" ({DeviceAddress:X2})";
            PopulateTreeView();
        }
        #region FORM COMMAND
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void btn_List_Click(object sender, EventArgs e)
        {
            if (Control.ModifierKeys == (Keys.Shift | Keys.Control))
                SDFormat();
            else
                PopulateTreeView();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void btn_Add_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "All files (*.*)|*.*";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                Task.Run(() => { AddFiles(fileDialog.FileNames); });
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void btn_Get_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                List<string> file_paths = new();

                foreach (ListViewItem item in listView1.SelectedItems)
                {
                    string path = MakePath((TreeNode)item.Tag);
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

                Task.Run(() => { CopyToLocal(file_paths, dest_path); });
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void btn_DelFile_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                List<string> file_paths = new();

                foreach (ListViewItem item in listView1.SelectedItems)
                {
                    string path = MakePath((TreeNode)item.Tag);
                    path = Path.Combine(path, item.Text).Replace('\\', '/');
                    file_paths.Add(path);
                }

                DeleteFiles(file_paths);
            }
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
        private void btn_AddDir_Click(object sender, EventArgs e)
        {
            AddDirGetName addDirGetName = new AddDirGetName();
            if(addDirGetName.ShowDialog() == DialogResult.OK)
            {
                AddDirectory(addDirGetName.textBoxName.Text);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void btn_DelDir_Click(object sender, EventArgs e)
        {
            DelDirectory();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void btn_RenDir_Click(object sender, EventArgs e)
        {
            AddDirGetName addDirGetName = new AddDirGetName();
            addDirGetName.textBoxName.Text = treeView1.SelectedNode.Text;
            if (addDirGetName.ShowDialog() == DialogResult.OK)
            {
                RenDirectory(addDirGetName.textBoxName.Text);
            }
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void MainForm_CancelEvent()
        {
            if (bSendBreak)
            {
                bSendBreak = false;
                Message message = new Message();
                message.DEST = DeviceAddress;
                message.CMD = ECmd_SD_Command;
                message.Data = new List<byte>() { SD_SubCmd_Abort };
                DevConfigService.Instance.InputPeriph?.SendMsg(message);
            }

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
                if (msg.CMD == ECmd_SD_Command)
                {
                    if (msg.Data.Count == 1 && msg.Data[0] == 0x0F )
                    {
                        bContinue = false;
                        MainForm.AppendToDebug("Unknown operation", true, true, Color.Red);
                        return;
                    }
                    if (msg.Data.Count < 2)
                    {
                        bContinue = false;
                        MainForm.AppendToDebug($"Bad msg length {msg.Data.Count} bytes.", true, true, Color.Red);
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
                        case SD_SubCmd_FileItemName: // FileInfo file
                            if (MessageFlag == 0)
                            {
                                lock (messages)
                                    messages.Add(msg);
                            }
                            else
                            {
                                bContinue = false;
                            }
                            sync_obj.Set();
                            break;

                        // response zahájení čtení souboru
                        case SD_SubCmd_GetFile:
                            if (msg.Data[1] != 0)
                            {
                                Debug.WriteLine($"Error {msg.Data[1]}");
                                return;
                            }
                            file_bytes_map.Clear();
                            file_len_req = BitConverter.ToUInt32(msg.Data.Skip(2).Take(4).Reverse().ToArray());
                            file_crc32 = BitConverter.ToUInt32(msg.Data.Skip(6).Take(4).Reverse().ToArray());
                            file_timestamp = BitConverter.ToUInt32(msg.Data.Skip(10).Take(4).Reverse().ToArray());
                            // TODO file_attributes = msg.Data[14];
                            file_len_act = 0;
                            Debug.WriteLine($"Get file with {file_len_req} bytes");
                            sync_obj.Set();
                            break;

                        // response čtení další čísti souboru
                        case SD_SubCmd_GetFilePart:
                            if (msg.Data[1] != 0)
                            {
                                Debug.WriteLine($"Error {msg.Data[1]}");
                                return;
                            }
                            uint file_pos = BitConverter.ToUInt32(msg.Data.Skip(2).Take(4).Reverse().ToArray());
                            lock(file_bytes_map)
                                file_bytes_map[file_pos] = msg.Data.Skip(6).ToList();
                            file_len_act += (uint)(msg.Data.Count - 6);
                            sync_obj.Set();
                            break;

                        case SD_SubCmd_Format:
                        case SD_SubCmd_RenDirNew:
                        case SD_SubCmd_RenDirOld:
                        case SD_SubCmd_RenFileNew:
                        case SD_SubCmd_RenFileOld:
                        case SD_SubCmd_CreateDir:
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
            bContinue = true;
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
            MainForm.AppendToDebug($"GetFileList ({directory})");

            List<FileInfo> fileinfolist = new();

            Message message = new Message();
            message.DEST = DeviceAddress;
            message.CMD = ECmd_SD_Command;

            message.Data = new List<byte>();
            message.Data.Add(SD_SubCmd_ListFiles);
            message.Data.AddRange(name_encoding.GetBytes(directory + "\0"));

            lock (messages)
                messages.Clear();
            sync_obj.Reset();

            DevConfigService.Instance.InputPeriph?.SendMsg(message);

            while (sync_obj.WaitOne(2000) && bContinue)
            {
                lock (messages)
                {
                    if (messages.Count == files_to_read)
                        break;
                }
                sync_obj.Reset();
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
                MainForm.AppendToDebug($"GetFileList Error {(FxError)MessageFlag}", true, false, Color.Red);
            else
                MainForm.AppendToDebug($"GetFileList OK", true, false, Color.DarkGreen);

            Cursor = Cursors.Default;
            return fileinfolist;

        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void SDFormat()
        {
            if (MessageBox.Show("Do you want to format the SD card on your device?", "DevConfig - FORMAT", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                Task.Run(() =>
                {
                    MainForm.AppendToDebug("Format SD card");

                    Debug.WriteLine("Format SD");
                    Message message = new Message();
                    message.DEST = DeviceAddress;
                    message.CMD = ECmd_SD_Command;
                    message.Data = new List<byte>();
                    message.Data.Add(SD_SubCmd_Format);
                    bContinue = true;
                    sync_obj.Reset();
                    DevConfigService.Instance.InputPeriph?.SendMsg(message);

                    if (sync_obj.WaitOne(3000))
                    {
                        if (MessageFlag == (byte)UpdateEnumFlags.RespOK)
                        {
                            MainForm.AppendToDebug("Format SD card OK", true, false, Color.DarkGreen);
                        }
                        else
                        {
                            MainForm.AppendToDebug($"Format SD card ERROR ({(UpdateEnumFlags)MessageFlag})", true, true, Color.Red);
                        }
                    }
                    else
                    {
                        if(bContinue)
                            MainForm.AppendToDebug("Format SD card TIMEOUT", true, true, Color.Red);
                    }
                });
            }
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
                CopyToLocal(file_path, local_file_path);
                Debug.WriteLine($"{file_path}\n   {local_file_path}");
            }
            this.Invoke(delegate 
            { 
                Cursor = Cursors.Default;
                MainForm.ProgressBar_Value = 0;
            });
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void CopyToLocal(string file_path, string local_file_path)
        {
            Message message = new Message();
            message.DEST = DeviceAddress;
            message.CMD = ECmd_SD_Command;

            message.Data = new List<byte>();
            message.Data.Add(SD_SubCmd_GetFile);
            message.Data.AddRange(name_encoding.GetBytes(file_path + "\0"));

            sync_obj.Reset();
            bSendBreak = true;
            DevConfigService.Instance.InputPeriph?.SendMsg(message);

            MainForm.Invoke(delegate 
            {
                MainForm.ProgressBar_Minimum = 0;
                MainForm.ProgressBar_Maximum = 1;
                MainForm.ProgressBar_Value = 0;
            });

            while (bContinue)
            {
                if (sync_obj.WaitOne(1000))
                {
                    if (!bContinue)
                        break;

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

                        // file_timestamp
                        DateTimeOffset dateTime = DateTimeOffset.FromUnixTimeSeconds(file_timestamp);
                        File.SetLastWriteTime(local_file_path, dateTime.DateTime);

                        // CRC32
                        uint crc32 = CRC.CRC32WideFast(File.ReadAllBytes(local_file_path), 0, file_len_req);
                        if (crc32 != file_crc32)
                        {
                            Debug.WriteLine("CRC error");
                            File.Delete(local_file_path);
                        }
                        break;
                    }
                    else
                    {
                        MainForm.Invoke(delegate
                        {
                            //if (MainForm.progressBar.Maximum < file_len_req)
                                MainForm.ProgressBar_Maximum = (int)file_len_req;

                            /*if (file_len_act > MainForm.progressBar.Maximum)
                                MainForm.progressBar.Value = MainForm.progressBar.Maximum;
                            else*/
                                MainForm.ProgressBar_Value = (int)file_len_act;
                        });
                    }
                }
                else
                {
                    Debug.WriteLine("Error Timeout");
                    break;
                }
            }
            lock(file_bytes_map)
                file_bytes_map.Clear();
            bSendBreak = false;
        }
        #endregion

        #region ADD FILE
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void AddFiles(string[] fileNames)
        {
            this.Invoke(delegate { Cursor = Cursors.WaitCursor; });

            bContinue = true;
            foreach (string file_name in fileNames)
            {
                if (!bContinue)
                    break;
                AddFile(file_name);
            }

            this.Invoke(delegate
            {
                Cursor = Cursors.Default;
                MainForm.ProgressBar_Value = 0;
            });
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void AddFile(string src_file_name)
        {
            string dest_file_name = string.Empty;
            this.Invoke(delegate {  
                dest_file_name = Path.Combine(MakePath(treeView1.SelectedNode), Path.GetFileName(src_file_name)).Replace('\\', '/');
            });

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(src_file_name);

            Debug.WriteLine($"Copy from '{src_file_name}', to '{dest_file_name}', len = {fileInfo.Length:X}");

            // fileInfo.LastWriteTime je localtime ale potrebujeme pro prevod utc time
            var dt = DateTime.SpecifyKind(fileInfo.LastWriteTime, DateTimeKind.Utc);
            var dateTimeOffset = new DateTimeOffset(dt);
            var timestamp = dateTimeOffset.ToUnixTimeSeconds();

            byte[] file_bytes = File.ReadAllBytes(src_file_name);

            Message message = new Message();
            message.DEST = DeviceAddress;
            message.CMD = ECmd_SD_Command;

            message.Data = new List<byte>();
            message.Data.Add(SD_SubCmd_PutFile);
            message.Data.AddRange(((uint)fileInfo.Length).GetBytes().Reverse());    // size
            message.Data.AddRange(((uint)timestamp).GetBytes().Reverse());          // timestamp
            Debug.WriteLine($"timestamp = {timestamp}({timestamp:X8}) {fileInfo.LastWriteTime}");
            uint crc32 = CRC.CRC32WideFast(file_bytes, 0, (uint)fileInfo.Length);
            message.Data.AddRange(((uint)crc32).GetBytes().Reverse());              // crc32
            Debug.WriteLine($"crc32 = {crc32}({crc32:X8})");
            message.Data.AddRange(name_encoding.GetBytes(dest_file_name + "\0"));  // file name

            sync_obj.Reset();
            DevConfigService.Instance.InputPeriph?.SendMsg(message);

            MemoryStream fs = new MemoryStream(file_bytes); //using FileStream fs = File.OpenRead(src_file_name);

            byte[] data = new byte[240];

            while (bContinue)
            {
                if (sync_obj.WaitOne(2000))
                {
                    message.Data = new List<byte>() { SD_SubCmd_PutFilePart };
                    int readed = fs.Read(data, 0, data.Length);
                    Debug.WriteLine($"readed {readed}");
                    if (readed <= 0)
                        break;
                    message.Data.AddRange(data.Take(readed));

                    sync_obj.Reset();
                    DevConfigService.Instance.InputPeriph?.SendMsg(message);
                }
                else
                {
                    // timeout
                    break;
                }
            }
        }

        #endregion

        #region DEL,REN FILE
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void DeleteFiles(List<string> file_paths)
        {
            Cursor = Cursors.WaitCursor;
            foreach (string file_path in file_paths)
            {
                Debug.WriteLine($"DleteFile {file_path}");
                DeleteFile(file_path);
            }
            Cursor = Cursors.Default;

        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void DeleteFile(string file_path)
        {
            Message message = new Message();
            message.DEST = DeviceAddress;
            message.CMD = ECmd_SD_Command;

            message.Data = new List<byte>();
            message.Data.Add(SD_SubCmd_DelFile);
            message.Data.AddRange(name_encoding.GetBytes(file_path + "\0"));

            DevConfigService.Instance.InputPeriph?.SendMsg(message);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void RenameFile(string old_file_name, string new_file_name, ListViewItem item)
        {
            string path = MakePath(treeView1.SelectedNode);
            if (!path.EndsWith('/'))
                path += "/";
            if (MoveFile(path + old_file_name, path + new_file_name))
                Debug.WriteLine("Move OK");
            else
                Debug.WriteLine("Move ERROR");
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private bool MoveFile(string old_file_name, string new_file_name)
        {
            Message message = new Message();
            message.DEST = DeviceAddress;
            message.CMD = ECmd_SD_Command;

            message.Data = new List<byte>();
            message.Data.Add(SD_SubCmd_RenFileOld);
            message.Data.AddRange(name_encoding.GetBytes(old_file_name + "\0"));
            sync_obj.Reset();
            DevConfigService.Instance.InputPeriph?.SendMsg(message);

            if (sync_obj.WaitOne(1000))
            {
                message.Data = new List<byte>();
                message.Data.Add(SD_SubCmd_RenFileNew);
                message.Data.AddRange(name_encoding.GetBytes(new_file_name + "\0"));
                sync_obj.Reset();
                DevConfigService.Instance.InputPeriph?.SendMsg(message);

                if (sync_obj.WaitOne(1000))
                    return true;
            }

            return false;
        }

        #endregion

        #region DIRECTORY
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void AddDirectory(string text)
        {
            string path = MakePath(treeView1.SelectedNode);
            if (!path.EndsWith('/'))
                path += "/";
            path += text;

            MainForm.AppendToDebug($"Create Directory ({path})");

            var dt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            var dateTimeOffset = new DateTimeOffset(dt);
            var timestamp = dateTimeOffset.ToUnixTimeSeconds();

            Message message = new Message();
            message.DEST = DeviceAddress;
            message.CMD = ECmd_SD_Command;
            message.Data = new List<byte>();
            message.Data.Add(SD_SubCmd_CreateDir);                                  // sub cmd
            message.Data.AddRange(((uint)timestamp).GetBytes().Reverse());          // timestamp
            message.Data.AddRange(name_encoding.GetBytes(path + "\0"));            // dir name

            sync_obj.Reset();
            bContinue = true;
            DevConfigService.Instance.InputPeriph?.SendMsg(message);
            if (sync_obj.WaitOne(3000))
            {
                if (MessageFlag == (byte)UpdateEnumFlags.RespOK)
                {
                    MainForm.AppendToDebug("Create Directory OK", true, false, Color.DarkGreen);
                    // OK vytvorime polozku ve stromu
                    DirInfo dirInfo1 = new DirInfo(path);
                    dirInfo1.FileInfoList = new List<FileInfo>();
                    TreeNode new_node = new TreeNode(Path.GetFileName(path));
                    new_node.ImageKey = "FolderClosed.bmp";
                    new_node.SelectedImageKey = "FolderClosed.bmp";
                    new_node.Tag = dirInfo1;
                    treeView1.SelectedNode.Nodes.Add(new_node);
                }
                else
                {
                    MainForm.AppendToDebug($"Create Directory ERROR ({(UpdateEnumFlags)MessageFlag})", true, true, Color.Red);
                }
            }
            else
            {
                if (bContinue)
                    MainForm.AppendToDebug("Create Directory TIMEOUT", true, true, Color.Red);
            }
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
                message.CMD = ECmd_SD_Command;
                message.Data = new List<byte>();
                message.Data.Add(SD_SubCmd_DelDir);
                message.Data.AddRange(name_encoding.GetBytes(path + "\0"));
                sync_obj.Reset();
                bContinue = true;
                DevConfigService.Instance.InputPeriph?.SendMsg(message);
                if (sync_obj.WaitOne(3000))
                {
                    if (MessageFlag == (byte)UpdateEnumFlags.RespOK)
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
        private void RenDirectory(string text)
        {
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
                msg_arr[0].CMD = ECmd_SD_Command;
                msg_arr[0].Data = new List<byte>() { SD_SubCmd_RenDirOld };
                msg_arr[0].Data.AddRange(name_encoding.GetBytes(path + "\0"));

                msg_arr[1] = new Message();
                msg_arr[1].DEST = DeviceAddress;
                msg_arr[1].CMD = ECmd_SD_Command;
                msg_arr[1].Data = new List<byte> { SD_SubCmd_RenDirNew };
                msg_arr[1].Data.AddRange(name_encoding.GetBytes(new_path + "\0"));

                bContinue = true;
                foreach (Message message in msg_arr)
                {
                    if (!bContinue)
                        break;

                    sync_obj.Reset();
                    DevConfigService.Instance.InputPeriph?.SendMsg(message);

                    if (sync_obj.WaitOne(1000))
                    {
                        if (MessageFlag != (byte)UpdateEnumFlags.RespOK)
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

                if (MessageFlag == (byte)UpdateEnumFlags.RespOK)
                {
                    MainForm.AppendToDebug("Rename Directory OK", true, false, Color.DarkGreen);
                    treeView1.SelectedNode.Text = Path.GetFileName(new_path);
                    ((DirInfo)treeView1.SelectedNode.Tag).Name = Path.GetFileName(new_path);
                }
            }
        }
        #endregion

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

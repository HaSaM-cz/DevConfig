namespace DevConfig
{
    partial class SDCardCtrl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            TreeNode treeNode2 = new TreeNode("SD Card");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SDCardCtrl));
            treeView1 = new TreeView();
            imageList = new ImageList(components);
            btn_List = new Button();
            listView1 = new ListViewEx();
            columnHeader1 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            contextMenuList = new ContextMenuStrip(components);
            GetFileMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            AddFileMenuItem = new ToolStripMenuItem();
            RenFileMenuItem = new ToolStripMenuItem();
            DelFileMenuItem = new ToolStripMenuItem();
            splitContainer1 = new SplitContainer();
            btn_AddFile = new Button();
            btn_GetFile = new Button();
            btn_DelFile = new Button();
            btn_AddDir = new Button();
            btn_DelDir = new Button();
            textBox1 = new TextBox();
            btn_RenDir = new Button();
            contextMenuTree = new ContextMenuStrip(components);
            addDirectoryToolStripMenuItem = new ToolStripMenuItem();
            renameDirectoryToolStripMenuItem = new ToolStripMenuItem();
            deleteDirectoryToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            refreshToolStripMenuItem = new ToolStripMenuItem();
            formatSDCardToolStripMenuItem = new ToolStripMenuItem();
            contextMenuList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            contextMenuTree.SuspendLayout();
            SuspendLayout();
            // 
            // treeView1
            // 
            treeView1.Dock = DockStyle.Fill;
            treeView1.HideSelection = false;
            treeView1.ImageIndex = 0;
            treeView1.ImageList = imageList;
            treeView1.Location = new Point(0, 0);
            treeView1.Name = "treeView1";
            treeNode2.ImageKey = "SDCard.png";
            treeNode2.Name = "SDCard";
            treeNode2.SelectedImageKey = "SDCard.png";
            treeNode2.Text = "SD Card";
            treeView1.Nodes.AddRange(new TreeNode[] { treeNode2 });
            treeView1.SelectedImageIndex = 0;
            treeView1.Size = new Size(359, 661);
            treeView1.TabIndex = 0;
            treeView1.AfterCollapse += treeView1_AfterCollapse;
            treeView1.AfterExpand += treeView1_AfterExpand;
            treeView1.AfterSelect += treeView1_AfterSelect;
            treeView1.KeyDown += treeView1_KeyDown;
            treeView1.MouseClick += treeView1_MouseClick;
            // 
            // imageList
            // 
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageStream = (ImageListStreamer)resources.GetObject("imageList.ImageStream");
            imageList.TransparentColor = Color.Teal;
            imageList.Images.SetKeyName(0, "FolderClosed.bmp");
            imageList.Images.SetKeyName(1, "FolderOpened.bmp");
            imageList.Images.SetKeyName(2, "SDCard.png");
            // 
            // btn_List
            // 
            btn_List.Location = new Point(12, 12);
            btn_List.Name = "btn_List";
            btn_List.Size = new Size(73, 29);
            btn_List.TabIndex = 1;
            btn_List.Text = "Reload";
            btn_List.UseVisualStyleBackColor = true;
            btn_List.Click += btn_List_Click;
            // 
            // listView1
            // 
            listView1.AllowColumnReorder = true;
            listView1.AllowDrop = true;
            listView1.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader3, columnHeader2 });
            listView1.ContextMenuStrip = contextMenuList;
            listView1.Dock = DockStyle.Fill;
            listView1.DoubleClickActivation = false;
            listView1.FullRowSelect = true;
            listView1.Location = new Point(0, 0);
            listView1.Name = "listView1";
            listView1.Size = new Size(826, 661);
            listView1.TabIndex = 2;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            listView1.SubItemClicked += listView1_SubItemClicked;
            listView1.SubItemEndEditing += listView1_SubItemEndEditing;
            listView1.ColumnClick += listView1_ColumnClick;
            listView1.ItemDrag += listView1_ItemDrag;
            listView1.DragDrop += listView1_DragDrop;
            listView1.DragEnter += listView1_DragEnter;
            listView1.DragOver += listView1_DragOver;
            listView1.DragLeave += listView1_DragLeave;
            listView1.GiveFeedback += listView1_GiveFeedback;
            listView1.KeyDown += listView1_KeyDown;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Name";
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Last Modified";
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Size";
            // 
            // contextMenuList
            // 
            contextMenuList.ImageScalingSize = new Size(20, 20);
            contextMenuList.Items.AddRange(new ToolStripItem[] { GetFileMenuItem, toolStripSeparator1, AddFileMenuItem, RenFileMenuItem, DelFileMenuItem });
            contextMenuList.Name = "contextMenuList";
            contextMenuList.Size = new Size(160, 106);
            // 
            // GetFileMenuItem
            // 
            GetFileMenuItem.Name = "GetFileMenuItem";
            GetFileMenuItem.Size = new Size(159, 24);
            GetFileMenuItem.Text = "Get File";
            GetFileMenuItem.Click += GetFileMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(156, 6);
            // 
            // AddFileMenuItem
            // 
            AddFileMenuItem.Name = "AddFileMenuItem";
            AddFileMenuItem.Size = new Size(159, 24);
            AddFileMenuItem.Text = "Add File";
            AddFileMenuItem.Click += AddFileMenuItem_Click;
            // 
            // RenFileMenuItem
            // 
            RenFileMenuItem.Name = "RenFileMenuItem";
            RenFileMenuItem.Size = new Size(159, 24);
            RenFileMenuItem.Text = "Rename File";
            RenFileMenuItem.Click += RenFileMenuItem_Click;
            // 
            // DelFileMenuItem
            // 
            DelFileMenuItem.Name = "DelFileMenuItem";
            DelFileMenuItem.Size = new Size(159, 24);
            DelFileMenuItem.Text = "Delete File";
            DelFileMenuItem.Click += DelFileMenuItem_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer1.Location = new Point(0, 47);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(treeView1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(listView1);
            splitContainer1.Size = new Size(1189, 661);
            splitContainer1.SplitterDistance = 359;
            splitContainer1.TabIndex = 3;
            // 
            // btn_AddFile
            // 
            btn_AddFile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_AddFile.Location = new Point(983, 12);
            btn_AddFile.Name = "btn_AddFile";
            btn_AddFile.Size = new Size(94, 29);
            btn_AddFile.TabIndex = 4;
            btn_AddFile.Text = "Add File";
            btn_AddFile.UseVisualStyleBackColor = true;
            btn_AddFile.Click += btn_Add_Click;
            // 
            // btn_GetFile
            // 
            btn_GetFile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_GetFile.Location = new Point(883, 12);
            btn_GetFile.Name = "btn_GetFile";
            btn_GetFile.Size = new Size(94, 29);
            btn_GetFile.TabIndex = 5;
            btn_GetFile.Text = "Get File";
            btn_GetFile.UseVisualStyleBackColor = true;
            btn_GetFile.Click += btn_Get_Click;
            // 
            // btn_DelFile
            // 
            btn_DelFile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_DelFile.Location = new Point(1083, 12);
            btn_DelFile.Name = "btn_DelFile";
            btn_DelFile.Size = new Size(94, 29);
            btn_DelFile.TabIndex = 6;
            btn_DelFile.Text = "Del File";
            btn_DelFile.UseVisualStyleBackColor = true;
            btn_DelFile.Click += btn_DelFile_Click;
            // 
            // btn_AddDir
            // 
            btn_AddDir.Location = new Point(91, 12);
            btn_AddDir.Name = "btn_AddDir";
            btn_AddDir.Size = new Size(73, 29);
            btn_AddDir.TabIndex = 7;
            btn_AddDir.Text = "Add Dir.";
            btn_AddDir.UseVisualStyleBackColor = true;
            btn_AddDir.Click += btn_AddDir_Click;
            // 
            // btn_DelDir
            // 
            btn_DelDir.Location = new Point(170, 12);
            btn_DelDir.Name = "btn_DelDir";
            btn_DelDir.Size = new Size(73, 29);
            btn_DelDir.TabIndex = 8;
            btn_DelDir.Text = "Del Dir.";
            btn_DelDir.UseVisualStyleBackColor = true;
            btn_DelDir.Click += btn_DelDir_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(370, 12);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(118, 27);
            textBox1.TabIndex = 9;
            textBox1.Text = "for list view edit";
            textBox1.Visible = false;
            // 
            // btn_RenDir
            // 
            btn_RenDir.Location = new Point(249, 12);
            btn_RenDir.Name = "btn_RenDir";
            btn_RenDir.Size = new Size(73, 29);
            btn_RenDir.TabIndex = 10;
            btn_RenDir.Text = "Ren. Dir.";
            btn_RenDir.UseVisualStyleBackColor = true;
            btn_RenDir.Click += btn_RenDir_Click;
            // 
            // contextMenuTree
            // 
            contextMenuTree.ImageScalingSize = new Size(20, 20);
            contextMenuTree.Items.AddRange(new ToolStripItem[] { addDirectoryToolStripMenuItem, renameDirectoryToolStripMenuItem, deleteDirectoryToolStripMenuItem, toolStripSeparator2, refreshToolStripMenuItem, formatSDCardToolStripMenuItem });
            contextMenuTree.Name = "contextMenuTree";
            contextMenuTree.Size = new Size(211, 158);
            // 
            // addDirectoryToolStripMenuItem
            // 
            addDirectoryToolStripMenuItem.Name = "addDirectoryToolStripMenuItem";
            addDirectoryToolStripMenuItem.Size = new Size(210, 24);
            addDirectoryToolStripMenuItem.Text = "Add Directory";
            addDirectoryToolStripMenuItem.Click += addDirectoryToolStripMenuItem_Click;
            // 
            // renameDirectoryToolStripMenuItem
            // 
            renameDirectoryToolStripMenuItem.Name = "renameDirectoryToolStripMenuItem";
            renameDirectoryToolStripMenuItem.Size = new Size(210, 24);
            renameDirectoryToolStripMenuItem.Text = "Rename Directory";
            renameDirectoryToolStripMenuItem.Click += renameDirectoryToolStripMenuItem_Click;
            // 
            // deleteDirectoryToolStripMenuItem
            // 
            deleteDirectoryToolStripMenuItem.Name = "deleteDirectoryToolStripMenuItem";
            deleteDirectoryToolStripMenuItem.Size = new Size(210, 24);
            deleteDirectoryToolStripMenuItem.Text = "Delete Directory";
            deleteDirectoryToolStripMenuItem.Click += deleteDirectoryToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(194, 6);
            // 
            // refreshToolStripMenuItem
            // 
            refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            refreshToolStripMenuItem.Size = new Size(210, 24);
            refreshToolStripMenuItem.Text = "Refresh";
            refreshToolStripMenuItem.Click += refreshToolStripMenuItem_Click;
            // 
            // formatSDCardToolStripMenuItem
            // 
            formatSDCardToolStripMenuItem.Name = "formatSDCardToolStripMenuItem";
            formatSDCardToolStripMenuItem.Size = new Size(210, 24);
            formatSDCardToolStripMenuItem.Text = "Format SD card";
            formatSDCardToolStripMenuItem.Click += formatSDCardToolStripMenuItem_Click;
            // 
            // SDCardCtrl
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1189, 708);
            Controls.Add(btn_RenDir);
            Controls.Add(textBox1);
            Controls.Add(btn_DelDir);
            Controls.Add(btn_AddDir);
            Controls.Add(btn_DelFile);
            Controls.Add(btn_GetFile);
            Controls.Add(btn_AddFile);
            Controls.Add(splitContainer1);
            Controls.Add(btn_List);
            Name = "SDCardCtrl";
            Text = "SDCardCtrl";
            Load += SDCardCtrl_Load;
            contextMenuList.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            contextMenuTree.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TreeView treeView1;
        private Button btn_List;
        private ListViewEx listView1;
        private SplitContainer splitContainer1;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader3;
        private Button btn_AddFile;
        private Button btn_GetFile;
        private Button btn_DelFile;
        private ImageList imageList;
        private Button btn_AddDir;
        private ColumnHeader columnHeader2;
        private Button btn_DelDir;
        private TextBox textBox1;
        private Button btn_RenDir;
        private ContextMenuStrip contextMenuTree;
        private ContextMenuStrip contextMenuList;
        private ToolStripMenuItem GetFileMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem AddFileMenuItem;
        private ToolStripMenuItem RenFileMenuItem;
        private ToolStripMenuItem DelFileMenuItem;
        private ToolStripMenuItem addDirectoryToolStripMenuItem;
        private ToolStripMenuItem renameDirectoryToolStripMenuItem;
        private ToolStripMenuItem deleteDirectoryToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem refreshToolStripMenuItem;
        private ToolStripMenuItem formatSDCardToolStripMenuItem;
    }
}
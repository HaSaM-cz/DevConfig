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
            TreeNode treeNode1 = new TreeNode("SD Card");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SDCardCtrl));
            treeView1 = new TreeView();
            imageList = new ImageList(components);
            btn_List = new Button();
            listView1 = new ListViewEx();
            columnHeader1 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            splitContainer1 = new SplitContainer();
            btn_AddFile = new Button();
            btn_GetFile = new Button();
            btn_DelFile = new Button();
            btn_AddDir = new Button();
            btn_DelDir = new Button();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
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
            treeNode1.ImageKey = "SDCard.png";
            treeNode1.Name = "SDCard";
            treeNode1.SelectedImageKey = "SDCard.png";
            treeNode1.Text = "SD Card";
            treeView1.Nodes.AddRange(new TreeNode[] { treeNode1 });
            treeView1.SelectedImageIndex = 0;
            treeView1.Size = new Size(242, 403);
            treeView1.TabIndex = 0;
            treeView1.AfterCollapse += treeView1_AfterCollapse;
            treeView1.AfterExpand += treeView1_AfterExpand;
            treeView1.AfterSelect += treeView1_AfterSelect;
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
            btn_List.Size = new Size(94, 29);
            btn_List.TabIndex = 1;
            btn_List.Text = "List";
            btn_List.UseVisualStyleBackColor = true;
            btn_List.Click += btn_List_Click;
            // 
            // listView1
            // 
            listView1.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader3, columnHeader2 });
            listView1.Dock = DockStyle.Fill;
            listView1.Location = new Point(0, 0);
            listView1.Name = "listView1";
            listView1.Size = new Size(554, 403);
            listView1.TabIndex = 2;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            listView1.ColumnClick += listView1_ColumnClick;
            listView1.ItemDrag += listView1_ItemDrag;
            listView1.DragDrop += listView1_DragDrop;
            listView1.DragEnter += listView1_DragEnter;
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
            splitContainer1.Size = new Size(800, 403);
            splitContainer1.SplitterDistance = 242;
            splitContainer1.TabIndex = 3;
            // 
            // btn_AddFile
            // 
            btn_AddFile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_AddFile.Location = new Point(494, 12);
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
            btn_GetFile.Location = new Point(594, 12);
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
            btn_DelFile.Location = new Point(694, 12);
            btn_DelFile.Name = "btn_DelFile";
            btn_DelFile.Size = new Size(94, 29);
            btn_DelFile.TabIndex = 6;
            btn_DelFile.Text = "Del File";
            btn_DelFile.UseVisualStyleBackColor = true;
            btn_DelFile.Click += btn_DelFile_Click;
            // 
            // btn_AddDir
            // 
            btn_AddDir.Location = new Point(112, 12);
            btn_AddDir.Name = "btn_AddDir";
            btn_AddDir.Size = new Size(94, 29);
            btn_AddDir.TabIndex = 7;
            btn_AddDir.Text = "Add Dir.";
            btn_AddDir.UseVisualStyleBackColor = true;
            // 
            // btn_DelDir
            // 
            btn_DelDir.Location = new Point(212, 12);
            btn_DelDir.Name = "btn_DelDir";
            btn_DelDir.Size = new Size(94, 29);
            btn_DelDir.TabIndex = 8;
            btn_DelDir.Text = "Del Dir.";
            btn_DelDir.UseVisualStyleBackColor = true;
            // 
            // SDCardCtrl
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
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
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
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
    }
}
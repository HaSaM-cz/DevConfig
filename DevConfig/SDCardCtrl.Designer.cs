﻿using DevConfig.Controls.ListViewExCtrl;

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
            textBox1 = new TextBox();
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
            treeNode1.ImageKey = "SDCard.png";
            treeNode1.Name = "SDCard";
            treeNode1.SelectedImageKey = "SDCard.png";
            treeNode1.Text = "SD Card";
            treeView1.Nodes.AddRange(new TreeNode[] { treeNode1 });
            treeView1.SelectedImageIndex = 0;
            treeView1.Size = new Size(356, 708);
            treeView1.TabIndex = 0;
            treeView1.AfterLabelEdit += treeView1_AfterLabelEdit;
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
            listView1.Size = new Size(829, 708);
            listView1.TabIndex = 2;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            listView1.SubItemClicked += listView1_SubItemClicked;
            listView1.SubItemEndEditing += listView1_SubItemEndEditing;
            listView1.ColumnClick += listView1_ColumnClick;
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
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(treeView1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(listView1);
            splitContainer1.Size = new Size(1189, 708);
            splitContainer1.SplitterDistance = 356;
            splitContainer1.TabIndex = 3;
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
            // contextMenuTree
            // 
            contextMenuTree.ImageScalingSize = new Size(20, 20);
            contextMenuTree.Items.AddRange(new ToolStripItem[] { addDirectoryToolStripMenuItem, renameDirectoryToolStripMenuItem, deleteDirectoryToolStripMenuItem, toolStripSeparator2, refreshToolStripMenuItem, formatSDCardToolStripMenuItem });
            contextMenuTree.Name = "contextMenuTree";
            contextMenuTree.Size = new Size(198, 130);
            // 
            // addDirectoryToolStripMenuItem
            // 
            addDirectoryToolStripMenuItem.Name = "addDirectoryToolStripMenuItem";
            addDirectoryToolStripMenuItem.Size = new Size(197, 24);
            addDirectoryToolStripMenuItem.Text = "Add Directory";
            addDirectoryToolStripMenuItem.Click += AddDirMenuItem_Click;
            // 
            // renameDirectoryToolStripMenuItem
            // 
            renameDirectoryToolStripMenuItem.Name = "renameDirectoryToolStripMenuItem";
            renameDirectoryToolStripMenuItem.Size = new Size(197, 24);
            renameDirectoryToolStripMenuItem.Text = "Rename Directory";
            renameDirectoryToolStripMenuItem.Click += RenDirMenuItem_Click;
            // 
            // deleteDirectoryToolStripMenuItem
            // 
            deleteDirectoryToolStripMenuItem.Name = "deleteDirectoryToolStripMenuItem";
            deleteDirectoryToolStripMenuItem.Size = new Size(197, 24);
            deleteDirectoryToolStripMenuItem.Text = "Delete Directory";
            deleteDirectoryToolStripMenuItem.Click += DelDirMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(194, 6);
            // 
            // refreshToolStripMenuItem
            // 
            refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            refreshToolStripMenuItem.Size = new Size(197, 24);
            refreshToolStripMenuItem.Text = "Refresh";
            refreshToolStripMenuItem.Click += ReloadSdCardMenuItem_Click;
            // 
            // formatSDCardToolStripMenuItem
            // 
            formatSDCardToolStripMenuItem.Name = "formatSDCardToolStripMenuItem";
            formatSDCardToolStripMenuItem.Size = new Size(197, 24);
            formatSDCardToolStripMenuItem.Text = "Format SD card";
            formatSDCardToolStripMenuItem.Click += FormatSdCardMenuItem_Click;
            // 
            // SDCardCtrl
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1189, 708);
            Controls.Add(textBox1);
            Controls.Add(splitContainer1);
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

        public TreeView treeView1;
        private ListViewEx listView1;
        private SplitContainer splitContainer1;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader3;
        private ImageList imageList;
        private ColumnHeader columnHeader2;
        private TextBox textBox1;
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
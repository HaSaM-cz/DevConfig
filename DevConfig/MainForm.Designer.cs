using System.Windows.Forms;

namespace DevConfig
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            splitContainer1 = new SplitContainer();
            listViewDevices = new ListView();
            colAddress = new ColumnHeader();
            colDevId = new ColumnHeader();
            colName = new ColumnHeader();
            colFwVer = new ColumnHeader();
            colCpuId = new ColumnHeader();
            tabControl = new TabControl();
            tabSwUpdate = new TabPage();
            tb_cpu_id = new TextBox();
            tb_version = new TextBox();
            tb_dev_id = new TextBox();
            tb_address = new TextBox();
            label_name = new Label();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            btnBrowse = new Button();
            btnUpdate = new Button();
            tbFwFileName = new TextBox();
            btnIdent = new Button();
            tabRegisters = new TabPage();
            menuStrip1 = new MenuStrip();
            connectionToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            closeToolStripMenuItem = new ToolStripMenuItem();
            controlToolStripMenuItem = new ToolStripMenuItem();
            refreshListToolStripMenuItem = new ToolStripMenuItem();
            progressBar = new ProgressBar();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tabControl.SuspendLayout();
            tabSwUpdate.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 28);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(listViewDevices);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(tabControl);
            splitContainer1.Size = new Size(1290, 614);
            splitContainer1.SplitterDistance = 636;
            splitContainer1.TabIndex = 0;
            // 
            // listViewDevices
            // 
            listViewDevices.Columns.AddRange(new ColumnHeader[] { colAddress, colDevId, colName, colFwVer, colCpuId });
            listViewDevices.Dock = DockStyle.Fill;
            listViewDevices.FullRowSelect = true;
            listViewDevices.GridLines = true;
            listViewDevices.Location = new Point(0, 0);
            listViewDevices.MultiSelect = false;
            listViewDevices.Name = "listViewDevices";
            listViewDevices.Size = new Size(636, 614);
            listViewDevices.TabIndex = 0;
            listViewDevices.UseCompatibleStateImageBehavior = false;
            listViewDevices.View = View.Details;
            listViewDevices.ColumnWidthChanged += listViewDevices_ColumnWidthChanged;
            listViewDevices.SelectedIndexChanged += listViewDevices_SelectedIndexChanged;
            listViewDevices.Resize += listViewDevices_Resize;
            // 
            // colAddress
            // 
            colAddress.Tag = "";
            colAddress.Text = "Addr.";
            // 
            // colDevId
            // 
            colDevId.Text = "Dev. ID";
            // 
            // colName
            // 
            colName.Text = "Name";
            colName.Width = 150;
            // 
            // colFwVer
            // 
            colFwVer.Text = "Ver.";
            // 
            // colCpuId
            // 
            colCpuId.Text = "CPU ID";
            colCpuId.Width = 100;
            // 
            // tabControl
            // 
            tabControl.Controls.Add(tabSwUpdate);
            tabControl.Controls.Add(tabRegisters);
            tabControl.Dock = DockStyle.Fill;
            tabControl.Location = new Point(0, 0);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(650, 614);
            tabControl.TabIndex = 0;
            // 
            // tabSwUpdate
            // 
            tabSwUpdate.Controls.Add(tb_cpu_id);
            tabSwUpdate.Controls.Add(tb_version);
            tabSwUpdate.Controls.Add(tb_dev_id);
            tabSwUpdate.Controls.Add(tb_address);
            tabSwUpdate.Controls.Add(label_name);
            tabSwUpdate.Controls.Add(label5);
            tabSwUpdate.Controls.Add(label4);
            tabSwUpdate.Controls.Add(label3);
            tabSwUpdate.Controls.Add(label2);
            tabSwUpdate.Controls.Add(label1);
            tabSwUpdate.Controls.Add(btnBrowse);
            tabSwUpdate.Controls.Add(btnUpdate);
            tabSwUpdate.Controls.Add(tbFwFileName);
            tabSwUpdate.Controls.Add(btnIdent);
            tabSwUpdate.Location = new Point(4, 29);
            tabSwUpdate.Name = "tabSwUpdate";
            tabSwUpdate.Padding = new Padding(3);
            tabSwUpdate.Size = new Size(642, 581);
            tabSwUpdate.TabIndex = 0;
            tabSwUpdate.Text = "SW Update";
            tabSwUpdate.UseVisualStyleBackColor = true;
            // 
            // tb_cpu_id
            // 
            tb_cpu_id.Location = new Point(254, 84);
            tb_cpu_id.Name = "tb_cpu_id";
            tb_cpu_id.ReadOnly = true;
            tb_cpu_id.Size = new Size(268, 27);
            tb_cpu_id.TabIndex = 13;
            // 
            // tb_version
            // 
            tb_version.Location = new Point(166, 84);
            tb_version.Name = "tb_version";
            tb_version.ReadOnly = true;
            tb_version.Size = new Size(82, 27);
            tb_version.TabIndex = 12;
            // 
            // tb_dev_id
            // 
            tb_dev_id.Location = new Point(87, 84);
            tb_dev_id.Name = "tb_dev_id";
            tb_dev_id.ReadOnly = true;
            tb_dev_id.Size = new Size(73, 27);
            tb_dev_id.TabIndex = 11;
            // 
            // tb_address
            // 
            tb_address.Location = new Point(19, 84);
            tb_address.Name = "tb_address";
            tb_address.ReadOnly = true;
            tb_address.Size = new Size(62, 27);
            tb_address.TabIndex = 10;
            // 
            // label_name
            // 
            label_name.AutoSize = true;
            label_name.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label_name.Location = new Point(19, 24);
            label_name.Name = "label_name";
            label_name.Size = new Size(66, 28);
            label_name.TabIndex = 9;
            label_name.Text = "Name";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(254, 61);
            label5.Name = "label5";
            label5.Size = new Size(55, 20);
            label5.TabIndex = 8;
            label5.Text = "CPU ID";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(87, 61);
            label4.Name = "label4";
            label4.Size = new Size(73, 20);
            label4.TabIndex = 7;
            label4.Text = "Device ID";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(166, 61);
            label3.Name = "label3";
            label3.Size = new Size(82, 20);
            label3.TabIndex = 6;
            label3.Text = "Fw. Version";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(19, 61);
            label2.Name = "label2";
            label2.Size = new Size(62, 20);
            label2.TabIndex = 5;
            label2.Text = "Address";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(19, 121);
            label1.Name = "label1";
            label1.Size = new Size(70, 20);
            label1.TabIndex = 4;
            label1.Text = "Firmware";
            // 
            // btnBrowse
            // 
            btnBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowse.Location = new Point(494, 143);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(29, 29);
            btnBrowse.TabIndex = 3;
            btnBrowse.Text = "...";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // btnUpdate
            // 
            btnUpdate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnUpdate.Location = new Point(529, 143);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(94, 29);
            btnUpdate.TabIndex = 2;
            btnUpdate.Text = "Update";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Click += btnUpdate_Click;
            // 
            // tbFwFileName
            // 
            tbFwFileName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbFwFileName.Location = new Point(19, 144);
            tbFwFileName.Name = "tbFwFileName";
            tbFwFileName.Size = new Size(469, 27);
            tbFwFileName.TabIndex = 1;
            // 
            // btnIdent
            // 
            btnIdent.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnIdent.Location = new Point(529, 84);
            btnIdent.Name = "btnIdent";
            btnIdent.Size = new Size(94, 29);
            btnIdent.TabIndex = 0;
            btnIdent.Text = "Ident";
            btnIdent.UseVisualStyleBackColor = true;
            btnIdent.Click += btnIdent_Click;
            // 
            // tabRegisters
            // 
            tabRegisters.Location = new Point(4, 29);
            tabRegisters.Name = "tabRegisters";
            tabRegisters.Padding = new Padding(3);
            tabRegisters.Size = new Size(615, 581);
            tabRegisters.TabIndex = 1;
            tabRegisters.Text = "Registers";
            tabRegisters.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { connectionToolStripMenuItem, controlToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1290, 28);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // connectionToolStripMenuItem
            // 
            connectionToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openToolStripMenuItem, closeToolStripMenuItem });
            connectionToolStripMenuItem.Name = "connectionToolStripMenuItem";
            connectionToolStripMenuItem.Size = new Size(98, 24);
            connectionToolStripMenuItem.Text = "Connection";
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(128, 26);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += Open_Click;
            // 
            // closeToolStripMenuItem
            // 
            closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            closeToolStripMenuItem.Size = new Size(128, 26);
            closeToolStripMenuItem.Text = "Close";
            closeToolStripMenuItem.Click += Close_Click;
            // 
            // controlToolStripMenuItem
            // 
            controlToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { refreshListToolStripMenuItem });
            controlToolStripMenuItem.Name = "controlToolStripMenuItem";
            controlToolStripMenuItem.Size = new Size(72, 24);
            controlToolStripMenuItem.Text = "Control";
            // 
            // refreshListToolStripMenuItem
            // 
            refreshListToolStripMenuItem.Name = "refreshListToolStripMenuItem";
            refreshListToolStripMenuItem.Size = new Size(164, 26);
            refreshListToolStripMenuItem.Text = "Refresh list";
            refreshListToolStripMenuItem.Click += RefreshList_Click;
            // 
            // progressBar
            // 
            progressBar.Dock = DockStyle.Bottom;
            progressBar.Location = new Point(0, 642);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(1290, 14);
            progressBar.Step = 1;
            progressBar.TabIndex = 1;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1290, 656);
            Controls.Add(splitContainer1);
            Controls.Add(menuStrip1);
            Controls.Add(progressBar);
            Name = "MainForm";
            Text = "Device Config";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tabControl.ResumeLayout(false);
            tabSwUpdate.ResumeLayout(false);
            tabSwUpdate.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private SplitContainer splitContainer1;
        private TabControl tabControl;
        private TabPage tabSwUpdate;
        private TabPage tabRegisters;
        private ListView listViewDevices;
        private ColumnHeader colName;
        private ColumnHeader colDevId;
        private ColumnHeader colFwVer;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem connectionToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem closeToolStripMenuItem;
        private ColumnHeader colCpuId;
        private ToolStripMenuItem controlToolStripMenuItem;
        private ToolStripMenuItem refreshListToolStripMenuItem;
        private ColumnHeader colAddress;
        private ToolStrip toolStrip1;
        private ToolStrip toolStrip2;
        private ProgressBar progressBar;
        private TextBox tbFwFileName;
        private Button btnIdent;
        private Label label1;
        private Button btnBrowse;
        private Button btnUpdate;
        private Label label2;
        private Label label_name;
        private Label label5;
        private Label label4;
        private Label label3;
        private TextBox tb_cpu_id;
        private TextBox tb_version;
        private TextBox tb_dev_id;
        private TextBox tb_address;
    }
}

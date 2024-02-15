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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            vS2015LightTheme1 = new WeifenLuo.WinFormsUI.Docking.VS2015LightTheme();
            menuStrip1 = new MenuStrip();
            connectionToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            closeToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            deviceTreeToolStripMenuItem = new ToolStripMenuItem();
            debugToolStripMenuItem = new ToolStripMenuItem();
            sDCardForSelectedDeviceToolStripMenuItem = new ToolStripMenuItem();
            deviceToolStripMenuItem = new ToolStripMenuItem();
            controlToolStripMenuItem = new ToolStripMenuItem();
            refreshListToolStripMenuItem = new ToolStripMenuItem();
            toolStrip1 = new ToolStrip();
            label_name = new ToolStripLabel();
            toolStripStatusLabel9 = new ToolStripStatusLabel();
            tb_address = new ToolStripStatusLabel();
            toolStripStatusLabel11 = new ToolStripStatusLabel();
            tb_dev_id = new ToolStripStatusLabel();
            toolStripStatusLabel13 = new ToolStripStatusLabel();
            tb_version = new ToolStripStatusLabel();
            btnAbort = new ToolStripButton();
            tsProgressBar = new ToolStripProgressBar();
            toolStripSeparator1 = new ToolStripSeparator();
            menuStrip1.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // dockPanel
            // 
            dockPanel.Dock = DockStyle.Fill;
            dockPanel.DockBackColor = Color.FromArgb(238, 238, 242);
            dockPanel.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingMdi;
            dockPanel.Location = new Point(0, 28);
            dockPanel.Name = "dockPanel";
            dockPanel.Padding = new Padding(6);
            dockPanel.ShowAutoHideContentOnHover = false;
            dockPanel.Size = new Size(1119, 828);
            dockPanel.TabIndex = 2;
            dockPanel.Theme = vS2015LightTheme1;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { connectionToolStripMenuItem, viewToolStripMenuItem, controlToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1119, 28);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // connectionToolStripMenuItem
            // 
            connectionToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openToolStripMenuItem, closeToolStripMenuItem, toolStripSeparator1, exitToolStripMenuItem });
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
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(128, 26);
            exitToolStripMenuItem.Text = "Exit";
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { deviceTreeToolStripMenuItem, debugToolStripMenuItem, sDCardForSelectedDeviceToolStripMenuItem, deviceToolStripMenuItem });
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new Size(55, 24);
            viewToolStripMenuItem.Text = "View";
            // 
            // deviceTreeToolStripMenuItem
            // 
            deviceTreeToolStripMenuItem.Name = "deviceTreeToolStripMenuItem";
            deviceTreeToolStripMenuItem.Size = new Size(275, 26);
            deviceTreeToolStripMenuItem.Text = "Device Tree";
            // 
            // debugToolStripMenuItem
            // 
            debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            debugToolStripMenuItem.Size = new Size(275, 26);
            debugToolStripMenuItem.Text = "Debug";
            // 
            // sDCardForSelectedDeviceToolStripMenuItem
            // 
            sDCardForSelectedDeviceToolStripMenuItem.Name = "sDCardForSelectedDeviceToolStripMenuItem";
            sDCardForSelectedDeviceToolStripMenuItem.Size = new Size(275, 26);
            sDCardForSelectedDeviceToolStripMenuItem.Text = "SD Card for selected device";
            sDCardForSelectedDeviceToolStripMenuItem.Click += sDCardForSelectedDeviceToolStripMenuItem_Click;
            // 
            // deviceToolStripMenuItem
            // 
            deviceToolStripMenuItem.Name = "deviceToolStripMenuItem";
            deviceToolStripMenuItem.Size = new Size(275, 26);
            deviceToolStripMenuItem.Text = "Device";
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
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new Size(20, 20);
            toolStrip1.Items.AddRange(new ToolStripItem[] { label_name, toolStripStatusLabel9, tb_address, toolStripStatusLabel11, tb_dev_id, toolStripStatusLabel13, tb_version, btnAbort, tsProgressBar });
            toolStrip1.Location = new Point(0, 28);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(1119, 32);
            toolStrip1.TabIndex = 8;
            toolStrip1.Text = "Name";
            // 
            // label_name
            // 
            label_name.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label_name.Margin = new Padding(0, 2, 10, 2);
            label_name.Name = "label_name";
            label_name.Size = new Size(66, 28);
            label_name.Text = "Name";
            // 
            // toolStripStatusLabel9
            // 
            toolStripStatusLabel9.Margin = new Padding(0, 4, 0, 0);
            toolStripStatusLabel9.Name = "toolStripStatusLabel9";
            toolStripStatusLabel9.Size = new Size(62, 28);
            toolStripStatusLabel9.Text = "Address";
            // 
            // tb_address
            // 
            tb_address.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            tb_address.Margin = new Padding(0, 2, 10, 2);
            tb_address.Name = "tb_address";
            tb_address.Size = new Size(54, 28);
            tb_address.Text = "0x00";
            // 
            // toolStripStatusLabel11
            // 
            toolStripStatusLabel11.Margin = new Padding(0, 4, 0, 0);
            toolStripStatusLabel11.Name = "toolStripStatusLabel11";
            toolStripStatusLabel11.Size = new Size(73, 28);
            toolStripStatusLabel11.Text = "Device ID";
            // 
            // tb_dev_id
            // 
            tb_dev_id.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            tb_dev_id.Margin = new Padding(0, 2, 10, 2);
            tb_dev_id.Name = "tb_dev_id";
            tb_dev_id.Size = new Size(76, 28);
            tb_dev_id.Text = "0x0000";
            // 
            // toolStripStatusLabel13
            // 
            toolStripStatusLabel13.Margin = new Padding(0, 4, 0, 0);
            toolStripStatusLabel13.Name = "toolStripStatusLabel13";
            toolStripStatusLabel13.Size = new Size(82, 28);
            toolStripStatusLabel13.Text = "Fw. Version";
            // 
            // tb_version
            // 
            tb_version.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            tb_version.Margin = new Padding(0, 2, 10, 2);
            tb_version.Name = "tb_version";
            tb_version.Size = new Size(38, 28);
            tb_version.Text = "0.0";
            // 
            // btnAbort
            // 
            btnAbort.Alignment = ToolStripItemAlignment.Right;
            btnAbort.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnAbort.Image = (Image)resources.GetObject("btnAbort.Image");
            btnAbort.ImageTransparentColor = Color.Magenta;
            btnAbort.Margin = new Padding(0, 4, 4, 2);
            btnAbort.Name = "btnAbort";
            btnAbort.Size = new Size(51, 26);
            btnAbort.Text = "Abort";
            btnAbort.Click += btnAbort_Click;
            // 
            // tsProgressBar
            // 
            tsProgressBar.Alignment = ToolStripItemAlignment.Right;
            tsProgressBar.Margin = new Padding(1, 10, 3, 5);
            tsProgressBar.Name = "tsProgressBar";
            tsProgressBar.Size = new Size(300, 17);
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(125, 6);
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1119, 856);
            Controls.Add(toolStrip1);
            Controls.Add(dockPanel);
            Controls.Add(menuStrip1);
            DoubleBuffered = true;
            IsMdiContainer = true;
            Name = "MainForm";
            Text = "Device Config";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private MenuStrip menuStrip1;
        private ToolStripMenuItem connectionToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem closeToolStripMenuItem;
        private ToolStripMenuItem controlToolStripMenuItem;
        private ToolStripMenuItem refreshListToolStripMenuItem;
        private ToolStrip toolStrip1;
        private ToolStrip toolStrip2;
        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel;
        private WeifenLuo.WinFormsUI.Docking.VS2015LightTheme vS2015LightTheme1;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem deviceTreeToolStripMenuItem;
        private ToolStripStatusLabel toolStripStatusLabel9;
        private ToolStripStatusLabel toolStripStatusLabel10;
        private ToolStripStatusLabel toolStripStatusLabel11;
        private ToolStripStatusLabel toolStripStatusLabel12;
        private ToolStripStatusLabel toolStripStatusLabel13;
        private ToolStripStatusLabel toolStripStatusLabel14;
        private ToolStripSplitButton toolStripSplitButton1;
        private ToolStripDropDownButton toolStripDropDownButton1;
        public ToolStripStatusLabel tb_version;
        public ToolStripStatusLabel tb_dev_id;
        public ToolStripStatusLabel tb_address;
        public ToolStripLabel label_name;
        private ToolStripMenuItem debugToolStripMenuItem;
        private ToolStripMenuItem sDCardForSelectedDeviceToolStripMenuItem;
        private ToolStripMenuItem deviceToolStripMenuItem;
        private ToolStripButton btnAbort;
        private ToolStripProgressBar tsProgressBar;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
    }
}

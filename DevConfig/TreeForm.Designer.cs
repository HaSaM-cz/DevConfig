namespace DevConfig
{
    partial class TreeForm
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
            listViewDevices = new ListView();
            colAddress = new ColumnHeader();
            colDevId = new ColumnHeader();
            colName = new ColumnHeader();
            colFwVer = new ColumnHeader();
            colCpuId = new ColumnHeader();
            SuspendLayout();
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
            listViewDevices.Size = new Size(800, 450);
            listViewDevices.TabIndex = 1;
            listViewDevices.UseCompatibleStateImageBehavior = false;
            listViewDevices.View = View.Details;
            listViewDevices.SelectedIndexChanged += listViewDevices_SelectedIndexChanged;
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
            // TreeForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(listViewDevices);
            Name = "TreeForm";
            Text = "TreeForm";
            ResumeLayout(false);
        }

        #endregion
        private ColumnHeader colAddress;
        private ColumnHeader colDevId;
        private ColumnHeader colName;
        private ColumnHeader colFwVer;
        private ColumnHeader colCpuId;
        public ListView listViewDevices;
    }
}
namespace DevConfig
{
    partial class DeviceForm
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
            tbFwFileName = new TextBox();
            tb_dev_id = new TextBox();
            tb_cpu_id = new TextBox();
            tb_version = new TextBox();
            tb_address = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label_name = new Label();
            btnBrowse = new Button();
            btnUpdate = new Button();
            btnIdent = new Button();
            btnReset = new Button();
            SuspendLayout();
            // 
            // tbFwFileName
            // 
            tbFwFileName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbFwFileName.Location = new Point(19, 144);
            tbFwFileName.Name = "tbFwFileName";
            tbFwFileName.Size = new Size(469, 27);
            tbFwFileName.TabIndex = 1;
            // 
            // tb_dev_id
            // 
            tb_dev_id.Location = new Point(87, 84);
            tb_dev_id.Name = "tb_dev_id";
            tb_dev_id.ReadOnly = true;
            tb_dev_id.Size = new Size(73, 27);
            tb_dev_id.TabIndex = 11;
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
            // tb_address
            // 
            tb_address.Location = new Point(19, 84);
            tb_address.Name = "tb_address";
            tb_address.ReadOnly = true;
            tb_address.Size = new Size(62, 27);
            tb_address.TabIndex = 10;
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
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(19, 61);
            label2.Name = "label2";
            label2.Size = new Size(62, 20);
            label2.TabIndex = 5;
            label2.Text = "Address";
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
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(87, 61);
            label4.Name = "label4";
            label4.Size = new Size(73, 20);
            label4.TabIndex = 7;
            label4.Text = "Device ID";
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
            // btnReset
            // 
            btnReset.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnReset.Location = new Point(529, 202);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(94, 29);
            btnReset.TabIndex = 14;
            btnReset.Text = "Reset";
            btnReset.UseVisualStyleBackColor = true;
            btnReset.Click += btnReset_Click;
            // 
            // DeviceForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(632, 567);
            Controls.Add(btnReset);
            Controls.Add(btnIdent);
            Controls.Add(btnUpdate);
            Controls.Add(btnBrowse);
            Controls.Add(label_name);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(tb_address);
            Controls.Add(tb_version);
            Controls.Add(tb_cpu_id);
            Controls.Add(tb_dev_id);
            Controls.Add(tbFwFileName);
            Name = "DeviceForm";
            Text = "DeviceForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox tbFwFileName;
        private TextBox tb_dev_id;
        private TextBox tb_cpu_id;
        private TextBox tb_version;
        private TextBox tb_address;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label_name;
        private Button btnBrowse;
        private Button btnUpdate;
        private Button btnIdent;
        private Button btnReset;
    }
}
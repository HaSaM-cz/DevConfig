namespace DevConfig
{
    partial class BackupForm
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
            radioButton1 = new RadioButton();
            radioButton2 = new RadioButton();
            BackupDestination = new TextBox();
            label1 = new Label();
            button1 = new Button();
            tbExclude = new TextBox();
            button_Start = new Button();
            button_Cancel = new Button();
            label2 = new Label();
            groupBox1 = new GroupBox();
            label_Time = new Label();
            label_Bytes = new Label();
            label_Files = new Label();
            label_Directorys = new Label();
            label6 = new Label();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            btnRefresh = new Button();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Checked = true;
            radioButton1.Location = new Point(93, 7);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(91, 24);
            radioButton1.TabIndex = 0;
            radioButton1.TabStop = true;
            radioButton1.Text = "Directory";
            radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Enabled = false;
            radioButton2.Location = new Point(190, 7);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(75, 24);
            radioButton2.TabIndex = 1;
            radioButton2.Text = "ZipFile";
            radioButton2.UseVisualStyleBackColor = true;
            // 
            // BackupDestination
            // 
            BackupDestination.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            BackupDestination.Location = new Point(12, 35);
            BackupDestination.Name = "BackupDestination";
            BackupDestination.Size = new Size(536, 27);
            BackupDestination.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 76);
            label1.Name = "label1";
            label1.Size = new Size(450, 20);
            label1.TabIndex = 3;
            label1.Text = "Exclude Files (semicolon-separated list of excluded file extensions)";
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button1.Location = new Point(557, 34);
            button1.Name = "button1";
            button1.Size = new Size(32, 29);
            button1.TabIndex = 3;
            button1.Text = "...";
            button1.UseVisualStyleBackColor = true;
            button1.Click += Browse_Folder_Click;
            // 
            // tbExclude
            // 
            tbExclude.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbExclude.Location = new Point(12, 99);
            tbExclude.Name = "tbExclude";
            tbExclude.Size = new Size(577, 27);
            tbExclude.TabIndex = 5;
            tbExclude.Text = "*.bin";
            // 
            // button_Start
            // 
            button_Start.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_Start.DialogResult = DialogResult.OK;
            button_Start.Location = new Point(495, 212);
            button_Start.Name = "button_Start";
            button_Start.Size = new Size(94, 29);
            button_Start.TabIndex = 6;
            button_Start.Text = "Start";
            button_Start.UseVisualStyleBackColor = true;
            button_Start.Click += button_Start_Click;
            // 
            // button_Cancel
            // 
            button_Cancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_Cancel.DialogResult = DialogResult.Cancel;
            button_Cancel.Location = new Point(495, 177);
            button_Cancel.Name = "button_Cancel";
            button_Cancel.Size = new Size(94, 29);
            button_Cancel.TabIndex = 7;
            button_Cancel.Text = "Cancel";
            button_Cancel.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 9);
            label2.Name = "label2";
            label2.Size = new Size(75, 20);
            label2.TabIndex = 8;
            label2.Text = "Backup to";
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(label_Time);
            groupBox1.Controls.Add(label_Bytes);
            groupBox1.Controls.Add(label_Files);
            groupBox1.Controls.Add(label_Directorys);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label3);
            groupBox1.Location = new Point(12, 132);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(477, 109);
            groupBox1.TabIndex = 9;
            groupBox1.TabStop = false;
            groupBox1.Text = "Summary";
            // 
            // label_Time
            // 
            label_Time.AutoSize = true;
            label_Time.Location = new Point(143, 83);
            label_Time.Name = "label_Time";
            label_Time.Size = new Size(49, 20);
            label_Time.TabIndex = 8;
            label_Time.Text = "0 min.";
            // 
            // label_Bytes
            // 
            label_Bytes.AutoSize = true;
            label_Bytes.Location = new Point(143, 63);
            label_Bytes.Name = "label_Bytes";
            label_Bytes.Size = new Size(33, 20);
            label_Bytes.TabIndex = 7;
            label_Bytes.Text = "0 b.";
            // 
            // label_Files
            // 
            label_Files.AutoSize = true;
            label_Files.Location = new Point(143, 43);
            label_Files.Name = "label_Files";
            label_Files.Size = new Size(17, 20);
            label_Files.TabIndex = 6;
            label_Files.Text = "0";
            // 
            // label_Directorys
            // 
            label_Directorys.AutoSize = true;
            label_Directorys.Location = new Point(143, 23);
            label_Directorys.Name = "label_Directorys";
            label_Directorys.Size = new Size(17, 20);
            label_Directorys.TabIndex = 5;
            label_Directorys.Text = "0";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(9, 83);
            label6.Name = "label6";
            label6.Size = new Size(109, 20);
            label6.TabIndex = 4;
            label6.Text = "Estimated time";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(9, 63);
            label5.Name = "label5";
            label5.Size = new Size(44, 20);
            label5.TabIndex = 3;
            label5.Text = "Bytes";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(9, 43);
            label4.Name = "label4";
            label4.Size = new Size(38, 20);
            label4.TabIndex = 2;
            label4.Text = "Files";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(9, 23);
            label3.Name = "label3";
            label3.Size = new Size(76, 20);
            label3.TabIndex = 0;
            label3.Text = "Directorys";
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(495, 142);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(94, 29);
            btnRefresh.TabIndex = 9;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // BackupForm
            // 
            AcceptButton = button_Start;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = button_Cancel;
            ClientSize = new Size(600, 255);
            Controls.Add(btnRefresh);
            Controls.Add(groupBox1);
            Controls.Add(label2);
            Controls.Add(button_Cancel);
            Controls.Add(button1);
            Controls.Add(button_Start);
            Controls.Add(radioButton1);
            Controls.Add(radioButton2);
            Controls.Add(tbExclude);
            Controls.Add(BackupDestination);
            Controls.Add(label1);
            Name = "BackupForm";
            Text = "BackupForm";
            FormClosing += BackupForm_FormClosing;
            Load += BackupForm_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private TextBox BackupDestination;
        private Label label1;
        private Button button1;
        private TextBox tbExclude;
        private Button button_Start;
        private Button button_Cancel;
        private Label label2;
        private GroupBox groupBox1;
        private Label label_Directorys;
        private Label label6;
        private Label label5;
        private Label label4;
        private Label label3;
        private Label label_Time;
        private Label label_Bytes;
        private Label label_Files;
        private Button btnRefresh;
    }
}
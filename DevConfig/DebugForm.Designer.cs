namespace DevConfig
{
    partial class DebugForm
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
            rtbLog = new RichTextBox();
            btnClear = new Button();
            SuspendLayout();
            // 
            // rtbLog
            // 
            rtbLog.Dock = DockStyle.Fill;
            rtbLog.Location = new Point(0, 0);
            rtbLog.Name = "rtbLog";
            rtbLog.Size = new Size(800, 450);
            rtbLog.TabIndex = 0;
            rtbLog.Text = "";
            // 
            // btnClear
            // 
            btnClear.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClear.Location = new Point(694, 12);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(94, 29);
            btnClear.TabIndex = 1;
            btnClear.Text = "Clear";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            // 
            // DebugForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnClear);
            Controls.Add(rtbLog);
            Name = "DebugForm";
            Text = "DebugForm";
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private RichTextBox rtbLog;
        private Button btnClear;
    }
}
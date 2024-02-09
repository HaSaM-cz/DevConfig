namespace DevConfig
{
    partial class AddDirGetName
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
            textBoxName = new TextBox();
            button_ok = new Button();
            button_cancel = new Button();
            SuspendLayout();
            // 
            // textBoxName
            // 
            textBoxName.Location = new Point(12, 12);
            textBoxName.Name = "textBoxName";
            textBoxName.Size = new Size(303, 27);
            textBoxName.TabIndex = 0;
            // 
            // button_ok
            // 
            button_ok.DialogResult = DialogResult.OK;
            button_ok.Location = new Point(221, 45);
            button_ok.Name = "button_ok";
            button_ok.Size = new Size(94, 29);
            button_ok.TabIndex = 2;
            button_ok.Text = "OK";
            button_ok.UseVisualStyleBackColor = true;
            // 
            // button_cancel
            // 
            button_cancel.DialogResult = DialogResult.Cancel;
            button_cancel.Location = new Point(121, 45);
            button_cancel.Name = "button_cancel";
            button_cancel.Size = new Size(94, 29);
            button_cancel.TabIndex = 1;
            button_cancel.Text = "Cancel";
            button_cancel.UseVisualStyleBackColor = true;
            // 
            // AddDirGetName
            // 
            AcceptButton = button_ok;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = button_cancel;
            ClientSize = new Size(327, 82);
            Controls.Add(button_cancel);
            Controls.Add(button_ok);
            Controls.Add(textBoxName);
            Name = "AddDirGetName";
            Text = "Enter name for directory";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button button_ok;
        private Button button_cancel;
        public TextBox textBoxName;
    }
}
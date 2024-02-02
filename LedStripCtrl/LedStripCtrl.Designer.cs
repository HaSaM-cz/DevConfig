namespace LedStripCtrl
{
    partial class LedStripCtrl
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnOpenXmlFile = new Button();
            btnLoadXmlToRGB = new Button();
            btnSaveXMLFile = new Button();
            treeView1 = new TreeView();
            SuspendLayout();
            // 
            // btnOpenXmlFile
            // 
            btnOpenXmlFile.Location = new Point(3, 3);
            btnOpenXmlFile.Name = "btnOpenXmlFile";
            btnOpenXmlFile.Size = new Size(113, 29);
            btnOpenXmlFile.TabIndex = 0;
            btnOpenXmlFile.Text = "Open XML file";
            btnOpenXmlFile.UseVisualStyleBackColor = true;
            btnOpenXmlFile.Click += btnOpenXmlFile_Click;
            // 
            // btnLoadXmlToRGB
            // 
            btnLoadXmlToRGB.Location = new Point(3, 38);
            btnLoadXmlToRGB.Name = "btnLoadXmlToRGB";
            btnLoadXmlToRGB.Size = new Size(113, 29);
            btnLoadXmlToRGB.TabIndex = 1;
            btnLoadXmlToRGB.Text = "Load XML to RGB";
            btnLoadXmlToRGB.UseVisualStyleBackColor = true;
            btnLoadXmlToRGB.Click += btnLoadXmlToRGB_Click;
            // 
            // btnSaveXMLFile
            // 
            btnSaveXMLFile.Location = new Point(3, 73);
            btnSaveXMLFile.Name = "btnSaveXMLFile";
            btnSaveXMLFile.Size = new Size(113, 29);
            btnSaveXMLFile.TabIndex = 2;
            btnSaveXMLFile.Text = "Save XML file";
            btnSaveXMLFile.UseVisualStyleBackColor = true;
            btnSaveXMLFile.Click += btnSaveXMLFile_Click;
            // 
            // treeView1
            // 
            treeView1.Location = new Point(122, 3);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(270, 444);
            treeView1.TabIndex = 3;
            // 
            // LedStripCtrl
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(treeView1);
            Controls.Add(btnSaveXMLFile);
            Controls.Add(btnLoadXmlToRGB);
            Controls.Add(btnOpenXmlFile);
            Name = "LedStripCtrl";
            Size = new Size(800, 450);
            ResumeLayout(false);
        }

        #endregion

        private Button btnOpenXmlFile;
        private Button btnLoadXmlToRGB;
        private Button btnSaveXMLFile;
        private TreeView treeView1;
    }
}

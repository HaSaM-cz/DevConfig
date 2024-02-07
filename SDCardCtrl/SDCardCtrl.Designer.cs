namespace SDCardCtrlNs
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
            TreeNode treeNode1 = new TreeNode("Uzel2");
            TreeNode treeNode2 = new TreeNode("Uzel1", new TreeNode[] { treeNode1 });
            TreeNode treeNode3 = new TreeNode("Uzel3");
            TreeNode treeNode4 = new TreeNode("Uzel0", new TreeNode[] { treeNode2, treeNode3 });
            TreeNode treeNode5 = new TreeNode("Uzel4");
            treeView1 = new TreeView();
            button1 = new Button();
            SuspendLayout();
            // 
            // treeView1
            // 
            treeView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            treeView1.Location = new Point(0, 49);
            treeView1.Name = "treeView1";
            treeNode1.Name = "Uzel2";
            treeNode1.Text = "Uzel2";
            treeNode2.Name = "Uzel1";
            treeNode2.Text = "Uzel1";
            treeNode3.Name = "Uzel3";
            treeNode3.Text = "Uzel3";
            treeNode4.Name = "Uzel0";
            treeNode4.Text = "Uzel0";
            treeNode5.Checked = true;
            treeNode5.Name = "Uzel4";
            treeNode5.Text = "Uzel4";
            treeView1.Nodes.AddRange(new TreeNode[] { treeNode4, treeNode5 });
            treeView1.Size = new Size(800, 401);
            treeView1.TabIndex = 0;
            // 
            // button1
            // 
            button1.Location = new Point(12, 12);
            button1.Name = "button1";
            button1.Size = new Size(94, 29);
            button1.TabIndex = 1;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // SDCardCtrl
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(button1);
            Controls.Add(treeView1);
            Name = "SDCardCtrl";
            Text = "SDCardCtrl";
            Load += SDCardCtrl_Load;
            ResumeLayout(false);
        }

        #endregion

        private TreeView treeView1;
        private Button button1;
    }
}
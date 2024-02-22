namespace DevConfig
{
    partial class RegisterForm
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
            listViewParameters = new ListView();
            columnHeader_ID = new ColumnHeader();
            columnHeader_Type = new ColumnHeader();
            columnHeader_RO = new ColumnHeader();
            columnHeader_Min = new ColumnHeader();
            columnHeader_Max = new ColumnHeader();
            columnHeader_Index = new ColumnHeader();
            columnHeader_Name = new ColumnHeader();
            columnHeader_Value = new ColumnHeader();
            SuspendLayout();
            // 
            // listViewParameters
            // 
            listViewParameters.Columns.AddRange(new ColumnHeader[] { columnHeader_ID, columnHeader_Type, columnHeader_RO, columnHeader_Min, columnHeader_Max, columnHeader_Index, columnHeader_Name, columnHeader_Value });
            listViewParameters.Dock = DockStyle.Fill;
            listViewParameters.Location = new Point(0, 0);
            listViewParameters.Name = "listViewParameters";
            listViewParameters.Size = new Size(800, 450);
            listViewParameters.TabIndex = 0;
            listViewParameters.UseCompatibleStateImageBehavior = false;
            listViewParameters.View = View.Details;
            // 
            // columnHeader_ID
            // 
            columnHeader_ID.Text = "ID";
            columnHeader_ID.Width = 40;
            // 
            // columnHeader_Type
            // 
            columnHeader_Type.Text = "Type";
            // 
            // columnHeader_RO
            // 
            columnHeader_RO.Text = "RO";
            columnHeader_RO.Width = 50;
            // 
            // columnHeader_Min
            // 
            columnHeader_Min.Text = "Min";
            // 
            // columnHeader_Max
            // 
            columnHeader_Max.Text = "Max";
            // 
            // columnHeader_Index
            // 
            columnHeader_Index.Text = "Index";
            columnHeader_Index.Width = 50;
            // 
            // columnHeader_Name
            // 
            columnHeader_Name.Text = "Name";
            columnHeader_Name.Width = 150;
            // 
            // columnHeader_Value
            // 
            columnHeader_Value.Text = "Value";
            columnHeader_Value.Width = 100;
            // 
            // RegisterForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(listViewParameters);
            Name = "RegisterForm";
            Text = "RegisterForm";
            ResumeLayout(false);
        }

        #endregion

        private ListView listViewParameters;
        private ColumnHeader columnHeader_ID;
        private ColumnHeader columnHeader_Type;
        private ColumnHeader columnHeader_RO;
        private ColumnHeader columnHeader_Min;
        private ColumnHeader columnHeader_Max;
        private ColumnHeader columnHeader_Index;
        private ColumnHeader columnHeader_Name;
        private ColumnHeader columnHeader_Value;
    }
}
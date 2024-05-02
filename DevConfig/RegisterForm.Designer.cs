using DevConfig.Controls.ListViewExCtrl;

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
            components = new System.ComponentModel.Container();
            ListViewGroup listViewGroup1 = new ListViewGroup("Main", HorizontalAlignment.Left);
            listViewParameters = new ListViewEx();
            columnHeader_ID = new ColumnHeader();
            columnHeader_Type = new ColumnHeader();
            columnHeader_RO = new ColumnHeader();
            columnHeader_Min = new ColumnHeader();
            columnHeader_Max = new ColumnHeader();
            columnHeader_Index = new ColumnHeader();
            columnHeader_Name = new ColumnHeader();
            columnHeader_Value = new ColumnHeader();
            textBox = new TextBox();
            comboBox = new ComboBox();
            timer = new System.Windows.Forms.Timer(components);
            toolTip = new ToolTip(components);
            SuspendLayout();
            // 
            // listViewParameters
            // 
            listViewParameters.AllowColumnReorder = true;
            listViewParameters.Columns.AddRange(new ColumnHeader[] { columnHeader_ID, columnHeader_Type, columnHeader_RO, columnHeader_Min, columnHeader_Max, columnHeader_Index, columnHeader_Name, columnHeader_Value });
            listViewParameters.Dock = DockStyle.Fill;
            listViewParameters.DoubleClickActivation = false;
            listViewParameters.FullRowSelect = true;
            listViewGroup1.CollapsedState = ListViewGroupCollapsedState.Expanded;
            listViewGroup1.Header = "Main";
            listViewGroup1.Name = "lvGroupMain";
            listViewParameters.Groups.AddRange(new ListViewGroup[] { listViewGroup1 });
            listViewParameters.Location = new Point(0, 0);
            listViewParameters.MultiSelect = false;
            listViewParameters.Name = "listViewParameters";
            listViewParameters.OwnerDraw = true;
            listViewParameters.Size = new Size(961, 607);
            listViewParameters.TabIndex = 0;
            listViewParameters.UseCompatibleStateImageBehavior = false;
            listViewParameters.View = View.Details;
            listViewParameters.SubItemClicked += listViewParameters_SubItemClicked;
            listViewParameters.SubItemEndEditing += listViewParameters_SubItemEndEditing;
            listViewParameters.DrawColumnHeader += listViewParameters_DrawColumnHeader;
            listViewParameters.DrawSubItem += listViewParameters_DrawSubItem;
            listViewParameters.MouseMove += listViewParameters_MouseMove;
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
            // textBox
            // 
            textBox.Location = new Point(792, 35);
            textBox.Name = "textBox";
            textBox.Size = new Size(125, 27);
            textBox.TabIndex = 2;
            textBox.Visible = false;
            // 
            // comboBox
            // 
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.FormattingEnabled = true;
            comboBox.Location = new Point(601, 60);
            comboBox.Name = "comboBox";
            comboBox.Size = new Size(151, 28);
            comboBox.TabIndex = 3;
            // 
            // timer
            // 
            timer.Interval = 250;
            timer.Tick += timer_Tick;
            // 
            // RegisterForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(961, 607);
            Controls.Add(listViewParameters);
            Controls.Add(textBox);
            Controls.Add(comboBox);
            Name = "RegisterForm";
            Text = "RegisterForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListViewEx listViewParameters;
        private ColumnHeader columnHeader_ID;
        private ColumnHeader columnHeader_Type;
        private ColumnHeader columnHeader_RO;
        private ColumnHeader columnHeader_Min;
        private ColumnHeader columnHeader_Max;
        private ColumnHeader columnHeader_Index;
        private ColumnHeader columnHeader_Name;
        private ColumnHeader columnHeader_Value;
        private TextBox textBox;
        private ComboBox comboBox;
        private System.Windows.Forms.Timer timer;
        private ToolTip toolTip;
    }
}
using DevConfig.Controls.ListViewExCtrl;
using DevConfig.Service;
using DevConfig.Utils;
using DevConfigSupp;
using GroupedListControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Message = CanDiagSupport.Message;
using Parameter = DevConfig.Service.Parameter;

namespace DevConfig
{
    public partial class RegisterForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        enum ParmaterSubItem { ParamID, Type, RO, Min, Max, Index, Name, Value };

        ///////////////////////////////////////////////////////////////////////////////////////////
        public RegisterForm()
        {
            InitializeComponent();
            //MainForm = DevConfigService.Instance.MainForm;

            //ListGroup lg = AddGroup("Main");

            //for (int i = 1; i <= 5; i++)
            //{
            //    /*ListGroup lg = new ListGroup();
            //    lg.Columns.Add("List Group " + i.ToString(), 120);
            //    lg.Columns.Add("Group " + i + " SubItem 1", 150);
            //    lg.Columns.Add("Group " + i + " Subitem 2", 150);
            //    lg.Name = "Group " + i;*/

            //    // Now add some sample items:
            //    for (int j = 1; j <= 5; j++)
            //    {
            //        ListViewItem item = lg.Items.Add("Item " + j.ToString());
            //        item.SubItems.Add(item.Text + " SubItem 1");
            //        item.SubItems.Add(item.Text + " SubItem 2");
            //    }

            //    // Add handling for the columnRightClick Event:
            //    //lg.ColumnRightClick += new ListGroup.ColumnRightClickHandler(lg_ColumnRightClick);
            //    //lg.MouseClick += new MouseEventHandler(lg_MouseClick);

            //    lg.SubItemClicked += new SubItemEventHandler(listViewEx1_SubItemClicked);
            //    lg.SubItemEndEditing += new SubItemEndEditingEventHandler(listViewEx1_SubItemEndEditing);

            //    groupListControl1.Controls.Add(lg);
            //}
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        //ListGroup AddGroup(string gr_name)
        //{
        //    ListGroup lg = new ListGroup();
        //    lg.Columns.Add("ID", 40);
        //    lg.Columns.Add("Type", 60);
        //    lg.Columns.Add("RO", 50);
        //    lg.Columns.Add("Min", 60);
        //    lg.Columns.Add("Max", 60);
        //    lg.Columns.Add("Index", 50);
        //    lg.Columns.Add("Name", 150);
        //    lg.Columns.Add("Value", 100);
        //    lg.Name = gr_name;

        //    lg.SubItemClicked += new SubItemEventHandler(listViewEx1_SubItemClicked);
        //    lg.SubItemEndEditing += new SubItemEndEditingEventHandler(listViewEx1_SubItemEndEditing);

        //    groupListControl1.Controls.Add(lg);
        //    return lg;
        //}

        ///////////////////////////////////////////////////////////////////////////////////////////
        internal void UpdateList()
        {
            listViewParameters.Items.Clear();

            if (DevConfigService.Instance.selectedDevice != null &&
                DevConfigService.Instance.selectedDevice.Parameters != null)
            {
                foreach (var parameter in DevConfigService.Instance.selectedDevice.Parameters)
                {
                    ListViewItem listViewItem = new ListViewItem($"{parameter.ParameterID}");
                    listViewItem.SubItems.AddRange(new string[]
                    {
                        $"{parameter.Type}",
                        $"{parameter.ReadOnly}",
                        $"{parameter.StrMin}",
                        $"{parameter.StrMax}",
                        $"{parameter.Index}",
                        $"{parameter.Name}",
                        $"{parameter.StrValue}",
                    });
                    listViewItem.ToolTipText = parameter.Description;

                    // najdeme grupu pro polozku
                    ListViewGroup? group = null;
                    if (parameter.Index == null)
                    {
                        group = listViewParameters.Groups[0];
                    }
                    else
                    {
                        group = (from xx in listViewParameters.Groups.Cast<ListViewGroup>()
                                 where xx.Tag != null
                                && xx.Tag.GetType() == typeof(byte)
                                && (byte)xx.Tag == parameter.ParameterID
                                 select xx).SingleOrDefault();
                        if (group == null)
                        {
                            group = new ListViewGroup($"{parameter.Name} ({parameter.ParameterID})", HorizontalAlignment.Left)
                            {
                                Tag = parameter.ParameterID,
                                CollapsedState = ListViewGroupCollapsedState.Expanded,
                            };
                            listViewParameters.Groups.Add(group);
                        }
                    }
                    listViewItem.Group = group;

                    listViewParameters.Items.Add(listViewItem).Tag = parameter;
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void listViewParameters_SubItemClicked(object sender, SubItemEventArgs e)
        {
            if (e.SubItem == (int)ParmaterSubItem.Value && e.Item.Tag != null)
            {
                Parameter par = (Parameter)(e.Item.Tag);
                if (!par.ReadOnly)
                {
                    if (DevConfigService.Instance.TryGetParamEnum(par.Format, out Dictionary<uint, string> di_enums))
                    {
                        comboBox.Items.Clear();
                        comboBox.Items.AddRange(di_enums.Values.ToArray());
                        comboBox.Text = $"{par.StrValue}";
                        listViewParameters.StartEditing(comboBox, e.Item, e.SubItem, $"{par.Value}");
                    }
                    else
                    {
                        if (par.Value == null || !par.Value.IsNumericType())
                            listViewParameters.StartEditing(textBox, e.Item, e.SubItem, $"{par.Value}");
                        else
                            listViewParameters.StartEditing(textBox, e.Item, e.SubItem, (Convert.ToDouble(par.Value) * (par.Gain ?? 1.0) + (par.Offset ?? 0.0)).ToRoundedString());
                    }
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void listViewParameters_SubItemEndEditing(object sender, SubItemEndEditingEventArgs e)
        {
            Debug.Assert(e.Item.Tag != null);

            Parameter par = ((Parameter)e.Item.Tag);

            if (par.IsNumeric)
            {
                if (DevConfigService.Instance.TryGetParamEnum(par.Format, out Dictionary<uint, string> di_enums))
                {
                    var x = (from xx in di_enums where xx.Value == e.DisplayText select xx.Key).First();

                    switch (par.Type)
                    {
                        case ParamType.UInt8: par.Value = Convert.ToByte(x); break;
                        case ParamType.SInt8: par.Value = Convert.ToSByte(x); break;
                        case ParamType.UInt16: par.Value = Convert.ToUInt16(x); break;
                        case ParamType.SInt16: par.Value = Convert.ToInt16(x); break;
                        case ParamType.UInt32: par.Value = Convert.ToUInt32(x); break;
                        case ParamType.SInt32: par.Value = Convert.ToInt32(x); break;
                    }
                }
                else
                {
                    if (DevConfigService.Instance.TryParse(e.DisplayText, out double new_value)) //if (double.TryParse(e.DisplayText, out double new_value))
                    {
                        new_value -= (par.Offset ?? 0.0);
                        new_value /= (par.Gain ?? 1.0);

                        switch (par.Type)
                        {
                            case ParamType.UInt8: par.Value = Convert.ToByte(new_value); break;
                            case ParamType.SInt8: par.Value = Convert.ToSByte(new_value); break;
                            case ParamType.UInt16: par.Value = Convert.ToUInt16(new_value); break;
                            case ParamType.SInt16: par.Value = Convert.ToInt16(new_value); break;
                            case ParamType.UInt32: par.Value = Convert.ToUInt32(new_value); break;
                            case ParamType.SInt32: par.Value = Convert.ToInt32(new_value); break;
                        }
                    }
                }
                if (Convert.ToDouble(par.Value) < Convert.ToDouble(par.MinVal) || Convert.ToDouble(par.MaxVal) < Convert.ToDouble(par.Value))
                {
                    if (MessageBox.Show($"The value is outside the allowed range.{Environment.NewLine}Do you want to return the original value?", "DevConfig - error", 
                        MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        par.Value = par.OldValue;
                    else
                        e.Cancel = true;
                }
            }
            else if (par.Type == ParamType.Bool)
            {
                if (e.DisplayText.Length > 0)
                {
                    par.Value = e.DisplayText[0] switch
                    {
                        'a' => true,
                        'A' => true,
                        't' => true,
                        'T' => true,
                        '1' => true,
                        _ => false,
                    };
                }
                else
                {
                    par.Value = false;
                }
            }
            else if (par.Type == ParamType.String)
            {
                par.Value = e.DisplayText;
            }

            if ($"{par.Value}" != $"{par.OldValue}") // prime srivnani nefunguje !!!! if (par.Value != par.OldValue)
                e.Item.ForeColor = Color.Red;
            else
                e.Item.ForeColor = SystemColors.WindowText;

            e.DisplayText = par.StrValue;
        }
    }
}

using DevConfig.Controls.ListViewExCtrl;
using DevConfig.Service;
using DevConfig.Utils;
using System.Data;
using System.Diagnostics;
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
        }

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
                    //listViewItem.ToolTipText = parameter.Description;

                    if ($"{parameter.Value}" != $"{parameter.OldValue}") // prime srivnani nefunguje !!!! if (par.Value != par.OldValue)
                        listViewItem.ForeColor = Color.Red;

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

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void listViewParameters_MouseMove(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = listViewParameters.HitTest(e.X, e.Y);
            if (info.SubItem != null)
            {
                Point p = e.Location;
                p.Y = info.SubItem.Bounds.Y;
                p.Offset(20, 25);
                if (timer.Tag == null || (Point)timer.Tag != p)
                {
                    timer.Tag = p;
                    timer.Enabled = false;
                    timer.Enabled = true;
                    if (toolTip.Tag != info)
                    {
                        toolTip.Tag = info;
                        toolTip.SetToolTip(listViewParameters, null);
                    }
                }
            }
            else
            {
                toolTip.Tag = null;
                toolTip.SetToolTip(listViewParameters, null);
                timer.Enabled = false;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Enabled = false;
            if (toolTip.Tag != null)
            {
                ListViewHitTestInfo info = (ListViewHitTestInfo)toolTip.Tag;
                Parameter pp = (Parameter)info.Item?.Tag!;
                ParmaterSubItem ix = (ParmaterSubItem)info.Item!.SubItems.IndexOf(info.SubItem);

                string? str_tip = ix switch
                {
                    ParmaterSubItem.ParamID => $"0x{pp.ParameterID:X2}",
                    ParmaterSubItem.Name => pp.Description ?? pp.Name,
                    ParmaterSubItem.Min => $"{pp.MinVal}",
                    ParmaterSubItem.Max => $"{pp.MaxVal}",
                    ParmaterSubItem.Value => pp.IsNumeric ? $"{pp.Value} (0x{pp.Value:X})" : $"{pp.Value}",
                    //ParmaterSubItem.Index:
                    //ParmaterSubItem.Type:
                    //ParmaterSubItem.RO:
                    _ => null,
                };

                if (!string.IsNullOrWhiteSpace(str_tip))
                {
                    Point point = (Point)timer.Tag!;
                    toolTip.Show(str_tip, listViewParameters, point);
                }
                toolTip.Tag = null;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
    }
}

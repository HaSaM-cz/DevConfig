using DevConfig.Service;
using DevConfigSupp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Message = CanDiagSupport.Message;

namespace DevConfig
{
    public partial class RegisterForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        ///////////////////////////////////////////////////////////////////////////////////////////
        public RegisterForm()
        {
            InitializeComponent();
            //MainForm = DevConfigService.Instance.MainForm;
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
                    listViewParameters.Items.Add(listViewItem);
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DevConfig
{
    public partial class TreeForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        MainForm MainForm;
        public TreeForm(MainForm mainForm)
        {
            InitializeComponent();
            MainForm = mainForm;
        }

        private void listViewDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            var items = listViewDevices.SelectedItems;
            if (items != null && items.Count == 1)
            {
                var item = items[0];
                if (item != null)
                {
                    MainForm.selectedDevice = (Device)item.Tag;
                    MainForm.selectedDeviceType = MainForm.GetDeviceType(MainForm.selectedDevice.DevId);
                    Debug.WriteLine($"Selected DevID = {MainForm.selectedDevice.DevId:X}");
                    MainForm.tb_address.Text = MainForm.selectedDevice.AddressStr;
                    MainForm.tb_dev_id.Text = MainForm.selectedDevice.DevIdStr;
                    MainForm.tb_version.Text = MainForm.selectedDevice.FwVer;
                    MainForm.tb_cpu_id.Text = MainForm.selectedDevice.CpuId;
                    MainForm.label_name.Text = MainForm.selectedDevice.Name;
                    MainForm.label_name.ForeColor = MainForm.tb_address.ForeColor = MainForm.tb_dev_id.ForeColor = MainForm.tb_version.ForeColor = MainForm.tb_cpu_id.ForeColor = SystemColors.WindowText;

                    MainForm.SetTabPage();
                }
            }
        }
    }
}

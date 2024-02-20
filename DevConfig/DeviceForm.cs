using CanDiagSupport;
using DevConfig.Service;
using Renci.SshNet.Messages;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Message = CanDiagSupport.Message;

namespace DevConfig
{
    public partial class DeviceForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        MainForm MainForm;

        ///////////////////////////////////////////////////////////////////////////////////////////
        public DeviceForm(MainForm mainForm)
        {
            InitializeComponent();
            MainForm = mainForm;
            tb_address.BackColor = tb_dev_id.BackColor = tb_version.BackColor = tb_cpu_id.BackColor = tb_address.BackColor;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void btnIdent_Click(object sender, EventArgs e)
        {
            if (DevConfigService.Instance.selectedDevice != null)
            {
                Debug.Assert(DevConfigService.Instance.selectedDevice.listViewItem != null);
                MainForm.btn_update_active = true;
                tb_address.ForeColor = tb_dev_id.ForeColor = tb_version.ForeColor = tb_cpu_id.ForeColor = Color.LightGray;
                tb_address.Font = tb_dev_id.Font = tb_version.Font = tb_cpu_id.Font = new Font(tb_address.Font, FontStyle.Regular);
                Task.Delay(1000).ContinueWith(task =>
                {
                    Message message = new() { CMD = Command.Ident, DEST = DevConfigService.Instance.selectedDevice.Address };
                    DevConfigService.Instance.InputPeriph?.SendMsg(message);
                });
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (DevConfigService.Instance.ProcessLock())
            {
                DevConfigService.Instance.FreeProcessLock();
                if (File.Exists(tbFwFileName.Text) && DevConfigService.Instance.selectedDevice != null)
                    DevConfigService.Instance.UpdateFw(tbFwFileName.Text);
            }
        }

            ///////////////////////////////////////////////////////////////////////////////////////////
            private void btnReset_Click(object sender, EventArgs e)
        {
            if (DevConfigService.Instance.selectedDevice != null)
            {
                Message message = new() { CMD = Command.Reset, DEST = DevConfigService.Instance.selectedDevice.Address };
                DevConfigService.Instance.InputPeriph?.SendMsg(message);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Encrypted binary files (*.bin)|*.bin|All files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                tbFwFileName.Text = ofd.FileName;
                if (MainForm.selectedDeviceType != null)
                    MainForm.selectedDeviceType.FirmwarePath = ofd.FileName;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////
    }
}

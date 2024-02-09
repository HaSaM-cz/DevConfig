using CanDiagSupport;
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
        byte MessageFlag = 0xFF;

        ///////////////////////////////////////////////////////////////////////////////////////////
        public DeviceForm(MainForm mainForm)
        {
            InitializeComponent();
            MainForm = mainForm;
            tb_address.BackColor = tb_dev_id.BackColor = tb_version.BackColor = tb_cpu_id.BackColor = tb_address.BackColor;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        internal void NewInputPeriph()
        {
            Debug.Assert(MainForm.InputPeriph != null);
            MainForm.InputPeriph.MessageReceived += InputPeriph_MessageReceived;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void InputPeriph_MessageReceived(Message message)
        {
            if ((message.CMD == Command.StartUpdate || message.CMD == Command.UpdateMsg) && message.Data.Count == 1)
            {
                MessageFlag = message.Data[0];
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void btnIdent_Click(object sender, EventArgs e)
        {
            if (MainForm.selectedDevice != null)
            {
                Debug.Assert(MainForm.selectedDevice.listViewItem != null);
                MainForm.btn_update_active = true;
                tb_address.ForeColor = tb_dev_id.ForeColor = tb_version.ForeColor = tb_cpu_id.ForeColor = Color.LightGray;
                tb_address.Font = tb_dev_id.Font = tb_version.Font = tb_cpu_id.Font = new Font(tb_address.Font, FontStyle.Regular);
                Task.Delay(1000).ContinueWith(task =>
                {
                    Message message = new() { CMD = Command.Ident, DEST = MainForm.selectedDevice.Address };
                    MainForm.InputPeriph?.SendMsg(message);
                });
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (File.Exists(tbFwFileName.Text) && MainForm.selectedDevice != null)
            {
                MainForm.progressBar.Minimum = 0;
                MainForm.progressBar.Maximum = 100;
                MainForm.progressBar.Value = 0;

                const int UpdateTmeout = 3000;
                byte[] sendbuf = new byte[260];

                MessageFlag = 0xFF;
                Message message = new Message() { CMD = Command.StartUpdate, DEST = MainForm.selectedDevice.Address };
                MainForm.InputPeriph?.SendMsg(message);

                int time = 10;
                while (time-- > 0)
                {
                    Task.Delay(100).Wait(); //Thread.Sleep(100);
                    Application.DoEvents();
                }

                Stream fBin = File.OpenRead(tbFwFileName.Text);

                byte[] buf = new byte[fBin.Length];

                fBin.Read(buf, 0, buf.Length);
                fBin.Close();

                MainForm.AppendToDebug("Uploading FW");

                byte perc = 0;
                MainForm.progressBar.Value = 0;
                Application.DoEvents();
                int offset = 0;
                byte ptr = 0;
                time = UpdateTmeout;
                while (offset < buf.Length)
                {
                    if (MessageFlag != 0xFF)
                    {
                        if (MessageFlag != 0x00)
                        {
                            MessageBox.Show((/*(UpdateEnum)*/MessageFlag).ToString());
                            //pgbUpdate.Visible = false;
                            //Disable_DEBUG_msg = 0;
                            return;
                        }

                        time = UpdateTmeout;
                        MessageFlag = 0xFF;
                        ptr = 0;
                        for (int i = 0; i < 240; i++)
                        {
                            sendbuf[i] = buf[offset];
                            offset++;
                            ptr++;
                            if (offset >= buf.Length)
                                break;
                        }

                        message.CMD = Command.UpdateMsg;
                        message.Data = sendbuf.Take(ptr).ToList();

                        MainForm.InputPeriph?.SendMsg(message);

                        byte px = (byte)(((float)offset / buf.Length) * 100);
                        if (perc != px)
                        {
                            perc = px;
                            MainForm.progressBar.Value = perc;
                            Application.DoEvents();
                        }
                    }

                    time--;
                    if (time == 0)
                    {
                        MessageBox.Show("No response");
                        MainForm.progressBar.Value = 0; //pgbUpdate.Visible = false;
                        //Disable_DEBUG_msg = 0;
                        return;
                    }
                    Task.Delay(1).Wait(); //Thread.Sleep(1);
                    Application.DoEvents();
                }

                //Disable_DEBUG_msg = 0;

                MainForm.AppendToDebug("Uploading FW DONE");

                //Task.Delay(200).Wait(); //Thread.Sleep(200);
                //Application.DoEvents();
                //Task.Delay(200).Wait(); //Thread.Sleep(200);

                //CloseSerialPort(false);
                //AppendToDebug("REOPEN THE PORT FOR FUTURE OPERATIONS");

                MainForm.progressBar.Value = 0;//pgbUpdate.Visible = false;

            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void btnReset_Click(object sender, EventArgs e)
        {
            if (MainForm.selectedDevice != null)
            {
                Message message = new() { CMD = Command.Reset, DEST = MainForm.selectedDevice.Address };
                MainForm.InputPeriph?.SendMsg(message);
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
    }
}

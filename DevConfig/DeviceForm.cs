using CanDiagSupport;
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
        bool btn_update_active = false;

        public DeviceForm(MainForm mainForm)
        {
            InitializeComponent();
            MainForm = mainForm;
        }

        private void btnIdent_Click(object sender, EventArgs e)
        {
            if (MainForm.selectedDevice != null)
            {
                Debug.Assert(MainForm.selectedDevice.listViewItem != null);
                btn_update_active = true;
                tb_address.ForeColor = tb_dev_id.ForeColor = tb_version.ForeColor = tb_cpu_id.ForeColor = Color.LightGray;
                tb_address.Font = tb_dev_id.Font = tb_version.Font = tb_cpu_id.Font = new Font(tb_address.Font, FontStyle.Regular);
                Task.Delay(1000).ContinueWith(task =>
                {
                    Message message = new() { CMD = Command.Ident, DEST = MainForm.selectedDevice.Address };
                    MainForm.InputPeriph?.SendMsg(message);
                });
            }
        }

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

                System.IO.Stream fBin = System.IO.File.OpenRead(tbFwFileName.Text);

                byte[] buf = new byte[fBin.Length];

                fBin.Read(buf, 0, buf.Length);
                fBin.Close();

                //AppendToDebug("Uploading FW");

                //Disable_DEBUG_msg = 1;

                //pgbUpdate.Visible = true;

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

                //AppendToDebug("Uploading FW DONE");

                //Task.Delay(200).Wait(); //Thread.Sleep(200);
                //Application.DoEvents();
                //Task.Delay(200).Wait(); //Thread.Sleep(200);

                //CloseSerialPort(false);
                //AppendToDebug("REOPEN THE PORT FOR FUTURE OPERATIONS");

                MainForm.progressBar.Value = 0;//pgbUpdate.Visible = false;

            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {

        }

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
        private void NewIdent(uint deviceID, byte address, string fwVer, string cpuId, byte state)
        {
            Device? device = (from xxx in MainForm.DevicesList where xxx.CpuId == cpuId select xxx).FirstOrDefault();
            if (device != null)
            {
                //device.CpuId = cpuId; // cpu id se shoduje
                device.Address = address;
                device.DevId = deviceID;
                device.FwVer = fwVer;
                device.Name = MainForm.GetDeviceName(address, deviceID);

                // update list
                /*if (device.listViewItem != null)
                {
                    listViewDevices.Invoke(delegate
                    {
                        //device.listViewItem.SubItems[(int)(SubItem.CpuID)].Text = device.CpuId;
                        device.listViewItem.SubItems[(int)(DeviceSubItem.Address)].Text = device.AddressStr;
                        device.listViewItem.SubItems[(int)(DeviceSubItem.DevID)].Text = device.DevIdStr;
                        device.listViewItem.SubItems[(int)(DeviceSubItem.Name)].Text = device.Name;
                        device.listViewItem.SubItems[(int)(DeviceSubItem.Version)].Text = device.FwVer;
                    });
                }*/

                if (device.Equals(MainForm.selectedDevice))
                {
                    tb_address.Invoke(delegate
                    {
                        tb_address.Text = device.AddressStr;
                        tb_dev_id.Text = device.DevIdStr;
                        tb_version.Text = device.FwVer;
                        tb_cpu_id.Text = device.CpuId;
                        if (btn_update_active)
                        {
                            btn_update_active = false;
                            tb_address.ForeColor = tb_dev_id.ForeColor = tb_version.ForeColor = tb_cpu_id.ForeColor = Color.Green;
                            tb_address.Font = tb_dev_id.Font = tb_version.Font = tb_cpu_id.Font = new Font(tb_address.Font, FontStyle.Bold);
                        }
                    });
                }
            }
            else
            {
                device = new Device()
                {
                    Address = address,
                    CpuId = cpuId,
                    DevId = deviceID,
                    FwVer = fwVer,
                    Name = MainForm.GetDeviceName(address, deviceID),
                };

                MainForm.DevicesList.Add(device);

                /*device.listViewItem = new ListViewItem(device.AddressStr);
                device.listViewItem.Tag = device;

                device.listViewItem.SubItems.Add(device.DevIdStr);
                device.listViewItem.SubItems.Add(device.Name);
                device.listViewItem.SubItems.Add(device.FwVer);
                device.listViewItem.SubItems.Add(device.CpuId);

                listViewDevices.Invoke(delegate
                {
                    listViewDevices.Items.Add(device.listViewItem);

                    // zkontrolujeme jestli nemame dve zarizeni se stejnou adresou
                    var device_dup = from xxx in DevicesList where xxx.Address == address select xxx;
                    if (device_dup != null && device_dup.Count() > 1)
                    {
                        foreach (var item in device_dup)
                        {
                            if (item.listViewItem != null)
                                item.listViewItem.SubItems[0].ForeColor = Color.Red;
                        }
                    }
                });*/
            }
        }


    }
}

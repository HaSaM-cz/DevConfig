using CanDiag;
using CanDiagSupport;
using DevConfig.Properties;
using DevConfig.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.DirectoryServices;
using System.Net;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Windows.Forms;
using static CanDiag.TraceExtension;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using Message = CanDiagSupport.Message;

namespace DevConfig
{
    public partial class MainForm : Form
    {
        enum SubItem { Address, DevID, Name, Version, CpuID };

        bool btn_update_active = false;

        List<DeviceType>? DevicesTypeList;

        List<Device> DevicesList = new();
        IInputPeriph? InputPeriph = null;

        public BindingList<Device>? DeviceList = new();

        ///////////////////////////////////////////////////////////////////////////////////////////
        public MainForm()
        {
            InitializeComponent();
            listViewDevices.AutoResizeColumn(listViewDevices.Columns.Count - 1, ColumnHeaderAutoResizeStyle.HeaderSize);
            tb_address.BackColor = tb_dev_id.BackColor = tb_version.BackColor = tb_cpu_id.BackColor = tb_address.BackColor;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void MainForm_Load(object sender, EventArgs e)
        {
            DevicesTypeList = JsonConvert.DeserializeObject<List<DeviceType>>(File.ReadAllText(@"Resources\Devices.json"));
            string[] BLPaths = Settings.Default.BLPath.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            for(int i = 0; i < BLPaths.Length; i+=2)
            {
                uint devid = Convert.ToUInt32(BLPaths[i]);
                DeviceType t = GetDeviceType(devid);
                t.FirmwarePath = BLPaths[i+1];
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            string BLPath = string.Empty;
            DevicesTypeList?.ForEach(devType => 
            {
                if (!string.IsNullOrWhiteSpace(devType.FirmwarePath))
                    BLPath += $"{devType.DevId}|{devType.FirmwarePath}|";
            });
            if (Settings.Default.BLPath != BLPath)
            {
                Settings.Default.BLPath = BLPath;
                Settings.Default.Save();
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void listViewDevices_Resize(object sender, EventArgs e)
        {
            listViewDevices.AutoResizeColumn(listViewDevices.Columns.Count - 1, ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void listViewDevices_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            int col = listViewDevices.Columns.Count - 1;
            if (col != e.ColumnIndex)
                listViewDevices.AutoResizeColumn(col, ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void Open_Click(object sender, EventArgs e)
        {
            ConnectForm connectForm = new ConnectForm();

            if (connectForm.ShowDialog() == DialogResult.Continue && connectForm.InputPeriph != null)
            {
                if (InputPeriph != null)
                    InputPeriph.Close();

                InputPeriph = connectForm.InputPeriph;
                InputPeriph!.MessageReceived += InputPeriph_MessageReceived;
                RefreshDeviceList();
            }

        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void Close_Click(object sender, EventArgs e)
        {
            if (InputPeriph != null)
            {
                InputPeriph.Close();
                InputPeriph = null;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void InputPeriph_MessageReceived(Message message)
        {
            if (message.CMD == 0x02 && message.Data.Count >= 14) // TODO && msg.RF)
            {
                Debug.WriteLine(message);

                byte state = message.Data[0];
                uint deviceID = (uint)(message.Data[1] << 24 | message.Data[2] << 16 | message.Data[3] << 8 | message.Data[4]);
                byte address = message.SRC;// message.Data[5];
                string fwVer = $"{message.Data[6]}.{message.Data[7]}";
                string cpuId = $"{BitConverter.ToString(message.Data.Skip(8).Take(12).ToArray()).Replace("-", " ")}";

                NewIdent(deviceID, address, fwVer, cpuId, state);
            }
            else if ((message.CMD == 0x50 || message.CMD == 0x51) && message.Data.Count == 1)
            {
                UpdateMessageFlag = message.Data[0];
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void NewIdent(uint deviceID, byte address, string fwVer, string cpuId, byte state)
        {
            Device? device = (from xxx in DevicesList where xxx.CpuId == cpuId select xxx).FirstOrDefault();
            if (device != null)
            {
                //device.CpuId = cpuId; // cpu id se shoduje
                device.Address = address;
                device.DevId = deviceID;
                device.FwVer = fwVer;
                device.Name = GetDeviceName(address, deviceID);

                // update list
                if(device.listViewItem != null)
                {
                    listViewDevices.Invoke(delegate 
                    {
                        //device.listViewItem.SubItems[(int)(SubItem.CpuID)].Text = device.CpuId;
                        device.listViewItem.SubItems[(int)(SubItem.Address)].Text = device.AddressStr;
                        device.listViewItem.SubItems[(int)(SubItem.DevID)].Text = device.DevIdStr;
                        device.listViewItem.SubItems[(int)(SubItem.Name)].Text = device.Name;
                        device.listViewItem.SubItems[(int)(SubItem.Version)].Text = device.FwVer;
                    });
                }

                if (device.Equals(selectedDevice))
                {
                    tabSwUpdate.Invoke(delegate
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
                    Name = GetDeviceName(address, deviceID),
                };

                DevicesList.Add(device);

                device.listViewItem = new ListViewItem(device.AddressStr);
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
                });
            }
        }

        ///////////////////////////////////////////////////////////////////////
        TraceExtension? traceExtension = null;
        public TraceExtension? TraceExtension
        {
            get
            {
                if (traceExtension == null)
                {
                    string trc_ext_fname = @"D:\HASAM\CanDiag\CanDiag\CanDiag1\Project\Turnstile\Turnstile.cs";//TODO Path.ChangeExtension(FilePath, "cs");
                    traceExtension = Util.FromFile(trc_ext_fname) as TraceExtension;
                }
                return traceExtension;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private DeviceType GetDeviceType(uint deviceID)
        {
            Debug.Assert(DevicesTypeList != null);

            DeviceType? t = (from xxx in DevicesTypeList where xxx.DevId == deviceID select xxx).FirstOrDefault();
            if (t == null)
            {
                t = new DeviceType() { DevId = deviceID, Name = $"Unknown device 0x{deviceID:X}" };
                DevicesTypeList.Add(t);
            }
            return t;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private string GetDeviceName(byte address, uint deviceID)
        {
            return GetDeviceType(deviceID).Name;
            //return TraceExtension?.GetUnitName(address) ?? "";
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void RefreshList_Click(object sender, EventArgs e)
        {
            RefreshDeviceList();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void RefreshDeviceList()
        {
            if (InputPeriph != null)
            {
                selectedDevice = null;
                DevicesList.Clear();
                listViewDevices.Items.Clear();

                progressBar.Minimum = 0;
                progressBar.Maximum = 0xFD;
                progressBar.Value = 0;

                Task.Run(() =>
                {
                    Invoke(delegate { Cursor = Cursors.WaitCursor; });

                    Message message = new Message();
                    message.CMD = 0x02;
                    message.SRC = 0x08;

                    // pošleme ident do všech zažízení
                    for (byte dest = 0x0; dest < 0xFE; dest++)
                    {
                        message.DEST = dest;
                        InputPeriph.SendMsg(message);
                        Task.Delay(3).Wait();
                        progressBar.Invoke(delegate { progressBar.Value = dest; });
                    }

                    // Pockame na dobehnuti
                    Task.Delay(500).ContinueWith(t =>
                    {
                        progressBar.Invoke(delegate
                        {
                            progressBar.Value = 0;
                            Cursor = Cursors.Default;
                        });
                    });
                });
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void btnIdent_Click(object sender, EventArgs e)
        {
            if (selectedDevice != null)
            {
                Debug.Assert(selectedDevice.listViewItem != null);
                btn_update_active = true;
                tb_address.ForeColor = tb_dev_id.ForeColor = tb_version.ForeColor = tb_cpu_id.ForeColor = Color.LightGray;
                tb_address.Font = tb_dev_id.Font = tb_version.Font = tb_cpu_id.Font = new Font(tb_address.Font, FontStyle.Regular);
                Task.Delay(1000).ContinueWith(task =>
                {
                    Message message = new() { CMD = 0x02, DEST = selectedDevice.Address };
                    InputPeriph?.SendMsg(message);
                });
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
                if (selectedDeviceType != null)
                    selectedDeviceType.FirmwarePath = ofd.FileName;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        byte UpdateMessageFlag = 0xFF;

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (File.Exists(tbFwFileName.Text) && selectedDevice != null)
            {
                progressBar.Minimum = 0;
                progressBar.Maximum = 100;
                progressBar.Value = 0;

                const int UpdateTmeout = 3000;
                byte[] sendbuf = new byte[260];

                UpdateMessageFlag = 0xFF;
                Message message = new Message() { CMD = 0x50, DEST = selectedDevice.Address };
                InputPeriph?.SendMsg(message); // Send_command(cmds.teCmd_StartUpdate, 0x00, sendbuf);

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
                progressBar.Value = 0;
                Application.DoEvents();
                int offset = 0;
                byte ptr = 0;
                time = UpdateTmeout;
                while (offset < buf.Length)
                {
                    if (UpdateMessageFlag != 0xFF)
                    {
                        if (UpdateMessageFlag != 0x00)
                        {
                            MessageBox.Show((/*(UpdateEnum)*/UpdateMessageFlag).ToString());
                            //pgbUpdate.Visible = false;
                            //Disable_DEBUG_msg = 0;
                            return;
                        }

                        time = UpdateTmeout;
                        UpdateMessageFlag = 0xFF;
                        ptr = 0;
                        for (int i = 0; i < 240; i++)
                        {
                            sendbuf[i] = buf[offset];
                            offset++;
                            ptr++;
                            if (offset >= buf.Length)
                                break;
                        }

                        message.CMD = 0x51;
                        message.Data = sendbuf.Take(ptr).ToList();

                        InputPeriph?.SendMsg(message); // Send_command(cmds.teCmd_UpdateMsg, ptr, sendbuf);

                        byte px = (byte)(((float)offset / buf.Length) * 100);
                        if (perc != px)
                        {
                            perc = px;
                            progressBar.Value = perc;
                            Application.DoEvents();
                        }
                    }

                    time--;
                    if (time == 0)
                    {
                        MessageBox.Show("No response");
                        progressBar.Value = 0; //pgbUpdate.Visible = false;
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

                progressBar.Value = 0;//pgbUpdate.Visible = false;

            }
        }

        Device? selectedDevice = null;
        DeviceType? selectedDeviceType = null;

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void listViewDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            var items = listViewDevices.SelectedItems;
            if (items != null && items.Count == 1)
            {
                var item = items[0];
                if (item != null)
                {
                    selectedDevice = (Device)item.Tag;
                    Debug.WriteLine($"Selected DevID = {selectedDevice.DevId:X}");
                    selectedDeviceType = GetDeviceType(selectedDevice.DevId);
                    tbFwFileName.Text = selectedDeviceType.FirmwarePath;
                    tb_address.Text = selectedDevice.AddressStr;
                    tb_dev_id.Text = selectedDevice.DevIdStr;
                    tb_version.Text = selectedDevice.FwVer;
                    tb_cpu_id.Text = selectedDevice.CpuId;
                    label_name.Text = selectedDevice.Name;
                    tbFwFileName.ForeColor = tb_address.ForeColor = tb_dev_id.ForeColor = tb_version.ForeColor = tb_cpu_id.ForeColor = SystemColors.WindowText;
                    tb_address.Font = tb_dev_id.Font = tb_version.Font = tb_cpu_id.Font = new Font(tb_address.Font, FontStyle.Regular);
                }
            }
        }
    }

    public class Device
    {
        public byte Address { get; set; }
        public string AddressStr { get { return $"{Address:X2}"; } }
        public uint DevId { get; set; }
        public string DevIdStr { get { return $"{DevId:X}"; } }

        public string? Name;// { get; set; }
        public string? FwVer;// { get; set; }
        public string? CpuId;// { get; set; }

        public ListViewItem? listViewItem = null;
    }

    ////////////////////////////////////////////////////////////////////////////
    internal class SimpleUnloadableAssemblyLoadContext : AssemblyLoadContext
    {
        //public SimpleUnloadableAssemblyLoadContext() : base(true) { }
        //protected override Assembly Load(AssemblyName assemblyName) { return null; }
    }
}

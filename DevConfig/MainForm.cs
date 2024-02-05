using CanDiag;
using CanDiagSupport;
using DevConfig.Properties;
using DevConfig.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.DirectoryServices;
using System.Net;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Windows.Forms;
using UsbSerialNs;
using WeifenLuo.WinFormsUI.Docking;
using static CanDiag.TraceExtension;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using Message = CanDiagSupport.Message;

namespace DevConfig
{
    public partial class MainForm : Form
    {
        const byte SrcAddress = 0x08;

        TreeForm? TreeWnd = null;
        DebugForm? DebugWnd = null;

        public Device? selectedDevice = null;
        public DeviceType? selectedDeviceType = null;

        enum DeviceSubItem { Address, DevID, Name, Version, CpuID };
        enum ParmaterSubItem { ParamID, Type, RO, Min, Max, Index, Name, Value };

        bool btn_update_active = false;
        ManualResetEvent MessageReceived = new(false);
        byte MessageFlag = 0xFF;

        List<DeviceType>? DevicesTypeList;

        List<Device> DevicesList = new();
        public IInputPeriph? InputPeriph = null;
        IMainApp MainApp;

        public BindingList<Device>? DeviceList = new();

        ///////////////////////////////////////////////////////////////////////////////////////////
        public MainForm()
        {
            MainApp = new MainAppClass(this);
            InitializeComponent();
            //listViewDevices.AutoResizeColumn(listViewDevices.Columns.Count - 1, ColumnHeaderAutoResizeStyle.HeaderSize);
            //tb_address.BackColor = tb_dev_id.BackColor = tb_version.BackColor = tb_cpu_id.BackColor = tb_address.BackColor;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void MainForm_Load(object sender, EventArgs e)
        {
            DevicesTypeList = JsonConvert.DeserializeObject<List<DeviceType>>(File.ReadAllText(@"Resources\Devices.json"));
            string[] BLPaths = Settings.Default.BLPath.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            for (int i = 0; i < BLPaths.Length; i += 2)
            {
                uint devid = Convert.ToUInt32(BLPaths[i]);
                DeviceType t = GetDeviceType(devid);
                t.FirmwarePath = BLPaths[i + 1];
            }
            TreeWnd = CreateChild<TreeForm>("Device");
            DebugWnd = CreateChild<DebugForm>("Debug");
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void DeviceTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*if (TreeWnd == null)
            {
                CreateTreeWnd();
            }
            else
            {
                if (TreeWnd.WindowState != FormWindowState.Maximized)
                    TreeWnd.WindowState = FormWindowState.Normal;
                TreeWnd.BringToFront();
            }*/
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        T? CreateChild<T>(string Text)
        {
            Type t = typeof(T);
            var obj = Activator.CreateInstance(t, new object[] { this });
            if (obj != null)
            {
                DockContent content = (DockContent)obj;
                content.Text = Text;
                content.Tag = Text;
                content.MdiParent = this;
                content.FormClosed += ((object? sender, FormClosedEventArgs e) => TreeWnd = null);
                content.MdiParent = this;
                content.Show(this.dockPanel, DockState.Document);
            }
            return (T?)obj;
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
            //listViewDevices.AutoResizeColumn(listViewDevices.Columns.Count - 1, ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void listViewDevices_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            /*int col = listViewDevices.Columns.Count - 1;
            if (col != e.ColumnIndex)
                listViewDevices.AutoResizeColumn(col, ColumnHeaderAutoResizeStyle.HeaderSize);*/
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

                // TODO pripadne nastaveni noveho InputPeriph pro zarizeni
            }

        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void Close_Click(object sender, EventArgs e)
        {
            // TODO pripadne zavreni karet zarizeni
            foreach (var x in tabControl.TabPages)
            {
                if (((TabPage)x).Controls[0].GetType().BaseType == typeof(UserControl))
                {
                    Debug.WriteLine($"{((TabPage)x).Controls[0].GetType().BaseType}");
                    Debug.WriteLine($"{((TabPage)x).Controls[0]}");
                }
            }



            /*var ucs = from x in DevicesTypeList where x.UserControls.Count > 0 select x;
            foreach (var item in ucs)
            {
                foreach (var u in item.UserControls)
                {
                    
                }

                //item.UserControls.ForEach(x => { x.Enabled = false; });
            }*/

            if (InputPeriph != null)
            {
                InputPeriph.Close();
                InputPeriph = null;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void InputPeriph_MessageReceived(Message message)
        {
            if (message.CMD == Command.Ident && message.Data.Count >= 14) // TODO && msg.RF)
            {
                Debug.WriteLine(message);

                byte state = message.Data[0];
                uint deviceID = (uint)(message.Data[1] << 24 | message.Data[2] << 16 | message.Data[3] << 8 | message.Data[4]);
                byte address = InputPeriph!.GetType() == typeof(UsbSerialNs.UsbSerial) ? message.Data[5] : address = message.SRC;
                string fwVer = $"{message.Data[6]}.{message.Data[7]}";
                string cpuId = $"{BitConverter.ToString(message.Data.Skip(8).Take(12).ToArray()).Replace("-", " ")}";

                NewIdent(deviceID, address, fwVer, cpuId, state);
            }
            else if ((message.CMD == Command.StartUpdate || message.CMD == Command.UpdateMsg) && message.Data.Count == 1)
            {
                MessageFlag = message.Data[0];
            }
            else if (message.CMD == Command.ParamRead)
            {
                if (message.Data[0] == 0)
                    NewParamData(MessageFlag, message.Data.ToArray());
                MessageReceived.Set();
            }
            else if (message.CMD == Command.GetListParam)
            {
                MessageFlag = message.Data[0];
                if (MessageFlag == 0)
                    NewParamItem(message.Data.ToArray());
                MessageReceived.Set();
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void NewParamData(byte ParamID, byte[] bytes)
        {
            try
            {
                Parameter parameter = (from xxx in parameters where xxx.ParameterID == ParamID select xxx).First();
                switch (parameter.ParType)
                {
                    case type.UInt8:
                        parameter.Value = bytes[1];
                        break;
                    case type.UInt16:
                        parameter.Value = BitConverter.ToUInt16(bytes, 1);
                        break;
                    case type.UInt32:
                        parameter.Value = BitConverter.ToUInt32(bytes, 1);
                        break;
                    case type.String:
                        parameter.Value = System.Text.Encoding.ASCII.GetString(bytes, 1, bytes.Length - 1);
                        break;
                    case type.IpAddr:
                        break;
                }

                listViewParameters.Invoke(delegate { parameter.listViewItem!.SubItems[listViewParameters.Columns.Count - 1].Text = $"{parameter.Value}"; });

                //Debug.WriteLine($"{parameter.ParName} - {parameter.ParType} - {parameter.Value:X} - {bytes.Length - 1}");
            }
            catch
            {

            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        enum type { UInt8, UInt16, UInt32, String, IpAddr };

        class Parameter
        {
            //internal byte Status;
            internal byte ParameterID;
            internal type ParType;
            internal bool ReadOnly;
            internal uint ParValMin;
            internal uint ParValMax;
            internal byte ParIndex;
            internal string? ParName;
            internal object? Value;
            internal ListViewItem? listViewItem = null;
        }

        List<Parameter> parameters = new List<Parameter>();

        private void NewParamItem(byte[] data)
        {
            Parameter parameter = new Parameter();

            //parameter.Status = data[0];
            parameter.ParameterID = data[1];
            parameter.ParType = (type)(data[2] & 0x7F);
            parameter.ReadOnly = (data[2] & 0x80) == 0x80;
            parameter.ParValMin = BitConverter.ToUInt32(data, 3);
            parameter.ParValMax = BitConverter.ToUInt32(data, 7);
            parameter.ParIndex = data[11];
            parameter.ParName = System.Text.Encoding.ASCII.GetString(data, 12, data.Length - 12);

            parameter.listViewItem = new ListViewItem($"{parameter.ParameterID}");
            parameter.listViewItem.SubItems.Add($"{parameter.ParType}");
            parameter.listViewItem.SubItems.Add($"{parameter.ReadOnly}");
            parameter.listViewItem.SubItems.Add($"{parameter.ParValMin}");
            parameter.listViewItem.SubItems.Add($"{parameter.ParValMax}");
            parameter.listViewItem.SubItems.Add($"{parameter.ParIndex}");
            parameter.listViewItem.SubItems.Add($"{parameter.ParName}");
            parameter.listViewItem.SubItems.Add($"");

            parameters.Add(parameter);
            listViewParameters.Invoke(delegate
            {
                var item = listViewParameters.Items.Add(parameter.listViewItem);
                item.Tag = parameter;
            });
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
                if (device.listViewItem != null)
                {
                    TreeWnd?.listViewDevices.Invoke(delegate
                    {
                        //device.listViewItem.SubItems[(int)(SubItem.CpuID)].Text = device.CpuId;
                        device.listViewItem.SubItems[(int)(DeviceSubItem.Address)].Text = device.AddressStr;
                        device.listViewItem.SubItems[(int)(DeviceSubItem.DevID)].Text = device.DevIdStr;
                        device.listViewItem.SubItems[(int)(DeviceSubItem.Name)].Text = device.Name;
                        device.listViewItem.SubItems[(int)(DeviceSubItem.Version)].Text = device.FwVer;
                    });
                }

                if (device.Equals(selectedDevice))
                {
                    this.Invoke(delegate
                    {
                        tb_address.Text = device.AddressStr;
                        tb_dev_id.Text = device.DevIdStr;
                        tb_version.Text = device.FwVer;
                        tb_cpu_id.Text = device.CpuId;
                        if (btn_update_active)
                        {
                            btn_update_active = false;
                            label_name.ForeColor = tb_address.ForeColor = tb_dev_id.ForeColor = tb_version.ForeColor = tb_cpu_id.ForeColor = Color.Green;
                            //tb_address.Font = tb_dev_id.Font = tb_version.Font = tb_cpu_id.Font = new Font(tb_address.Font, FontStyle.Bold);
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

                TreeWnd?.listViewDevices.Invoke(delegate
                {
                    TreeWnd.listViewDevices.Items.Add(device.listViewItem);

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
        public DeviceType GetDeviceType(uint deviceID)
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
                TreeWnd?.listViewDevices.Items.Clear();

                Task.Run(() =>
                {
                    Message message = new Message();
                    message.CMD = Command.Ident;
                    message.SRC = SrcAddress;

                    byte SearchFrom = 0x00;
                    byte SearchTo = 0xFE - 1;

                    if (InputPeriph!.GetType() == typeof(UsbSerialNs.UsbSerial))
                        SearchFrom = SearchTo = 0xFE;

                    Invoke(delegate
                    {
                        Cursor = Cursors.WaitCursor;
                        progressBar.Minimum = SearchFrom;
                        progressBar.Maximum = SearchTo;
                        if (progressBar.Minimum == progressBar.Maximum)
                            progressBar.Minimum--;
                        progressBar.Value = progressBar.Minimum;
                    });

                    // pošleme ident do všech zažízení
                    for (byte dest = SearchFrom; dest <= SearchTo; dest++)
                    {
                        progressBar.Invoke(delegate { progressBar.Value = dest; });
                        message.DEST = dest;
                        InputPeriph.SendMsg(message);
                        Task.Delay(3).Wait();
                    }

                    // Pockame na dobehnuti
                    Task.Delay(1500).ContinueWith(t =>
                    {
                        progressBar.Invoke(delegate
                        {
                            Cursor = Cursors.Default;
                            progressBar.Value = progressBar.Minimum;
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
                label_name.ForeColor = tb_address.ForeColor = tb_dev_id.ForeColor = tb_version.ForeColor = tb_cpu_id.ForeColor = Color.LightGray;
                //tb_address.Font = tb_dev_id.Font = tb_version.Font = tb_cpu_id.Font = new Font(tb_address.Font, FontStyle.Regular);
                Task.Delay(1000).ContinueWith(task =>
                {
                    Message message = new() { CMD = Command.Ident, DEST = selectedDevice.Address };
                    InputPeriph?.SendMsg(message);
                });
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedDevice != null && selectedDeviceType != null)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Encrypted binary files (*.bin)|*.bin|All files (*.*)|*.*";
                ofd.FileName = selectedDeviceType.FirmwarePath;
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                    
                selectedDeviceType.FirmwarePath = ofd.FileName;
                Debug.Assert(selectedDeviceType.FirmwarePath != null);


                progressBar.Minimum = 0;
                progressBar.Maximum = 100;
                progressBar.Value = 0;

                const int UpdateTmeout = 3000;
                byte[] sendbuf = new byte[260];

                MessageFlag = 0xFF;
                Message message = new Message() { CMD = Command.StartUpdate, DEST = selectedDevice.Address };
                InputPeriph?.SendMsg(message);

                int time = 10;
                while (time-- > 0)
                {
                    Task.Delay(100).Wait(); //Thread.Sleep(100);
                    Application.DoEvents();
                }

                System.IO.Stream fBin = System.IO.File.OpenRead(selectedDeviceType.FirmwarePath);

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

                        InputPeriph?.SendMsg(message);

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

        ///////////////////////////////////////////////////////////////////////////////////////////
        public void SetTabPage()
        {
            Debug.Assert(InputPeriph != null);

            var ucs = from x in DevicesTypeList where x.UserControls.Count > 0 select x;
            foreach (var item in ucs)
                item.UserControls.ForEach(x => { x.Enabled = false; });

            if (selectedDeviceType != null && File.Exists(selectedDeviceType.UserControl))
            {
                if (selectedDeviceType.UserControls.Count != 0)
                {
                    selectedDeviceType.UserControls.ForEach(x => { x.Enabled = true; });
                }
                else
                {
                    string assembly_name = Path.GetFullPath(selectedDeviceType.UserControl);
                    Assembly assembly = Assembly.LoadFile(Path.Combine("", assembly_name));
                    var types = assembly.GetTypes();
                    foreach (Type type in types)
                    {
                        if (type.IsVisible && type.IsAssignableTo(typeof(UserControl)))
                        {
                            UserControl? control = (UserControl?)Activator.CreateInstance(type, new object[] { MainApp });
                            if (control != null)
                            {
                                TabPage tp = new TabPage(control.Text) { Text = control.Text, Name = control.Name };
                                tp.Controls.Add(control);
                                tabControl.TabPages.Add(tp);
                                selectedDeviceType.UserControls.Add(control);
                            }
                        }
                    }
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void button1_Click(object sender, EventArgs e)
        {
            if (InputPeriph != null && selectedDevice != null)
            {
                Task.Run(() =>
                {
                    Invoke(delegate { Cursor = Cursors.WaitCursor; });

                    Message message = new Message();
                    message.CMD = Command.GetListParam;
                    message.SRC = SrcAddress;
                    message.DEST = selectedDevice.Address;
                    message.Data.Add(0);
                    MessageReceived.Reset();
                    InputPeriph.SendMsg(message);

                    message.Data[0] = 1;

                    while (true)
                    {
                        if (!MessageReceived.WaitOne(1000))
                            break;

                        if (MessageFlag != 0)
                            break;

                        MessageReceived.Reset();
                        InputPeriph.SendMsg(message);
                    }

                    message.CMD = Command.ParamRead;
                    foreach (var p in parameters)
                    {
                        MessageFlag = message.Data[0] = p.ParameterID;
                        MessageReceived.Reset();
                        InputPeriph.SendMsg(message);
                        if (!MessageReceived.WaitOne(1000))
                            continue;
                    }

                    progressBar.Invoke(delegate
                    {
                        progressBar.Value = 0;
                        Cursor = Cursors.Default;
                        listViewParameters.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    });
                });
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void listViewParameters_Resize(object sender, EventArgs e)
        {
            /*int width = 0;
            for (int i = 0; i < listViewParameters.Columns.Count; i++)
                width += listViewParameters.Columns[i].Width;
            width -= listViewParameters.Columns[listViewParameters.Columns.Count - 2].Width;
            listViewParameters.Columns[listViewParameters.Columns.Count - 2].Width = listViewParameters.Width - width;*/
            //listViewParameters.AutoResizeColumn(listViewParameters.Columns.Count - 2, ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void listViewParameters_SubItemClicked(object sender, SubItemEventArgs e)
        {
            if (e.SubItem == (int)ParmaterSubItem.Value && !((Parameter)e.Item.Tag).ReadOnly)
                listViewParameters.StartEditing(textBox1, e.Item, e.SubItem);
        }

        private void listViewParameters_SubItemEndEditing(object sender, SubItemEndEditingEventArgs e)
        {
            if ($"{((Parameter)e.Item.Tag).Value}" != e.DisplayText)
                e.Item.ForeColor = Color.Red;
            else
                e.Item.ForeColor = SystemColors.WindowText;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        internal void AppendToDebug(string text, bool bNewLine = true, bool bBolt = false, Color? color = null)
        {
            DebugWnd?.AppendText(text, bNewLine, bBolt, color);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
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

using CanDiag;
using DevConfig.Properties;
using DevConfig.Service;
using DevConfig.Utils;
using DevConfigSupp;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Timers;
using WeifenLuo.WinFormsUI.Docking;
using Message = CanDiagSupport.Message;

namespace DevConfig
{
    public partial class MainForm : Form
    {
        public const byte SrcAddress = 0x08;

        public DeviceForm? DeviceWnd = null;
        public DeviceTreeForm? TreeWnd = null;
        public DebugForm? DebugWnd = null;
        public RegisterForm? RegisterWnd = null;

        enum DeviceSubItem { Address, DevID, Name, Version, CpuID };
        enum ParmaterSubItem { ParamID, Type, RO, Min, Max, Index, Name, Value };

        public bool btn_update_active = false;

        internal List<DeviceType>? DevicesTypeList;
        internal List<Device> DevicesList = new();

        IMainApp MainApp;


        public delegate void CancelEventDelegate();
        public event CancelEventDelegate? AbortEvent;

        MruList<string>? ConnectMruList;

        ///////////////////////////////////////////////////////////////////////////////////////////
        public MainForm()
        {
            MainApp = new MainAppClass(this);
            InitializeComponent();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void MainForm_Load(object sender, EventArgs e)
        {
            ConnectMruList = new MruList<string>(Assembly.GetExecutingAssembly().GetName().Name ?? "DevConfig", connectionToolStripMenuItem, closeToolStripMenuItem, 6);
            ConnectMruList.FileSelected += OpenFile;
            /*var enc = CodePagesEncodingProvider.Instance.GetEncoding(852);
            CultureInfo ci = new CultureInfo("cs-CZ");
            foreach (EncodingInfo ei in Encoding.GetEncodings())
            {
                Encoding E2 = ei.GetEncoding();

                //if ((ei.Name != E2.BodyName) || (ei.Name != E2.HeaderName) || (ei.Name != E2.WebName))
                {
                    Debug.Write("{0,-18} ", ei.Name);
                    Debug.Write("{0,-9}  ", E2.CodePage.ToString());
                    Debug.Write("{0,-18} ", E2.BodyName);
                    Debug.Write("{0,-18} ", E2.HeaderName);
                    Debug.Write("{0,-18} ", E2.WebName);
                    Debug.WriteLine("{0} ", E2.EncodingName);
                }
            }
            //message.Data.AddRange(Encoding.UTF7.GetBytes(path + "\0"));            // dir name
            //var a = System.Text.Encoding.GetEncoding("cp852");
            */
            DevicesTypeList = JsonSerializer.Deserialize<List<DeviceType>>(File.ReadAllText(@"Resources\Devices.json"));
            string[] BLPaths = Settings.Default.BLPath.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            for (int i = 0; i < BLPaths.Length; i += 2)
            {
                uint devid = Convert.ToUInt32(BLPaths[i]);
                DeviceType t = GetDeviceType(devid);
                t.FirmwarePath = BLPaths[i + 1];
            }

            TreeWnd = CreateChild<DeviceTreeForm>("Device tree");
            TreeWnd!.DockState = DockState.DockLeft;

            DebugWnd = CreateChild<DebugForm>("Debug");
            DebugWnd!.DockState = DockState.DockLeft;

            DeviceWnd = CreateChild<DeviceForm>("Device");

            RegisterWnd = CreateChild<RegisterForm>("Registers");

            DeviceWnd!.BringToFront();

        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        T? CreateChild<T>(string Text)
        {
            Cursor = Cursors.WaitCursor;
            Type t = typeof(T);
            var obj = Activator.CreateInstance(t);
            if (obj != null)
            {
                DockContent content = (DockContent)obj;
                content.Text = Text;
                content.Tag = Text;
                content.MdiParent = this;
                content.FormClosed += ((object? sender, FormClosedEventArgs e) => obj = null);
                content.MdiParent = this;
                content.CloseButtonVisible = false; // TODO
                content.Show(this.dockPanel, DockState.Document);
            }
            Cursor = Cursors.Default;
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
        private void Open_Click(object sender, EventArgs e)
        {
            if (DevConfigService.Instance.ProcessLock())
            {
                ConnectForm connectForm = new ConnectForm();

                if (connectForm.ShowDialog() == DialogResult.Continue)
                {
                    Text = $"DevConfig - {DevConfigService.Instance.ConnectString}";
                    ConnectMruList?.AddFile(DevConfigService.Instance.ConnectString);

                    //TreeWnd?.NewInputPeriph();
                    //DebugWnd?.NewInputPeriph();
                    //DeviceWnd?.NewInputPeriph();
                    DevConfigService.Instance.RefreshDeviceList();
                }
                else
                {
                    DevConfigService.Instance.FreeProcessLock();
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void Close_Click(object sender, EventArgs e)
        {
            if (DevConfigService.Instance.ProcessLock())
            {
                if (DevConfigService.Instance.InputPeriph != null)
                {
                    DevConfigService.Instance.InputPeriph.Close();
                    DevConfigService.Instance.InputPeriph = null;
                    Text = "DevConfig";
                }
                DevConfigService.Instance.FreeProcessLock();
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void Exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void OpenFile(string file_name)
        {
            if (DevConfigService.Instance.ProcessLock())
            {
                if (DevConfigService.Instance.Open(file_name))
                {
                    Text = $"DevConfig - {DevConfigService.Instance.ConnectString}";
                    ConnectMruList?.AddFile(DevConfigService.Instance.ConnectString);
                    DevConfigService.Instance.RefreshDeviceList();
                }
                else
                {
                    ConnectMruList?.RemoveFile(file_name);
                    DevConfigService.Instance.FreeProcessLock();
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public void NewIdent(uint deviceID, byte address, string fwVer, string cpuId, byte state)
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

                if (device.Equals(DevConfigService.Instance.selectedDevice))
                {
                    this.Invoke(delegate
                    {
                        tb_address.Text = device.AddressStr;
                        tb_dev_id.Text = device.DevIdStr;
                        tb_version.Text = device.FwVer;
                        if (btn_update_active)
                        {
                            btn_update_active = false;
                            label_name.ForeColor = tb_address.ForeColor = tb_dev_id.ForeColor = tb_version.ForeColor = Color.Green;
                            if (DeviceWnd != null)
                            {
                                DeviceWnd.tb_address.Text = device.AddressStr;
                                DeviceWnd.tb_dev_id.Text = device.DevIdStr;
                                DeviceWnd.tb_version.Text = device.FwVer;

                                DeviceWnd.label_name.ForeColor =
                                DeviceWnd.tb_address.ForeColor = DeviceWnd.tb_dev_id.ForeColor = DeviceWnd.tb_version.ForeColor = DeviceWnd.tb_cpu_id.ForeColor = Color.Green;
                                DeviceWnd.tb_address.Font = DeviceWnd.tb_dev_id.Font = DeviceWnd.tb_version.Font = DeviceWnd.tb_cpu_id.Font = new Font(DeviceWnd.tb_address.Font, FontStyle.Bold);
                            }
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
        public string GetDeviceName(byte address, uint deviceID)
        {
            return GetDeviceType(deviceID).Name;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void RefreshList_Click(object sender, EventArgs e)
        {
            if (DevConfigService.Instance.ProcessLock())
                DevConfigService.Instance.RefreshDeviceList();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public void SetTabPage()
        {
            Debug.Assert(DevConfigService.Instance.InputPeriph != null);

            var ucs = from x in DevicesTypeList where x.UserControlsList.Count > 0 select x;
            foreach (var item in ucs)
                item.UserControlsList.ForEach(x => { x.Enabled = false; });

            if (DevConfigService.Instance.selectedDeviceType != null && File.Exists(DevConfigService.Instance.selectedDeviceType.UserControl))
            {
                if (DevConfigService.Instance.selectedDeviceType.UserControlsList.Count != 0)
                {
                    DevConfigService.Instance.selectedDeviceType.UserControlsList.ForEach(x =>
                    {
                        x.Enabled = true;
                        x.BringToFront();
                    });
                }
                else
                {
                    string assembly_name = Path.GetFullPath(DevConfigService.Instance.selectedDeviceType.UserControl);
                    Assembly assembly = Assembly.LoadFile(Path.Combine("", assembly_name));
                    var types = assembly.GetTypes();
                    foreach (Type type in types)
                    {
                        if (type.IsVisible && type.IsAssignableTo(typeof(DockContent)))
                        {
                            DockContentEx? control = (DockContentEx?)Activator.CreateInstance(type);
                            if (control != null)
                            {
                                control.SetMainApp(MainApp);

                                control.MdiParent = this;
                                control.FormClosed += ((object? sender, FormClosedEventArgs e) =>
                                {
                                    var ucs = from x in DevicesTypeList where x.UserControlsList.Contains(control) select x;
                                    foreach (var x in ucs)
                                        x.UserControlsList.Remove(control);
                                    control = null;
                                });
                                control.MdiParent = this;
                                control.Show(this.dockPanel, DockState.Document);
                                DevConfigService.Instance.selectedDeviceType.UserControlsList.Add(control);
                            }
                        }
                    }
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        internal void AppendToDebugIf(bool bCond, string text, bool bNewLine = true, bool bBolt = false, Color? color = null)
        {
            if (bCond)
                AppendToDebug(text, bNewLine, bBolt, color);
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        internal void AppendToDebug(string text, bool bNewLine = true, bool bBolt = false, Color? color = null)
        {
            Debug.WriteLine(text);
            DebugWnd?.AppendText(text, bNewLine, bBolt, color);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        internal void SelectItem(Device tag)
        {
            DevConfigService.Instance.selectedDevice = tag;
            DevConfigService.Instance.selectedDeviceType = GetDeviceType(DevConfigService.Instance.selectedDevice.DevId);
            Debug.WriteLine($"Selected DevID = {DevConfigService.Instance.selectedDevice.DevId:X}");
            tb_address.Text = DevConfigService.Instance.selectedDevice.AddressStr;
            tb_dev_id.Text = DevConfigService.Instance.selectedDevice.DevIdStr;
            tb_version.Text = DevConfigService.Instance.selectedDevice.FwVer;
            label_name.Text = DevConfigService.Instance.selectedDevice.Name;
            label_name.ForeColor = tb_address.ForeColor = tb_dev_id.ForeColor = tb_version.ForeColor = SystemColors.WindowText;

            SetTabPage();

            if (DeviceWnd != null)
            {
                DeviceWnd.label_name.Text = DevConfigService.Instance.selectedDevice.Name;
                DeviceWnd.tbFwFileName.Text = DevConfigService.Instance.selectedDeviceType.FirmwarePath;
                DeviceWnd.tb_address.Text = DevConfigService.Instance.selectedDevice.AddressStr;
                DeviceWnd.tb_cpu_id.Text = DevConfigService.Instance.selectedDevice.CpuId;
                DeviceWnd.tb_dev_id.Text = DevConfigService.Instance.selectedDevice.DevIdStr;
                DeviceWnd.tb_version.Text = DevConfigService.Instance.selectedDevice.FwVer;
                DeviceWnd.label_name.ForeColor = DeviceWnd.tb_address.ForeColor = DeviceWnd.tb_dev_id.ForeColor = DeviceWnd.tb_version.ForeColor = DeviceWnd.tb_cpu_id.ForeColor = SystemColors.WindowText;
                DeviceWnd.tb_address.Font = DeviceWnd.tb_dev_id.Font = DeviceWnd.tb_version.Font = DeviceWnd.tb_cpu_id.Font = new Font(DeviceWnd.tb_address.Font, FontStyle.Regular);
            }

            if (RegisterWnd != null)
            {
                if (DevConfigService.Instance.selectedDevice.Parameters == null)
                    DevConfigService.Instance.GetRegisterFromDevice();
                RegisterWnd.UpdateList();
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void btnAbort_Click(object sender, EventArgs e)
        {
            AbortEvent?.Invoke();

        }

        #region PROGRESS
        ///////////////////////////////////////////////////////////////////////////////////////////
        public int ProgressBar_Minimum
        {
            set
            {
                tsProgressBar.Minimum = value;
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        public int ProgressBar_Maximum
        {
            set
            {
                if (tsProgressBar.Minimum >= value)
                    tsProgressBar.Minimum = tsProgressBar.Maximum - 1;
                tsProgressBar.Maximum = value;
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        public int ProgressBar_Value
        {
            get { return tsProgressBar.Value; }
            set
            {
                if (tsProgressBar.Maximum < value)
                    tsProgressBar.Value = tsProgressBar.Maximum;
                else if (tsProgressBar.Minimum > value)
                    tsProgressBar.Value = tsProgressBar.Minimum;
                else
                    tsProgressBar.Value = value;
            }
        }

        public void ProgressBar_Step(int value)
        {
            tsProgressBar.Increment(value);
        }

        #endregion

        #region SD CARD Command
        private void SDCardOpen_Click(object sender, EventArgs e)
        {
            if (DevConfigService.Instance.selectedDevice == null)
            {
                MessageBox.Show("First you need to select device.", "DevConfig - info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if (DevConfigService.Instance.ProcessLock())
                    CreateChild<SDCardCtrl>($"SD Card (0x{DevConfigService.Instance.selectedDevice!.Address:X2})");
            }
        }

        private void SDCardBackup_Click(object sender, EventArgs e)
        {
            if (TryGetSDCardCtrl(out SDCardCtrl SDCardCtrlWnd))
                SDCardCtrlWnd.BackupSdCardMenuItem_Click(sender, e);
        }

        private void SDCardRestore_Click(object sender, EventArgs e)
        {
            if (TryGetSDCardCtrl(out SDCardCtrl SDCardCtrlWnd))
                SDCardCtrlWnd.RestoreSdCardMenuItem_Click(sender, e);
        }

        private void SDCardReload_Click(object sender, EventArgs e)
        {
            if (TryGetSDCardCtrl(out SDCardCtrl SDCardCtrlWnd))
                SDCardCtrlWnd.ReloadSdCardMenuItem_Click(sender, e);
        }

        private void SDCardFormat_Click(object sender, EventArgs e)
        {
            if (TryGetSDCardCtrl(out SDCardCtrl SDCardCtrlWnd))
                SDCardCtrlWnd.FormatSdCardMenuItem_Click(sender, e);
        }

        private void SDCardDirectoryInsert_Click(object sender, EventArgs e)
        {
            if (TryGetSDCardCtrl(out SDCardCtrl SDCardCtrlWnd))
                SDCardCtrlWnd.AddDirMenuItem_Click(sender, e);
        }

        private void SDCardDirectoryRename_Click(object sender, EventArgs e)
        {
            if (TryGetSDCardCtrl(out SDCardCtrl SDCardCtrlWnd))
                SDCardCtrlWnd.RenDirMenuItem_Click(sender, e);
        }

        private void SDCardDirectoryDelete_Click(object sender, EventArgs e)
        {
            if (TryGetSDCardCtrl(out SDCardCtrl SDCardCtrlWnd))
                SDCardCtrlWnd.DelDirMenuItem_Click(sender, e);
        }

        private void SDCardFileGet_Click(object sender, EventArgs e)
        {
            if (TryGetSDCardCtrl(out SDCardCtrl SDCardCtrlWnd))
                SDCardCtrlWnd.GetFileMenuItem_Click(sender, e);
        }

        private void SDCardFileInsert_Click(object sender, EventArgs e)
        {
            if (TryGetSDCardCtrl(out SDCardCtrl SDCardCtrlWnd))
                SDCardCtrlWnd.AddFileMenuItem_Click(sender, e);
        }

        private void SDCardFileRename_Click(object sender, EventArgs e)
        {
            if (TryGetSDCardCtrl(out SDCardCtrl SDCardCtrlWnd))
                SDCardCtrlWnd.RenFileMenuItem_Click(sender, e);
        }

        private void SDCardFileDelete_Click(object sender, EventArgs e)
        {
            if (TryGetSDCardCtrl(out SDCardCtrl SDCardCtrlWnd))
                SDCardCtrlWnd.DelFileMenuItem_Click(sender, e);
        }

        private bool TryGetSDCardCtrl(out SDCardCtrl SDCardCtrlWnd)
        {
            if (dockPanel.ActiveContent != null && dockPanel.ActiveContent.GetType() == typeof(SDCardCtrl))
            {
                SDCardCtrlWnd = (SDCardCtrl)dockPanel.ActiveContent;
                return true;
            }
            else
            {
                MessageBox.Show("SD card window is not selected.", "DevConfig - info", MessageBoxButtons.OK, MessageBoxIcon.Information);
#pragma warning disable CS8625 // Literál null nejde pøevést na odkazový typ, který nemùže mít hodnotu null.
                SDCardCtrlWnd = default;
#pragma warning restore CS8625 // Literál null nejde pøevést na odkazový typ, který nemùže mít hodnotu null.
                return false;
            }
        }

        #endregion

        #region Register Command
        private void RegisterReloadSelected_Click(object sender, EventArgs e)
        {
            if (DevConfigService.Instance.selectedDevice != null)
            {
                DevConfigService.Instance.selectedDevice.Parameters = null;
                DevConfigService.Instance.GetRegisterFromDevice();
                RegisterWnd?.UpdateList();
            }
        }

        private void saveConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*if (DevConfigService.Instance.selectedDevice != null && DevConfigService.Instance.selectedDevice.Parameters != null)
            {
                JsonSerializerOptions options = new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } };
                //var json = JsonSerializer.Deserialize<List<ParamConfig>>(File.ReadAllText(file_name), options);

                string a = JsonSerializer.Serialize<List<Parameter>>(DevConfigService.Instance.selectedDevice.Parameters, options);

                Debug.WriteLine(a);
            }*/
        }

        private void RegisterSaveSelected_Click(object sender, EventArgs e)
        {

        }

        private void RegisterSaveAll_Click(object sender, EventArgs e)
        {

        }
        #endregion

    }

    ////////////////////////////////////////////////////////////////////////////
    internal class SimpleUnloadableAssemblyLoadContext : AssemblyLoadContext
    {
        //public SimpleUnloadableAssemblyLoadContext() : base(true) { }
        //protected override Assembly Load(AssemblyName assemblyName) { return null; }
    }
}

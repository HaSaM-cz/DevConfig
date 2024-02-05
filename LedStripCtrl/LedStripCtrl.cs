using CanDiagSupport;
using RGB_config;
using System.Diagnostics;
using Message = CanDiagSupport.Message;

namespace LedStripCtrl
{
    public partial class LedStripCtrl : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        IMainApp MainApp;
        IInputPeriph? InputPeriph;

        byte MsgFlag = 0;
        int progerss_val = 0;

        ///////////////////////////////////////////////////////////////////////////////////////////
        cGlobals globals = new cGlobals();

        ///////////////////////////////////////////////////////////////////////////////////////////
        public LedStripCtrl(IMainApp main_app)
        {
            Text = "RGB config";
            MainApp = main_app;
            InitializeComponent();

            cGlobals.MainTreeView = this.treeView1;
            cGlobals.RefreshMainTreeView();

            if (MainApp.inputPeriph != null)
            {
                InputPeriph = MainApp.inputPeriph;
                InputPeriph.MessageReceived += InputPeriph_MessageReceived;
            }

        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void Send_command(cmds command, byte dlen, byte[] dta, int timeout = 0)
        {
            MainApp.SetProperty("ProgressValue", ++progerss_val);
            Message message = new Message();
            message.DEST = (byte)(MainApp.GetProperty("SelectedDeviceCanID") ?? 0);
            message.CMD = (byte)command;
            message.Data = dta.Take(dlen).ToList();

            if (MainApp.inputPeriph != null && !MainApp.inputPeriph.Equals(InputPeriph))
            {
                InputPeriph = MainApp.inputPeriph;
                InputPeriph.MessageReceived += InputPeriph_MessageReceived;
            }
            MainApp.inputPeriph?.SendMsg(message);
            if (timeout > 0)
            {
                Task.Delay(timeout);
                Application.DoEvents();
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void InputPeriph_MessageReceived(Message msg)
        {
            switch ((cmds)msg.CMD)
            {
                case cmds.teCmd_WritePar:
                    if (msg.Data[0] != 0)
                        MsgFlag = msg.Data[0];
                    else if (msg.Data.Count < 2)
                        MsgFlag = msg.Data[0];
                    else
                        MsgFlag = msg.Data[1];
                    break;
                default:
                    break;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void btnOpenXmlFile_Click(object sender, EventArgs e)
        {
            globals.OpenXmlFile();
            cGlobals.RefreshMainTreeView();
            //SetTabControlData();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void btnLoadXmlToRGB_Click(object sender, EventArgs e)
        {
            progerss_val = 0;

            var total_substates = (from xx in cGlobals.listStates select xx.list_SubStates.Count).Sum();
            int progress_max = cGlobals.listModes.Count + total_substates + cGlobals.listDevices.Count * 11 + 2;
            MainApp.SetProperty("ProgressMin", 0);
            MainApp.SetProperty("ProgressMax", progress_max);
            MainApp.SetProperty("ProgressValue", 0);
            Debug.WriteLine($"Modes = {cGlobals.listModes.Count}, States = {cGlobals.listStates.Count}, Devices = {cGlobals.listDevices.Count}, sum = {progress_max}");

            byte[] dta = new byte[255];
            byte dlen = 0;

            AppendToDebug("Clear config");
            Application.DoEvents();
            int time = 20;

            // write Clear RGB ----------------------------------------------------------------------------
            dlen = 0;
            dta[dlen++] = (byte)eParam.paClearRGB;
            MsgFlag = 0xFF;
            Send_command(cmds.teCmd_WritePar, dlen, dta);
            time = 20;

            byte destination = (byte)MainApp.GetProperty("SelectedDeviceCanID");// cGlobals.StringToByte(txtAddress.Text);
            if (destination == 255)
            {
                time = 10;
            }

            while (time > 0)
            {
                if (MsgFlag != 0xFF)
                {
                    break;
                }
                time--;
                Task.Delay(100).Wait(); //Thread.Sleep(100);
                Application.DoEvents();
            }

            if (destination == 255)
            {

            }
            else if (time == 0)
            {
                MessageBox.Show("Clear config - No response");
                return;
            }
            else if (MsgFlag != 0)
            {
                MessageBox.Show("Clear config - Error: " + MsgFlag.ToString());
                return;
            }

            // write pixels offset ----------------------------------------------------------------------------
            AppendToDebug("Write pixels config");
            dlen = 0;
            foreach (cGlobals.cDeviceConfig device in cGlobals.listDevices)
            {
                dta[dlen++] = (byte)eParam.paPixelOffset;
                dta[dlen++] = device.address;
                dta[dlen++] = (byte)(device.line1_Offset & 0xFF);
                dta[dlen++] = (byte)(device.line1_Offset >> 8);
                dta[dlen++] = (byte)(device.line2_Offset & 0xFF);
                dta[dlen++] = (byte)(device.line2_Offset >> 8);
            }
            MsgFlag = 0xFF;
            //Debug.WriteLine("SEND 0");
            Send_command(cmds.teCmd_WritePar, dlen, dta);
            time = 20;
            if (destination == 255)
            {
                time = 1;
            }
            while (time > 0)
            {
                if (MsgFlag != 0xFF)
                {
                    break;
                }
                time--;
                Task.Delay(100).Wait(); //Thread.Sleep(100);
                Application.DoEvents();
            }

            //Debug.WriteLine("TIMEOUT 0");
            if (destination == 255)
            {

            }
            else if (time == 0)
            {
                MessageBox.Show("Write pixels - No response");
                return;
            }
            else if (MsgFlag != 0)
            {
                MessageBox.Show("Write pixels - Error: " + MsgFlag.ToString());
                return;
            }

            // write modes ----------------------------------------------------------------------------
            AppendToDebug("Write modes config");

            byte counter = 0;

            foreach (cGlobals.cModesConfig mode in cGlobals.listModes)
            {
                dlen = 0;
                dta[dlen++] = (byte)eParam.paMode;
                dta[dlen++] = (byte)(mode.id);
                dta[dlen++] = (byte)(mode.auxiliary & 0xFF);
                dta[dlen++] = (byte)(mode.auxiliary >> 8);
                dta[dlen++] = (byte)(mode.auxiliary >> 16);
                dta[dlen++] = (byte)(mode.auxiliary >> 24);
                dta[dlen++] = (byte)(mode.auxiliary >> 32);
                dta[dlen++] = (byte)(mode.auxiliary >> 40);
                dta[dlen++] = (byte)(mode.auxiliary >> 48);
                dta[dlen++] = (byte)(mode.auxiliary >> 56);
                dta[dlen++] = (byte)(mode.mode);
                dta[dlen++] = (byte)(mode.step_time1ms & 0xFF);
                dta[dlen++] = (byte)(mode.step_time1ms >> 8);
                dta[dlen++] = (byte)(mode.max_RED);
                dta[dlen++] = (byte)(mode.min_RED);
                dta[dlen++] = (byte)(mode.max_GREEN);
                dta[dlen++] = (byte)(mode.min_GREEN);
                dta[dlen++] = (byte)(mode.max_BLUE);
                dta[dlen++] = (byte)(mode.min_BLUE);
                MsgFlag = 0xFF;
                //Debug.WriteLine("SEND 1");
                Send_command(cmds.teCmd_WritePar, dlen, dta);
                time = 20;
                if (destination == 255)
                {
                    time = 1;
                }
                while (time > 0)
                {
                    if (MsgFlag != 0xFF)
                    {
                        break;
                    }
                    time--;
                    Task.Delay(100).Wait();  //Thread.Sleep(100);
                    Application.DoEvents();
                }

                //Debug.WriteLine("TIMEOUT 1");
                if (destination == 255)
                {

                }
                else if (time == 0)
                {
                    MessageBox.Show(counter.ToString() + "Write modes - No response");
                    return;
                }
                else if (MsgFlag != 0)
                {
                    MessageBox.Show(counter.ToString() + "Write modes - Error: " + MsgFlag.ToString());
                    return;
                }
                counter++;
            }

            // write states ----------------------------------------------------------------------------
            AppendToDebug("Write states config");
            counter = 0;

            foreach (cGlobals.cStatesConfig state in cGlobals.listStates)
            {
                foreach (cGlobals.cSubStatesConfig substate in state.list_SubStates)
                {
                    dlen = 0;
                    dta[dlen++] = (byte)eParam.paState;
                    dta[dlen++] = (byte)(state.stateID);
                    dta[dlen++] = (byte)(substate.substateID);                              // here shoud be subsate
                    dta[dlen++] = (byte)(substate.SyncStateEnable);
                    dta[dlen++] = (byte)(substate.SyncStateTimer);
                    dta[dlen++] = (byte)(substate.list_segment.Count);

                    foreach (cGlobals.ts_segments segment in substate.list_segment)
                    {
                        dta[dlen++] = (byte)(segment.pixels & 0xFF);
                        dta[dlen++] = (byte)(segment.pixels >> 8);
                        dta[dlen++] = (byte)(segment.order_data.Count);
                        foreach (cGlobals.ts_order_data order in segment.order_data)
                        {
                            dta[dlen++] = (byte)(order.order_time & 0xFF);
                            dta[dlen++] = (byte)(order.order_time >> 8);
                            dta[dlen++] = (byte)(order.order_repeat);
                            dta[dlen++] = (byte)(order.order_mode);
                        }
                    }

                    time = 20;
                    MsgFlag = 0xFF;
                    Send_command(cmds.teCmd_WritePar, dlen, dta);
                    if (destination == 255)
                    {
                        time = 1;
                    }
                    while (time > 0)
                    {
                        if (MsgFlag != 0xFF)
                        {
                            break;
                        }
                        time--;
                        Task.Delay(100).Wait(); //Thread.Sleep(100);
                        Application.DoEvents();
                    }

                    if (destination == 255)
                    {

                    }
                    else if (time == 0)
                    {
                        MessageBox.Show(counter.ToString() + "Write states - No response");
                        return;
                    }
                    else if (MsgFlag != 0)
                    {
                        MessageBox.Show(counter.ToString() + "Write states - Error: " + MsgFlag.ToString());
                        return;
                    }
                }
            }

            AppendToDebug("Write pixels map config");
            foreach (cGlobals.cDeviceConfig device in cGlobals.listDevices)
            {
                //--- send map 0 ----------------------------------------------------
                dlen = 0;
                dta[dlen++] = (byte)eParam.paPixelsMapInit;
                dta[dlen++] = device.address;
                dta[dlen++] = 0;
                Send_command(cmds.teCmd_WritePar, dlen, dta, 100);

                dlen = 0;
                dta[dlen++] = (byte)eParam.paPixelsMapData;
                dlen += 250;

                Buffer.BlockCopy(device.map0, 0, dta, 1, 250);
                Send_command(cmds.teCmd_WritePar, dlen, dta, 100);

                Buffer.BlockCopy(device.map0, 250, dta, 1, 250);
                Send_command(cmds.teCmd_WritePar, dlen, dta, 100);

                Buffer.BlockCopy(device.map0, 500, dta, 1, 250);
                Send_command(cmds.teCmd_WritePar, dlen, dta, 100);

                Buffer.BlockCopy(device.map0, 750, dta, 1, 250);
                Send_command(cmds.teCmd_WritePar, dlen, dta, 100);

                //--- send map 1 ----------------------------------------------------
                dlen = 0;
                dta[dlen++] = (byte)eParam.paPixelsMapInit;
                dta[dlen++] = device.address;
                dta[dlen++] = 1;
                Send_command(cmds.teCmd_WritePar, dlen, dta, 100);

                dlen = 0;
                dta[dlen++] = (byte)eParam.paPixelsMapData;
                dlen += 250;

                Buffer.BlockCopy(device.map1, 0, dta, 1, 250);
                Send_command(cmds.teCmd_WritePar, dlen, dta, 100);

                Buffer.BlockCopy(device.map1, 250, dta, 1, 250);
                Send_command(cmds.teCmd_WritePar, dlen, dta, 100);

                Buffer.BlockCopy(device.map1, 500, dta, 1, 250);
                Send_command(cmds.teCmd_WritePar, dlen, dta, 100);

                Buffer.BlockCopy(device.map1, 750, dta, 1, 250);
                Send_command(cmds.teCmd_WritePar, dlen, dta, 100);

                dlen = 0;
                dta[dlen++] = (byte)eParam.paSaveRGB;
                Send_command(cmds.teCmd_WritePar, dlen, dta, 100);
            }
            AppendToDebug("Write config OK");
            MainApp.SetProperty("ProgressValue", 0);
        }

        private void btnSaveXMLFile_Click(object sender, EventArgs e)
        {

        }
        private void AppendToDebug(string v)
        {
            MainApp.SetProperty("AppendToDebug", v);
        }
    }
}

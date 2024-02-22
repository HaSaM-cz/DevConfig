using CanDiagSupport;
using SshCANns;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using TcpTunelNs;
using ToolStickNs;
using UsbSerialNs;

using Message = CanDiagSupport.Message;

namespace DevConfig.Service
{
    public class DevConfigService
    {
        internal MainForm MainForm;
        internal Device? selectedDevice = null;
        internal DeviceType? selectedDeviceType = null;
        private int process_lock = 0; // Kontrola pracujícího procesu. Povolení pouze jedné úlohy.
        ///////////////////////////////////////////////////////////////////////////////////////////

        private bool bContinue = true;
        private byte MessageFlag = 0;
        private Message? message = null;
        private readonly ManualResetEvent sync_obj = new(false);
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////
        public enum UpdateEnumFlags
        {
            RespOK = 0x00,
            ERR_FlashErase = 0x01,
            ERR_FlashProgram = 0x02,
            ERR_TooMachData = 0x03,
            ERR_AES_CRC = 0x04,
            ERR_Head_CRC = 0x05,
            ERR_WrongDevID = 0x06,
            ERR_BL_Run = 0x07,
            ERR_MsgLen = 0x08,
            ERR_NotInit = 0x09,
        }
        ///////////////////////////////////////////////////////////////////////////////////////////

        IInputPeriph? _InputPeriph = null;
        public IInputPeriph? InputPeriph
        {
            get { return _InputPeriph; }
            set
            {
                _InputPeriph = value;
                if(_InputPeriph != null)
                    _InputPeriph.MessageReceived += _InputPeriph_MessageReceived;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private static DevConfigService? instance = null;
        internal string ConnectString = string.Empty;

        public static DevConfigService Instance
        {
            get
            {
                if (instance == null)
                    instance = new DevConfigService();
                return instance;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        DevConfigService()
        {
            MainForm = (MainForm)Application.OpenForms["MainForm"];
            MainForm.AbortEvent += MainForm_AbortEvent;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void MainForm_AbortEvent()
        {
            bContinue = false;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void _InputPeriph_MessageReceived(Message msg)
        {
            if (msg.CMD == Command.Ident && msg.Data.Count >= 14) // TODO && msg.RF)
            {
                //Debug.WriteLine(message);
                byte state = msg.Data[0];
                uint deviceID = (uint)(msg.Data[1] << 24 | msg.Data[2] << 16 | msg.Data[3] << 8 | msg.Data[4]);

                byte address = DevConfigService.Instance.InputPeriph!.GetType() == typeof(UsbSerialNs.UsbSerial) ? msg.Data[5] : address = msg.SRC;
                //byte address = message.Data[5]; Usb Serial 
                //byte address = message.SRC; // Toolstick
                //byte address = message.SRC; // Karo SSH
                //byte address = message.Data[5]; // TS TCP tunel \
                //byte address = message.SRC; // Karo TCP tunel /

                string fwVer = $"{msg.Data[6]}.{msg.Data[7]}";
                string cpuId = $"{BitConverter.ToString(msg.Data.Skip(8).Take(12).ToArray()).Replace("-", " ")}";

                MainForm.NewIdent(deviceID, address, fwVer, cpuId, state);
            }
            else if ((msg.CMD == Command.StartUpdate || msg.CMD == Command.UpdateMsg) && msg.Data.Count == 1)
            {
                //Debug.WriteLine(message);
                MessageFlag = msg.Data[0];
                sync_obj.Set();
            }
            else if (msg.CMD == Command.GetListParam || 
                     msg.CMD == Command.ParamRead)
            {
                if (msg.Data.Count >= 1)
                {
                    message = msg;
                    MessageFlag = msg.Data[0];
                    sync_obj.Set();
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        internal void RefreshDeviceList()
        {
            if (InputPeriph != null)
            {
                bool usb_serial = false;
                selectedDevice = null;
                MainForm.DevicesList.Clear();
                MainForm.TreeWnd?.listViewDevices.Items.Clear();

                Task.Run(() =>
                {
                    bContinue = true;
                    Message message = new Message();
                    message.CMD = Command.Ident;
                    message.SRC = MainForm.SrcAddress;

                    byte SearchFrom = 0x00;
                    byte SearchTo = 0xFE - 1;

                    if (InputPeriph!.GetType() == typeof(UsbSerialNs.UsbSerial))
                    {
                        usb_serial = true;
                        SearchFrom = SearchTo = 0xFE;
                    }
                    else
                    {
                        MainForm.Invoke(delegate
                        {
                            MainForm.Cursor = Cursors.WaitCursor;
                            MainForm.ProgressBar_Minimum = SearchFrom;
                            MainForm.ProgressBar_Maximum = SearchTo;
                            MainForm.ProgressBar_Value = SearchFrom;
                        });
                    }

                    // pošleme ident do všech zažízení
                    for (byte dest = SearchFrom; dest <= SearchTo && bContinue; dest++)
                    {
                        if (!usb_serial)
                            MainForm.Invoke(delegate { MainForm.ProgressBar_Value = dest; });
                        message.DEST = dest;
                        InputPeriph.SendMsg(message);
                        Task.Delay(3).Wait();
                    }

                    // Pockame na dobehnuti
                    if (!usb_serial)
                    {
                        Task.Delay(bContinue ? 1500 : 0).ContinueWith(t =>
                        {
                            MainForm.Invoke(delegate
                            {
                                MainForm.Cursor = Cursors.Default;
                                MainForm.ProgressBar_Value = SearchFrom;
                            });
                        });
                    }
                    FreeProcessLock();
                });
            }
            else
                FreeProcessLock();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        internal void UpdateFw(string file_name)
        {
            Debug.Assert(selectedDevice != null);

            Task.Run(() =>
            {
                if (DevConfigService.Instance.ProcessLock())
                {
                    bContinue = true;

                    MainForm.AppendToDebug("Uploading FW");

                    byte[] data = new byte[240];
                    FileStream file = File.OpenRead(file_name);

                    MainForm.Invoke(delegate
                    {
                        MainForm.Cursor = Cursors.WaitCursor;
                        MainForm.ProgressBar_Minimum = 0;
                        MainForm.ProgressBar_Maximum = (int)file.Length;
                        MainForm.ProgressBar_Value = 0;
                    });

                    Message message = new Message() { CMD = Command.StartUpdate, SRC = MainForm.SrcAddress, DEST = selectedDevice.Address };
                    sync_obj.Reset();
                    //Debug.WriteLine(message);
                    InputPeriph?.SendMsg(message);
                    MainForm.AppendToDebug("*", false);

                    message.CMD = Command.UpdateMsg;

                    while (bContinue)
                    {
                        if (sync_obj.WaitOne(3000))
                        {
                            MainForm.Invoke(delegate { MainForm.ProgressBar_Step(data.Length); });
                            if (MessageFlag == (byte)UpdateEnumFlags.RespOK)
                            {
                                int readed = file.Read(data, 0, data.Length);
                                if (readed == 0)
                                {
                                    MainForm.AppendToDebug($"{Environment.NewLine}Uploading FW DONE");
                                    break;
                                }
                                message.Data = data.Take(readed).ToList();
                                sync_obj.Reset();
                                //Debug.WriteLine(message);
                                InputPeriph?.SendMsg(message);
                                MainForm.AppendToDebug("*", false);
                            }
                            else
                            {
                                MainForm.AppendToDebug($"{Environment.NewLine}Uploading FW ERROR {(UpdateEnumFlags)MessageFlag}");
                                break;
                            }
                        }
                        else
                        {
                            MainForm.AppendToDebug($"{Environment.NewLine}Uploading FW TIMEOUT");
                            break;
                        }
                    }

                    if (!bContinue)
                        MainForm.AppendToDebug($"{Environment.NewLine}Uploading FW ABORTED");

                    // Pockame na dobehnuti
                    Task.Delay(bContinue ? 1000 : 0).ContinueWith(t =>
                    {
                        MainForm.Invoke(delegate
                        {
                            MainForm.Cursor = Cursors.Default;
                            MainForm.ProgressBar_Value = 0;
                        });
                    });

                    DevConfigService.Instance.FreeProcessLock();
                }
            });
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        internal bool Open(string connect_str)
        {
            IInputPeriph? input_periph = null;
            string[] connect_str_parts = connect_str.Split(new char[] {  ';', ':', '@', '/', ',' });

            if(connect_str_parts.Length < 2)
                return false;

            switch (connect_str_parts[0])
            {
                case "USB Serial":
                    input_periph = new UsbSerial();
                    input_periph.Open(new { PortName = connect_str_parts[1], BaudRate = (connect_str_parts.Length > 2 ? connect_str_parts[2] : "115200") });
                    break;
                case "USB ToolStick":
                    input_periph = new ToolStick();
                    input_periph.Open(new { PortName = connect_str_parts[1], BaudRate = (connect_str_parts.Length > 2 ? connect_str_parts[2] : "115200") });
                    break;
                case "TCP Tunel":
                    input_periph = new TcpTunel();
                    input_periph.Open(new { HostName = connect_str_parts[1], Port = (connect_str_parts.Length > 2 ? connect_str_parts[2] : "10000") });
                    break;
                case "SSH Karo CAN":
                    input_periph = new SshCan();
                    input_periph.Open(new { HostName = connect_str_parts[1], Login = "root", Password = "ds-1564", LoginPort = (connect_str_parts.Length > 2 ? connect_str_parts[2] : "22") });
                    break;
            }

            if (input_periph != null)
            {
                ConnectString = connect_str;
                if (InputPeriph != null)
                    InputPeriph.Close();

                input_periph.Run();

                InputPeriph = input_periph;
                return true;
            }

            return false;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        internal bool ProcessLock()
        {
            if (Interlocked.Increment(ref process_lock) == 1)
                return true;
            
            Interlocked.Decrement(ref process_lock);
            MessageBox.Show("Wait for the running operation to complete.", "DevConfig - message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;    
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        internal void FreeProcessLock()
        {
            Interlocked.Exchange(ref process_lock, 0);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #region REGISTERS
        internal void GetRegisterFromDevice()
        {
            if (selectedDevice != null)
            {
                // vycteme seznam patramatru
                selectedDevice.Parameters = new List<Parameter>();

                Message msg = new Message();
                msg.CMD = Command.GetListParam;
                msg.SRC = MainForm.SrcAddress;
                msg.DEST = selectedDevice.Address;
                msg.Data.Add(0);

                sync_obj.Reset();
                InputPeriph?.SendMsg(msg);

                msg.Data[0] = 1;

                while (true)
                {
                    if (!sync_obj.WaitOne(1000))
                        break;
                    if (MessageFlag == 0 && message != null)
                    {
                        selectedDevice.Parameters.AddRange( NewParamItem(message.Data.ToArray()) );
                        message = null;
                        MessageFlag = 0xFF;
                    }
                    else
                        break;
                    sync_obj.Reset();
                    InputPeriph?.SendMsg(msg);
                }

                if (selectedDevice.Parameters.Count == 0 && selectedDeviceType?.Parameters != null)
                {
                    string file_name = Path.GetFullPath(@"Resources\" + selectedDeviceType.Parameters);

                    if (File.Exists(file_name))
                    {
                        JsonSerializerOptions options = new JsonSerializerOptions
                        {
                            Converters = { new JsonStringEnumConverter() }
                        };

                        var json = JsonSerializer.Deserialize<List<ParamConfig>>(File.ReadAllText(file_name), options);
                        var @params = (from xx in json where xx.DevId == selectedDevice.DevId select xx.Data).FirstOrDefault();
                        if (@params != null)
                        {
                            selectedDevice.Parameters.Clear();
                            foreach(Parameter parameter in @params)
                            {
                                if(parameter.Index == null || parameter.Index < 1)
                                {
                                    parameter.Index = null;
                                    selectedDevice.Parameters.Add(parameter);
                                }
                                else
                                {
                                    for(byte i = 0; i <= parameter.Index; i++)
                                    {
                                        Parameter par_idx = (Parameter)parameter.Clone();
                                        par_idx.Index = i;
                                        par_idx.Name += $"({i})";
                                        selectedDevice.Parameters.Add(par_idx);
                                    }
                                }
                            }
                        }
                    }
                }

                // vycteme hodnoty patramatru
                msg.CMD = Command.ParamRead;
                for(int i = 0; i < selectedDevice.Parameters.Count; i++)
                {
                    //msg.Data[0] = selectedDevice.Parameters[i].ParameterID;
                    msg.Data = new() { selectedDevice.Parameters[i].ParameterID };
                    if(selectedDevice.Parameters[i].Index != null)
                        msg.Data.Add((byte)selectedDevice.Parameters[i].Index!);
                    sync_obj.Reset();
                    InputPeriph?.SendMsg(msg);

                    if (sync_obj.WaitOne(1000))
                    {
                        if (MessageFlag == 0 && message != null)
                        {
                            NewParamData(selectedDevice.Parameters[i], message.Data.ToArray());
                            message = null;
                            MessageFlag = 0xFF;
                        }
                    }
                        //continue;
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private object? MinMaxVal(type t, byte[] d, int i)
        {
            switch (t)
            {
                case type.UInt8: 
                case type.UInt16:
                case type.UInt32:
                case type.String:
                    return BitConverter.ToUInt32(d, i); // TODO dodelat MSB

                case type.SInt8: 
                case type.SInt16:
                case type.SInt32:
                    return BitConverter.ToInt32(d, i); // TODO dodelat MSB

                default:
                    return null;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private Parameter[] NewParamItem(byte[] data)
        {
            Parameter[] parameters = new Parameter[data[11]+1];

            string param_name = System.Text.Encoding.ASCII.GetString(data.Skip(12).TakeWhile((x) => x != 0).ToArray());
            byte[] data2 = data.Skip(12 + param_name.Length).ToArray();

            parameters[0] = new Parameter();
            parameters[0].ParameterID = data[1];                             // Param ID
            parameters[0].Type = (type)(data[2] & 0x7F);                     // Param Type
            parameters[0].ReadOnly = (data[2] & 0x80) == 0x80;               // Flags
            parameters[0].MinVal = MinMaxVal(parameters[0].Type, data, 3);   // Param MinVal
            parameters[0].MaxVal = MinMaxVal(parameters[0].Type, data, 7);   // Param MaxVal
            parameters[0].Name = param_name;                                 // Param Name
            if (data2.Length > 1)
            {
                parameters[0].ByteOrder = (ByteOrder)data2[1];               // Poradi bajtu ve zprave.
                parameters[0].Format = System.Text.Encoding.ASCII.GetString(data2.Skip(2).TakeWhile((x) => x != 0).ToArray());
                byte[] data3 = data2.Skip(2 + parameters[0].Format!.Length).ToArray();
            }
            parameters[0].Index = null;                                      // Param Index

            for (byte i = 1; i <= data[11]; i++)
            {
                parameters[i] = (Parameter)parameters[0].Clone();
                parameters[i].Index = i;                                     // Param Index
                parameters[i].Name += $"({i})";                              // Param Name
            }

            if(data[11] > 0)
            {
                parameters[0].Index = 0;                                     // Param Index
                parameters[0].Name += "(0)";                                 // Param Name
            }

            return parameters;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void NewParamData(Parameter parameter, byte[] bytes)
        {
            try
            {
                int skip = 0;
                switch (parameter.Type)
                {
                    //case type.IpAddr:  break;
                    //case type.MacAddr: break;
                    case type.String: 
                        parameter.Value = System.Text.Encoding.ASCII.GetString(
                            bytes.SkipWhile((x) => x < 20).TakeWhile((x) => x != 0).ToArray() ); 
                        break;

                    case type.UInt8:
                        skip = bytes.Length - 1;
                        parameter.Value = (byte)bytes[skip];  
                        break;

                    case type.SInt8:
                        skip = bytes.Length - 1; 
                        parameter.Value = (sbyte)bytes[skip]; 
                        break;

                    case type.UInt16:
                        skip = bytes.Length - 2;
                        if (parameter.ByteOrder == ByteOrder.LSB)
                            parameter.Value = BitConverter.ToUInt16(bytes, skip);
                        else
                            parameter.Value = BitConverter.ToUInt16(bytes.Skip(skip).Take(2).Reverse().ToArray());
                        break;

                    case type.SInt16:
                        skip = bytes.Length - 2;
                        if (parameter.ByteOrder == ByteOrder.LSB)
                            parameter.Value = BitConverter.ToInt16(bytes, skip);
                        else
                            parameter.Value = BitConverter.ToInt16(bytes.Skip(skip).Take(2).Reverse().ToArray());
                        break;

                    case type.UInt32:
                        skip = bytes.Length - 4;
                        if (parameter.ByteOrder == ByteOrder.LSB)
                            parameter.Value = BitConverter.ToUInt32(bytes, skip); 
                        else
                            parameter.Value = BitConverter.ToUInt32(bytes.Skip(skip).Take(4).Reverse().ToArray());
                        break;

                    case type.SInt32:
                        skip = bytes.Length - 4;
                        if (parameter.ByteOrder == ByteOrder.LSB)
                            parameter.Value = BitConverter.ToInt32(bytes, skip);
                        else
                            parameter.Value = BitConverter.ToInt32(bytes.Skip(skip).Take(4).Reverse().ToArray());
                        break;

                    default: throw new NotImplementedException();
                }
                Debug.WriteLine($"{parameter.Name} - {parameter.Type} - {parameter.Value:X} - {bytes.Length - 1}");
            }
            catch
            {

            }
        }
        #endregion
    }
}
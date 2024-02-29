using CanDiagSupport;
using DevConfig.Utils;
using SshCANns;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using TcpTunelNs;
using ToolStickNs;
using UsbSerialNs;
using static System.Windows.Forms.DataFormats;
using Message = CanDiagSupport.Message;

namespace DevConfig.Service
{
    public class DevConfigService
    {
        internal MainForm MainForm;
        internal Device? selectedDevice = null;
        internal DeviceType? selectedDeviceType = null;

        ///////////////////////////////////////////////////////////////////////////////////////////

        private int process_lock = 0; // Kontrola pracujícího procesu. Povolení pouze jedné úlohy.
        private bool bContinue = true;
        private byte MessageFlag = 0;
        private Message? message = null;
        private readonly ManualResetEvent sync_obj = new(false);
        private byte LastReqValue = 0;

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
        #region  ParamEnums
        internal Dictionary<string, Dictionary<uint, string>> ParamEnums = new();
        internal Dictionary<uint, string>? GetParamEnum(string s_format)
        {
            if(s_format.Contains('['))
            {
                var str_enums = new string(s_format.SkipWhile(x => x != '[').TakeWhile(x => x != ']').ToArray()) + ']';

                if (ParamEnums.ContainsKey(str_enums))
                {
                    return ParamEnums[str_enums];
                }
                else
                {
                    string[] str_enums_arr = str_enums.Split(new char[] { ',', '[', ']' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                    uint i = 0;
                    Dictionary<uint, string> di_enums = new();
                    foreach (string str_enum in str_enums_arr)
                    {
                        string[] en_opar = str_enum.Split('=', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                        if (en_opar.Length >= 2)
                            i = en_opar[1].ToUInt32();
                        di_enums[i++] = en_opar[0];
                    }
                    ParamEnums.Add(str_enums, di_enums);
                    return di_enums;
                }
            }
            return null;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        internal bool TryParse(string? text, [NotNullWhen(true)] out double d_val)
        {
            d_val = default;

            if(text == null)
                return false;

            text = text.ToLower();

            if (text.StartsWith("0b"))
            {
                d_val = Convert.ToDouble(Convert.ToUInt32(text[2..], 2));
                return true;
            }
            else if (text.StartsWith("0o"))
            {
                d_val = Convert.ToDouble(Convert.ToUInt32(text[2..], 8));
                return true;
            }
            else if (text.StartsWith("0x"))
            {
                d_val = Convert.ToDouble(Convert.ToUInt32(text[2..], 16));
                return true;
            }
            else
            {
                return double.TryParse(text, out d_val);
            }    
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        internal bool TryGetParamEnum(string? format, [NotNullWhen(true)] out Dictionary<uint, string> di_enums)
        {
#pragma warning disable CS8625 // Literál null nejde převést na odkazový typ, který nemůže mít hodnotu null.
            di_enums = default;
#pragma warning restore CS8625

            if (format == null)
                return false;

            var x = GetParamEnum(format);
            if (x != null)
            {
                di_enums = x;
                return true;
            }
            return false;
        }
        #endregion

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

                byte address;
                var ip_type = DevConfigService.Instance.InputPeriph!.GetType();
                if (ip_type == typeof(UsbSerialNs.UsbSerial))
                {
                    address = msg.Data[5];
                }
                else if (ip_type == typeof(TcpTunelNs.TcpTunel))
                {
                    if (msg.Data[5] == msg.SRC)
                        address = msg.Data[5];
                    else if (LastReqValue == msg.Data[5])
                        address = msg.Data[5];
                    else if (LastReqValue == msg.SRC)
                        address = msg.SRC;
                    else
                        return;
                }
                else
                {
                    address = msg.SRC;
                }

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
                Debug.WriteLine($"+ {msg}");
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

                    /////////////////
                    //byte[]? dev_addr = null;// new byte[] { 0x10, 0x20, 0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F, 0x70, 0x80, 0x81, 0x94 };
                    byte[]? dev_addr = new byte[] { 0x10, 0x20, 0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F, 0x70, 0x80, 0x81, 0x94 };

                    byte SearchFrom = 0x00;
                    byte SearchTo = (byte)(dev_addr != null ? (dev_addr.Length - 1) : (0xFE - 1));

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
                    for (byte d = SearchFrom; d <= SearchTo && bContinue; d++)
                    {
                        byte dest = (byte)(dev_addr != null ? dev_addr[d] : d);

                        if (!usb_serial)
                            MainForm.Invoke(delegate { MainForm.ProgressBar_Value = dest; });
                        message.DEST = dest;
                        LastReqValue = dest;
                        InputPeriph.SendMsg(message);
                        Task.Delay(5).Wait();
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
                //Debug.WriteLine($"- {msg}");
                InputPeriph?.SendMsg(msg);

                //Task.Delay(1000).Wait(); // TODO

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
                    Task.Delay(3).Wait(); // TODO
                    sync_obj.Reset();
                    //Debug.WriteLine($"- {msg}");
                    InputPeriph?.SendMsg(msg);
                }

                if (selectedDevice.Parameters.Count == 0 && selectedDeviceType?.Parameters != null)
                {
                    string file_name = Path.GetFullPath(@"Resources\" + selectedDeviceType.Parameters);

                    if (File.Exists(file_name))
                    {
                        JsonSerializerOptions options = new JsonSerializerOptions{ Converters = { new JsonStringEnumConverter() } };
                        var json = JsonSerializer.Deserialize<List<ParamConfig>>(File.ReadAllText(file_name), options);
                        var ParamConfigs = from xx in json where xx.DevId != null && xx.DevId.Contains(selectedDevice.DevId) select xx;
                        selectedDevice.Parameters.Clear();
                        foreach (var ParamConfig in ParamConfigs)
                        {
                            if (ParamConfig.Data != null)
                            {
                                var @params = ParamConfig.Data;
                                foreach (Parameter parameter in @params)
                                {
                                    if (parameter.Enabled)
                                    {
                                        parameter.ByteOrder ??= ParamConfig.ByteOrder;
                                        parameter.MinVal ??= parameter.DefaultMin();
                                        parameter.MaxVal ??= parameter.DefaultMax();

                                        if (parameter.Index == null || parameter.Index < 1)
                                        {
                                            parameter.Index = null;
                                            selectedDevice.Parameters.Add(parameter);
                                        }
                                        else
                                        {
                                            for (byte i = 0; i <= parameter.Index; i++)
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
                    }
                }

                // vycteme hodnoty patramatru
                msg.CMD = Command.ParamRead;
                for(int i = 0; i < selectedDevice.Parameters.Count; i++)
                {
                    msg.Data = new() { selectedDevice.Parameters[i].ParameterID };
                    if(selectedDevice.Parameters[i].Index != null)
                        msg.Data.Add((byte)selectedDevice.Parameters[i].Index!);
                    else
                        msg.Data.Add((byte)0);
                    sync_obj.Reset();
                    LastReqValue = msg.Data[0];
                    Debug.WriteLine($"- {msg}");
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
        private object? MinMaxVal(ParamType t, byte[] d, int i)
        {
            switch (t)
            {
                case ParamType.UInt8: 
                case ParamType.UInt16:
                case ParamType.UInt32:
                case ParamType.String:
                    return BitConverter.ToUInt32(d, i); // TODO dodelat MSB

                case ParamType.SInt8: 
                case ParamType.SInt16:
                case ParamType.SInt32:
                    return BitConverter.ToInt32(d, i); // TODO dodelat MSB

                default:
                    return null;
            }
        }

        enum get_list_param_e { eDescription, eFormat, eByteOrder, eGain, eOffset };

        ///////////////////////////////////////////////////////////////////////////////////////////
        /// Zpracování nového parametru co prisel z CAN
        ///////////////////////////////////////////////////////////////////////////////////////////
        private Parameter[] NewParamItem(byte[] data)
        {
            Parameter[] parameters = new Parameter[data[11]+1];

            string param_name = System.Text.Encoding.ASCII.GetString(data.Skip(12).TakeWhile((x) => x != 0).ToArray());

            parameters[0] = new Parameter();
            parameters[0].ParameterID = data[1];                             // Param ID
            parameters[0].Type = (ParamType)(data[2] & 0x7F);                // Param Type
            parameters[0].ReadOnly = (data[2] & 0x80) == 0x80;               // Flags
            parameters[0].MinVal = MinMaxVal(parameters[0].Type, data, 3);   // Param MinVal
            parameters[0].MaxVal = MinMaxVal(parameters[0].Type, data, 7);   // Param MaxVal
            parameters[0].Name = param_name;                                 // Param Name
            parameters[0].Index = null;                                      // Param Index
            parameters[0].ByteOrder = ByteOrder.LSB;                         // Byte Order

            // Volitelne parametry


            byte str_len;
            int opt_par_index = 12 + param_name.Length + 1;

            while(opt_par_index < data.Length)
            {
                get_list_param_e par_type = (get_list_param_e)data[opt_par_index++];
                switch(par_type)
                {
                    case get_list_param_e.eDescription:
                        str_len = data[opt_par_index++];
                        parameters[0].Description = System.Text.Encoding.ASCII.GetString(data.Skip(opt_par_index).Take(str_len).ToArray());
                        opt_par_index += str_len;
                        break;

                    case get_list_param_e.eFormat:
                        str_len = data[opt_par_index++];
                        parameters[0].Format = System.Text.Encoding.ASCII.GetString(data.Skip(opt_par_index).Take(str_len).ToArray());
                        opt_par_index += str_len;
                        break;

                    case get_list_param_e.eByteOrder:
                        parameters[0].ByteOrder = (ByteOrder)data[opt_par_index++];
                        break;

                    case get_list_param_e.eGain:
                        parameters[0].Gain = double.Parse(BitConverter.ToSingle(data, opt_par_index).ToString("E"));
                        opt_par_index += 4;
                        break;

                    case get_list_param_e.eOffset:
                        parameters[0].Offset = double.Parse(BitConverter.ToSingle(data, opt_par_index).ToString("E"));
                        opt_par_index += 4;
                        break;
                }
            }

            // Naklonujeme poku je to pole.
            if(data[11] > 0)
            {
                parameters[0].Index = 0;
                for (byte i = 1; i <= data[11]; i++)
                {
                    parameters[i] = (Parameter)parameters[0].Clone();
                    parameters[i].Index = i;
                }
            }

            return parameters;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        /// Zpracování nových dat pro parametr co prisel z CAN
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void NewParamData(Parameter parameter, byte[] bytes)
        {
            try
            {
                int skip = 0;
                switch (parameter.Type)
                {
                    case ParamType.IpAddr:  
                        skip = bytes.Length - 4;
                        parameter.Value = bytes[skip..];
                        break;
                    case ParamType.MacAddr:
                        skip = bytes.Length - 6;
                        parameter.Value = bytes[skip..];
                        break;
                    case ParamType.String: 
                        parameter.Value = System.Text.Encoding.ASCII.GetString(
                            bytes.SkipWhile((x) => x < 20).TakeWhile((x) => x != 0).ToArray() );
                        skip = bytes.Length - ((string)parameter.Value).Length;
                        if(skip == 1 && bytes.Length >= 2 && bytes[1] == LastReqValue)
                        {
                            // Tady je to torochu divočina. Problem může nastat pokud ParID je platný ASCII znak.
                            skip = 2;
                            parameter.Value = ((string)parameter.Value).Substring(1);
                        }
                        break;

                    case ParamType.Bool:
                        skip = bytes.Length - 1;
                        parameter.Value = (bytes[skip] != 0);
                        break;

                    case ParamType.UInt8:
                        skip = bytes.Length - 1;
                        parameter.Value = (byte)bytes[skip];  
                        break;

                    case ParamType.SInt8:
                        skip = bytes.Length - 1; 
                        parameter.Value = (sbyte)bytes[skip]; 
                        break;

                    case ParamType.UInt16:
                        skip = bytes.Length - 2;
                        if (parameter.ByteOrder == ByteOrder.LSB)
                            parameter.Value = BitConverter.ToUInt16(bytes, skip);
                        else
                            parameter.Value = BitConverter.ToUInt16(bytes.Skip(skip).Take(2).Reverse().ToArray());
                        break;

                    case ParamType.SInt16:
                        skip = bytes.Length - 2;
                        if (parameter.ByteOrder == ByteOrder.LSB)
                            parameter.Value = BitConverter.ToInt16(bytes, skip);
                        else
                            parameter.Value = BitConverter.ToInt16(bytes.Skip(skip).Take(2).Reverse().ToArray());
                        break;

                    case ParamType.UInt32:
                        skip = bytes.Length - 4;
                        if (parameter.ByteOrder == ByteOrder.LSB)
                            parameter.Value = BitConverter.ToUInt32(bytes, skip); 
                        else
                            parameter.Value = BitConverter.ToUInt32(bytes.Skip(skip).Take(4).Reverse().ToArray());
                        break;

                    case ParamType.SInt32:
                        skip = bytes.Length - 4;
                        if (parameter.ByteOrder == ByteOrder.LSB)
                            parameter.Value = BitConverter.ToInt32(bytes, skip);
                        else
                            parameter.Value = BitConverter.ToInt32(bytes.Skip(skip).Take(4).Reverse().ToArray());
                        break;

                    default: throw new NotImplementedException();
                }
                parameter.insert_par_id_when_write = skip >= 2;
                parameter.OldValue = parameter.Value;
                Debug.WriteLine($"{parameter.Name} - {parameter.Type} - {parameter.Value:X} - {bytes.Length - 1}");
            }
            catch(Exception ex)
            {
                MainForm.AppendToDebug(ex.Message, default, default, Color.Red);
            }
        }

        #endregion
    }
}
﻿
using CanDiagSupport;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Message = CanDiagSupport.Message;

namespace DevConfig.Service
{
    public class DevConfigService
    {
        MainForm mainForm;
        bool bContinue = true;
        byte MessageFlag = 0;
        readonly ManualResetEvent sync_obj = new(false);
        ///////////////////////////////////////////////////////////////////////////////////////////
        public Device? selectedDevice = null;

        ///////////////////////////////////////////////////////////////////////////////////////////
        enum UpdateEnumFlags
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
            mainForm = (MainForm)Application.OpenForms["MainForm"];
            mainForm.AbortEvent += MainForm_AbortEvent;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void MainForm_AbortEvent()
        {
            bContinue = false;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void _InputPeriph_MessageReceived(Message message)
        {
            if (message.CMD == Command.Ident && message.Data.Count >= 14) // TODO && msg.RF)
            {
                //Debug.WriteLine(message);
                byte state = message.Data[0];
                uint deviceID = (uint)(message.Data[1] << 24 | message.Data[2] << 16 | message.Data[3] << 8 | message.Data[4]);
                byte address = DevConfigService.Instance.InputPeriph!.GetType() == typeof(UsbSerialNs.UsbSerial) ? message.Data[5] : address = message.SRC;
                string fwVer = $"{message.Data[6]}.{message.Data[7]}";
                string cpuId = $"{BitConverter.ToString(message.Data.Skip(8).Take(12).ToArray()).Replace("-", " ")}";

                mainForm.NewIdent(deviceID, address, fwVer, cpuId, state);
            }
            else if ((message.CMD == Command.StartUpdate || message.CMD == Command.UpdateMsg) && message.Data.Count == 1)
            {
                //Debug.WriteLine(message);
                MessageFlag = message.Data[0];
                sync_obj.Set();
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        internal void RefreshDeviceList()
        {
            if (InputPeriph != null)
            {
                selectedDevice = null;
                mainForm.DevicesList.Clear();
                mainForm.TreeWnd?.listViewDevices.Items.Clear();

                Task.Run(() =>
                {
                    bContinue = true;
                    Message message = new Message();
                    message.CMD = Command.Ident;
                    message.SRC = MainForm.SrcAddress;

                    byte SearchFrom = 0x00;
                    byte SearchTo = 0xFE - 1;

                    if (InputPeriph!.GetType() == typeof(UsbSerialNs.UsbSerial))
                        SearchFrom = SearchTo = 0xFE;

                    mainForm.Invoke(delegate
                    {
                        mainForm.Cursor = Cursors.WaitCursor;
                        mainForm.ProgressBar_Minimum = SearchFrom;
                        mainForm.ProgressBar_Maximum = SearchTo;
                        mainForm.ProgressBar_Value = SearchFrom;
                    });

                    // pošleme ident do všech zažízení
                    for (byte dest = SearchFrom; dest <= SearchTo && bContinue; dest++)
                    {
                        mainForm.Invoke(delegate { mainForm.ProgressBar_Value = dest; });
                        message.DEST = dest;
                        InputPeriph.SendMsg(message);
                        Task.Delay(3).Wait();
                    }

                    // Pockame na dobehnuti
                    Task.Delay(bContinue ? 1500 : 0).ContinueWith(t =>
                    {
                        mainForm.Invoke(delegate
                        {
                            mainForm.Cursor = Cursors.Default;
                            mainForm.ProgressBar_Value = SearchFrom;
                        });
                    });
                });
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        internal void UpdateFw(string file_name)
        {
            Debug.Assert(selectedDevice != null);


            Task.Run(() =>
            {
                bContinue = true;

                mainForm.AppendToDebug("Uploading FW");

                byte[] data = new byte[240];
                FileStream file = File.OpenRead(file_name);

                mainForm.Invoke(delegate
                {
                    mainForm.Cursor = Cursors.WaitCursor;
                    mainForm.ProgressBar_Minimum = 0;
                    mainForm.ProgressBar_Maximum = (int)file.Length;
                    mainForm.ProgressBar_Value = 0;
                });

                Message message = new Message() { CMD = Command.StartUpdate, SRC = MainForm.SrcAddress, DEST = selectedDevice.Address };
                sync_obj.Reset();
                //Debug.WriteLine(message);
                InputPeriph?.SendMsg(message);
                mainForm.AppendToDebug("*", false);

                message.CMD = Command.UpdateMsg;

                while (bContinue)
                {
                    if (sync_obj.WaitOne(3000))
                    {
                        mainForm.Invoke(delegate { mainForm.ProgressBar_Step(data.Length); });
                        if (MessageFlag == (byte)UpdateEnumFlags.RespOK)
                        {
                            int readed = file.Read(data, 0, data.Length);
                            if (readed == 0)
                            {
                                mainForm.AppendToDebug($"{Environment.NewLine}Uploading FW DONE");
                                break;
                            }
                            message.Data = data.Take(readed).ToList();
                            sync_obj.Reset();
                            //Debug.WriteLine(message);
                            InputPeriph?.SendMsg(message);
                            mainForm.AppendToDebug("*", false);
                        }
                        else
                        {
                            mainForm.AppendToDebug($"{Environment.NewLine}Uploading FW ERROR {(UpdateEnumFlags)MessageFlag}");
                            break;
                        }
                    }
                    else
                    {
                        mainForm.AppendToDebug($"{Environment.NewLine}Uploading FW TIMEOUT");
                        break;
                    }
                }

                if(!bContinue)
                    mainForm.AppendToDebug($"{Environment.NewLine}Uploading FW ABORTED");

                // Pockame na dobehnuti
                Task.Delay(bContinue ? 1000 : 0).ContinueWith(t =>
                {
                    mainForm.Invoke(delegate
                    {
                        mainForm.Cursor = Cursors.Default;
                        mainForm.ProgressBar_Value = 0;
                    });
                });
            });
        }

        ///////////////////////////////////////////////////////////////////////////////////////////

    }
}
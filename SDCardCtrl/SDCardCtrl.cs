using DevConfigSupp;
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

using Message = CanDiagSupport.Message;

namespace SDCardCtrlNs
{
    public partial class SDCardCtrl : DockContentEx
    {
        const byte ECmd_SD_UploadFile = 0x60;
        const byte ECmd_SD_UploadFileStart = 0x61;
        const byte ECmd_SD_DownloadFile = 0x62;
        const byte ECmd_SD_DownloadFileStart = 0x63;
        const byte ECmd_SD_DownloadFileOtherData = 0x64;
        const byte ECmd_SD_GetList = 0x65;
        const byte ECmd_SD_DeleteFile = 0x66;
        const byte ECmd_StringCommand = 0xA0;

        byte CAN_ID;

        ///////////////////////////////////////////////////////////////////////////////////////////
        public SDCardCtrl()// : base (null)
        {
            InitializeComponent();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void SDCardCtrl_Load(object sender, EventArgs e)
        {
            CAN_ID = (byte)(MainApp.GetProperty("SelectedDeviceCanID") ?? 0);
            Debug.WriteLine($"CAN ID = {CAN_ID}");
            Text += $" ({CAN_ID:X2})";
            //GetTist();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void button1_Click(object sender, EventArgs e)
        {
            GetTist();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void GetTist()
        {
            string directory = string.Empty;

            Message message = new Message();
            message.DEST = CAN_ID;
#if !TXT_FORMAT
            message.CMD = ECmd_SD_GetList;
            message.Data = Encoding.ASCII.GetBytes(directory + "\0").ToList();
#else            
            message.CMD = ECmd_StringCommand;
            message.Data = Encoding.ASCII.GetBytes($"\x02GetList {directory}\x0d").ToList();
#endif

            SendMessage(message);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        protected override void InputPeriph_MessageReceived(Message msg)
        {
            switch (msg.CMD)
            {
                case ECmd_SD_GetList:
                    break;
                default:
                    break;
            }
        }

    }
}

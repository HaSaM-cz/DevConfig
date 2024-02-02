namespace LedStripCtrl
{
    public partial class LedStripCtrl
    {
        public enum cmds
        {
            teCmd_Identify = 0x02,
            teCmd_SetState = 0x10,
            teCmd_WritePar = 0x47,
            teCmd_ReadPar = 0x48,
            teCmd_StartUpdate = 0x50,
            teCmd_UpdateMsg = 0x51,
            teCmd_Reset = 0x5F,
        }

        public enum eParam
        {
            paPixelOffset,
            paMode,
            paState,
            paClearRGB,
            paSaveRGB,
            paPixelsMapInit,
            paPixelsMapData
        }

        public enum UpdateEnum
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
    }
}

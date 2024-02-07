using DevConfig.Utils;
using Newtonsoft.Json;
using WeifenLuo.WinFormsUI.Docking;

namespace DevConfig
{
    public class DeviceType
    {
        [JsonConverter(typeof(HexStringJsonConverter))]
        public uint DevId;

        public string? FirmwarePath; 
        public string Name = string.Empty;
        public string? UserControl;
        public List<DockContent> UserControlsList = new();
    }
}

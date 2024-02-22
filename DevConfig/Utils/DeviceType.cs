using DevConfig.Utils;
using Newtonsoft.Json;
using WeifenLuo.WinFormsUI.Docking;

namespace DevConfig
{
    public class DeviceType
    {
        [JsonIgnore] public string? FirmwarePath;
        [JsonIgnore] public List<DockContent> UserControlsList = new();

        [JsonConverter(typeof(HexStringJsonConverter))]
        public uint DevId;
        public string Name = string.Empty;
        public string? UserControl;
        public string? Parameters;
    }
}

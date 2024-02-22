using DevConfig.Utils;
using System.Text.Json.Serialization;
using WeifenLuo.WinFormsUI.Docking;

namespace DevConfig
{
    public class DeviceType
    {
        [JsonIgnore] public string? FirmwarePath;
        [JsonIgnore] public List<DockContent> UserControlsList = new();

        [JsonConverter(typeof(HexUInt32JsonConverter))] public uint DevId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? UserControl { get; set; }
        public string? Parameters { get; set; }
    }
}

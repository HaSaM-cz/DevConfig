using DevConfig.Utils;
using Newtonsoft.Json;

namespace DevConfig
{
    public class DeviceType
    {
        [JsonConverter(typeof(HexStringJsonConverter))]
        public uint DevId;

        public string? FirmwarePath; 
        public string Name = string.Empty;
    }
}

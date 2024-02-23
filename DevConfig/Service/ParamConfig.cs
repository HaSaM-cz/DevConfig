using DevConfig.Utils;
using System.Text.Json.Serialization;

namespace DevConfig.Service
{
    public class ParamConfig
    {
        [JsonConverter(typeof(HexUInt32ArrJsonConverter))] public List<uint>? DevId { get; set; }
        public List<Parameter>? Data { get; set; }
        public ByteOrder ByteOrder { get; set; } = ByteOrder.LSB;
    }
}

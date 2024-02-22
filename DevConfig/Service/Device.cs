using DevConfig.Utils;
using System.Text.Json.Serialization;

namespace DevConfig.Service
{
    public class Device
    {
        public byte Address { get; set; }
        public string AddressStr { get { return $"{Address:X2}"; } }
        public uint DevId { get; set; }
        public string DevIdStr { get { return $"{DevId:X}"; } }

        public string? Name;// { get; set; }
        public string? FwVer;// { get; set; }
        public string? CpuId;// { get; set; }

        internal List<Parameter>? Parameters = null;

        public ListViewItem? listViewItem = null;
    }

    public enum ByteOrder { LSB, MSB };
    public enum type { UInt8, UInt16, UInt32, String, IpAddr, SInt8, SInt16, SInt32, MacAddr };

    public class Parameter : ICloneable
    {
        [JsonConverter(typeof(HexByteJsonConverter))] public byte ParameterID { get; set; }
        public type Type { get; set; }
        public bool ReadOnly { get; set; }
        [JsonConverter(typeof(HexObjectJsonConverter))] public object? MinVal { get; set; }
        [JsonConverter(typeof(HexObjectJsonConverter))] public object? MaxVal { get; set; }
        public byte? Index { get; set; }
        public string? Name { get; set; }
        public string? Format { get; set; }
        public ByteOrder ByteOrder { get; set; } = ByteOrder.LSB;

        //////////////////////////////////////////////////////////////////////////
        internal object? Value;

        //////////////////////////////////////////////////////////////////////////
        internal string StrMin
        {
            get{ return string.IsNullOrWhiteSpace(Format) ? $"{MinVal}" : string.Format(Format, MinVal); }
        }

        //////////////////////////////////////////////////////////////////////////
        internal string StrMax
        {
            get{ return string.IsNullOrWhiteSpace(Format) ? $"{MaxVal}" : string.Format(Format, MaxVal); }
        }

        //////////////////////////////////////////////////////////////////////////
        internal string StrValue 
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Format))
                    return $"{Value}";
                else if (Value == null)
                    return "";
                else
                    return string.Format(Format, Value);
            } 
        }

        //////////////////////////////////////////////////////////////////////////
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class ParamConfig
    {
        [JsonConverter(typeof(HexUInt32JsonConverter))] public uint DevId { get; set; }
        public List<Parameter>? Data { get; set; }
    }
}

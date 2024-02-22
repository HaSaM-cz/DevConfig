using DevConfig.Utils;
using Newtonsoft.Json;

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
        public byte ParameterID;
        public type Type;
        public bool ReadOnly;
        [JsonConverter(typeof(HexStringJsonConverter))]
        public object? MinVal;
        [JsonConverter(typeof(HexStringJsonConverter))]
        public object? MaxVal;
        public byte? Index;
        public string? Name;
        public string? Format;
        internal object? Value;
        public ByteOrder ByteOrder = ByteOrder.LSB;

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
        [JsonConverter(typeof(HexStringJsonConverter))]
        public uint DevId;
        public List<Parameter>? Data;
    }
}

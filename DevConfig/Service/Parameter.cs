using DevConfig.Utils;
using System.Text.Json.Serialization;

namespace DevConfig.Service
{
    public enum ByteOrder { LSB, MSB };
    public enum type { UInt8, UInt16, UInt32, String, IpAddr, SInt8, SInt16, SInt32, MacAddr, Bool };

    public class Parameter : ICloneable
    {
        [JsonConverter(typeof(HexByteJsonConverter))] public byte ParameterID { get; set; }
        public type Type { get; set; }
        public bool ReadOnly { get; set; }
        public bool Enabled { get; set; } = true;
        [JsonConverter(typeof(HexObjectJsonConverter))] public object? MinVal { get; set; }
        [JsonConverter(typeof(HexObjectJsonConverter))] public object? MaxVal { get; set; }
        public byte? Index { get; set; }
        public string? Name { get; set; }
        public string? Format { get; set; }
        public ByteOrder? ByteOrder { get; set; }
        public double? Gain { get; set; }
        public double? Offset { get; set; }


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
                if (Value == null)
                    return "";

                object value = Value;
                if (value.IsNumericType())
                {
                    if (Gain != null || Offset != null)
                        value = Convert.ToDouble(Value) * (Gain ?? 1.0) + (Offset ?? 0.0);
                }

                if (string.IsNullOrWhiteSpace(Format))
                {
                    return $"{value}";
                }
                else
                {
                    if (Format.Contains('['))
                    {
                        var enum_data = Format.SkipWhile(x => x != '[').Skip(1).TakeWhile(x => x != ']');
                        if (enum_data != null)
                        {
                            uint i = 0;
                            Dictionary<uint, string> di_enums = new Dictionary<uint, string>();
                            string[] str_enums = new string(enum_data.ToArray()).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                            foreach (string str_enum in str_enums)
                            {
                                string[] en_opar = str_enum.Split('=', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                                if (en_opar.Length >= 2)
                                    i = en_opar[1].ToUInt32();
                                di_enums[i++] = en_opar[0];
                            }
                            return di_enums[Convert.ToUInt32(value)];
                        }
                    }
                    return string.Format(Format, value);
                }
            } 
        }

        //////////////////////////////////////////////////////////////////////////
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        //////////////////////////////////////////////////////////////////////////
        internal object? DefaultMin()
        {
            return Type switch
            {
                type.UInt8 => byte.MinValue,
                type.UInt16 => ushort.MinValue,
                type.UInt32 => uint.MinValue,
                type.SInt8 => sbyte.MinValue,
                type.SInt16 => short.MinValue,
                type.SInt32 => int.MinValue,
                type.Bool => false,
                _ => null
            };
        }

        //////////////////////////////////////////////////////////////////////////
        internal object? DefaultMax()
        {
            return Type switch
            {
                type.UInt8 => byte.MaxValue,
                type.UInt16 => ushort.MaxValue,
                type.UInt32 => uint.MaxValue,
                type.SInt8 => sbyte.MaxValue,
                type.SInt16 => short.MaxValue,
                type.SInt32 => int.MaxValue,
                type.Bool => true,
                _ => null
            };
        }
    }
}

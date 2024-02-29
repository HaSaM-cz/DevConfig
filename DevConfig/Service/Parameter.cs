using DevConfig.Utils;
using System;
using System.Text.Json.Serialization;

namespace DevConfig.Service
{
    public enum ByteOrder { LSB, MSB };
    public enum ParamType { UInt8, UInt16, UInt32, String, IpAddr, SInt8, SInt16, SInt32, Bool, MacAddr };

    public class Parameter : ICloneable
    {
        [JsonConverter(typeof(HexByteJsonConverter))] public byte ParameterID { get; set; }
        public ParamType Type { get; set; }
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
        public string? Description { get; internal set; }


        //////////////////////////////////////////////////////////////////////////
        internal object? Value;
        internal object? OldValue;
        internal bool insert_par_id_when_write = false;

        //////////////////////////////////////////////////////////////////////////
        internal string StrMin
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Format))
                    return $"{MinVal}";
                else if (DevConfigService.Instance.TryGetParamEnum(Format, out Dictionary<uint, string> di_enums))
                    return di_enums.First().Value;
                else if (IsNumeric && (Gain != null || Offset != null))
                    return string.Format(Format, Convert.ToDouble(MinVal) * (Gain ?? 1.0) + (Offset ?? 0.0)); 
                else
                    return string.Format(Format, MinVal);
            }
        }

        //////////////////////////////////////////////////////////////////////////
        internal string StrMax
        {
            get
            { 
                if(string.IsNullOrWhiteSpace(Format))
                    return $"{MaxVal}";
                else if (DevConfigService.Instance.TryGetParamEnum(Format, out Dictionary<uint, string> di_enums))
                    return di_enums.Last().Value;
                else if (IsNumeric && (Gain != null || Offset != null))
                    return string.Format(Format, Convert.ToDouble(MaxVal) * (Gain ?? 1.0) + (Offset ?? 0.0));
                else
                    return string.Format(Format, MaxVal);
            }
        }

        //////////////////////////////////////////////////////////////////////////
        internal string StrValue 
        {
            get
            {
                if (Value == null)
                    return "";

                object value = Value;
                if (Type == ParamType.IpAddr)
                {
                    value = $"{((byte[])Value)[0]}.{((byte[])Value)[1]}.{((byte[])Value)[2]}.{((byte[])Value)[3]}";
                }
                else if (Type == ParamType.MacAddr)
                {
                    value = BitConverter.ToString((byte[])Value);
                }
                else if (value.IsNumericType())
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
                    if(DevConfigService.Instance.TryGetParamEnum(Format, out Dictionary<uint, string> di_enums))
                    {
                        uint e_idx = Convert.ToUInt32(value);
                        if (di_enums.ContainsKey(e_idx))
                            return di_enums[e_idx];
                        else
                            return $"{value}";
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
                ParamType.UInt8 => byte.MinValue,
                ParamType.UInt16 => ushort.MinValue,
                ParamType.UInt32 => uint.MinValue,
                ParamType.SInt8 => sbyte.MinValue,
                ParamType.SInt16 => short.MinValue,
                ParamType.SInt32 => int.MinValue,
                ParamType.Bool => false,
                _ => null
            };
        }

        //////////////////////////////////////////////////////////////////////////
        internal object? DefaultMax()
        {
            return Type switch
            {
                ParamType.UInt8 => byte.MaxValue,
                ParamType.UInt16 => ushort.MaxValue,
                ParamType.UInt32 => uint.MaxValue,
                ParamType.SInt8 => sbyte.MaxValue,
                ParamType.SInt16 => short.MaxValue,
                ParamType.SInt32 => int.MaxValue,
                ParamType.Bool => true,
                _ => null
            };
        }

        //////////////////////////////////////////////////////////////////////////
        internal bool IsNumeric
        {
            get
            {
                switch(Type)
                {
                    case ParamType.UInt8:
                    case ParamType.UInt16:
                    case ParamType.UInt32:
                    case ParamType.SInt8:
                    case ParamType.SInt16:
                    case ParamType.SInt32:
                        return true;
                    default:
                        return false;
                }
            }
        }
    }
}

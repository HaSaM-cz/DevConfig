using System.Text.Json;
using System.Text.Json.Serialization;

namespace DevConfig.Utils
{
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class HexObjectJsonConverter : JsonConverter<object>
    {
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt64(out long b))
                    return b;
            }
            else
            {
                string? str = reader.GetString();
                if (str != null)
                {
                    if (str.StartsWith("0x"))
                        return Convert.ToUInt32(str.Substring("0x".Length), 16);
                    else
                        return Convert.ToUInt32(str);
                }
            }
            return 0;
        }

        public override void Write(Utf8JsonWriter writer, object Value, JsonSerializerOptions options)
        {
            throw new JsonException();
            //writer.WriteStringValue($"0x{Value:X}");
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class HexUInt32JsonConverter : JsonConverter<UInt32>
    {
        public override UInt32 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetUInt32(out uint b))
                    return b;
            }
            else
            {
                string? str = reader.GetString();
                if (str != null)
                {
                    if (str.StartsWith("0x"))
                        return Convert.ToUInt32(str.Substring("0x".Length), 16);
                    else
                        return Convert.ToUInt32(str);
                }
            }
            return 0;
        }

        public override void Write(Utf8JsonWriter writer, UInt32 Value, JsonSerializerOptions options)
        {
            throw new JsonException();
            //writer.WriteStringValue($"0x{Value:X}");
        }
    }
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class HexByteJsonConverter : JsonConverter<byte>
    {
        public override byte Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetByte(out byte b))
                    return b;
            }
            else
            {
                string? str = reader.GetString();
                if (str != null)
                {
                    if (str.StartsWith("0x"))
                        return Convert.ToByte(str.Substring("0x".Length), 16);
                    else
                        return Convert.ToByte(str);
                }
            }
            return 0;
        }

        public override void Write(Utf8JsonWriter writer, byte Value, JsonSerializerOptions options)
        {
            throw new JsonException();
            //writer.WriteStringValue($"0x{Value:X}");
        }
    }
}

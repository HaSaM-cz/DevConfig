using Microsoft.VisualBasic.ApplicationServices;
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
            if(Value.IsNumericType())
                writer.WriteStringValue($"0x{Value:X}");
            else
                writer.WriteStringValue($"{Value}");
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class HexUInt32ArrJsonConverter : JsonConverter<List<UInt32>>
    {
        public override List<uint>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null; // Or throw an exception if you don't want to allow null
            else if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException();
            var set = new List<UInt32>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    return set;
                }
                else if (reader.TokenType == JsonTokenType.Number)
                {
                    set.Add(reader.GetUInt32());
                }
                else if (reader.TokenType == JsonTokenType.String)
                {
                    string str = reader.GetString()!;
                    uint i = str.ToUInt32();
                    set.Add(i);
                }
                else
                {
                    //reader.Skip();
                    throw new JsonException(); // Unexpected token;
                }
            }
            throw new JsonException(); // Truncated file;
        }

        public override void Write(Utf8JsonWriter writer, List<uint> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
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
                    return str.ToUInt32();
            }
            return 0;
        }

        public override void Write(Utf8JsonWriter writer, UInt32 Value, JsonSerializerOptions options)
        {
            writer.WriteStringValue($"0x{Value:X}");
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
            writer.WriteStringValue($"0x{Value:X}");
        }
    }
}

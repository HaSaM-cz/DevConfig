using Newtonsoft.Json;


namespace DevConfig.Utils
{
    public sealed class HexStringJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(uint).Equals(objectType);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            writer.WriteValue($"0x{value:X}");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.ValueType != null)
            {
                if (reader.ValueType.FullName == typeof(string).FullName)
                {
                    string? str = (string?)reader.Value;
                    if (str != null)
                    {
                        if (str.StartsWith("0x"))
                            return Convert.ToUInt32(str.Substring("0x".Length), 16);
                        else
                            return Convert.ToUInt32(str);
                    }
                }
                else if (reader.ValueType.FullName == typeof(long).FullName)
                {
                    return reader.Value!;
                }
            }

            throw new JsonSerializationException();
        }
    }
}

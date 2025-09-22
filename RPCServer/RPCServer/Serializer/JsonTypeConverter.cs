using System.Text.Json;
using System.Text.Json.Serialization;

namespace RPCServer.Serializer
{
    public class JsonTypeConverter : JsonConverter<object>
    {
        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Number:
                    if (reader.TryGetInt64(out long l))
                        return l;
                    return reader.GetDouble();
                case JsonTokenType.String:
                    if (reader.TryGetDateTime(out DateTime datetime))
                        return datetime;
                    return reader.GetString();
                case JsonTokenType.True:
                    return true;
                case JsonTokenType.False:
                    return false;
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.StartObject:
                    return JsonSerializer.Deserialize<Dictionary<string, object>>(ref reader, options);
                case JsonTokenType.StartArray:
                    return JsonSerializer.Deserialize<List<object>>(ref reader, options);
                default:
                    throw new JsonException($"지원하지 않는 JsonTokenType: {reader.TokenType}");
            }
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}

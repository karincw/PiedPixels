using RPCServer.DTO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace RPCServer.Serializer
{
    public class JsonRpcSerializer : ISerializer
    {
        private readonly JsonSerializerOptions options;

        public JsonRpcSerializer()
        {
            options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }; 
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.Converters.Add(new JsonTypeConverter());
        }

        public RpcRequestDTO DeSerialize(string data)
        {
            return JsonSerializer.Deserialize<RpcRequestDTO>(data, options);
        }

        public RpcRequestDTO DeSerialize(byte[] data)
        {
            return JsonSerializer.Deserialize<RpcRequestDTO>(Encoding.UTF8.GetString(data), options);
        }

        public string Serialize(RpcResponseDTO data)
        {
            return JsonSerializer.Serialize(data, options);
        }
    }
}

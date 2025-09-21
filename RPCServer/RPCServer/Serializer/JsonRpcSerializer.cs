using RPCServer.DTO;
using System.Text;
using System.Text.Json;
using YamlDotNet.Serialization;

namespace RPCServer.Serializer
{
    public class JsonRpcSerializer : ISerializer
    {
        public RpcRequestDTO DeSerialize(string data)
        {
            return JsonSerializer.Deserialize<RpcRequestDTO>(data);
        }

        public RpcRequestDTO DeSerialize(byte[] data)
        {
            return JsonSerializer.Deserialize<RpcRequestDTO>(Encoding.UTF8.GetString(data));
        }

        public string Serialize(RpcResponseDTO data)
        {
            return JsonSerializer.Serialize(data);
        }
    }
}

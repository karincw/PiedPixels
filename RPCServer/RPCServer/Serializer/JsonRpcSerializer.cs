using RPCServer.DTO;
using System.Text.Json;

namespace RPCServer.Serializer
{
    public class JsonRpcSerializer : ISerializer
    {
        public RpcRequestDTO DeSerialize(string data)
        {
            return JsonSerializer.Deserialize<RpcRequestDTO>(data);
        }
    }
}

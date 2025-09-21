using RPCServer.DTO;

namespace RPCServer.Serializer
{
    public interface ISerializer
    {
        public RpcRequestDTO DeSerialize(string data);
        public string Serialize(RpcResponseDTO data);
    }
}

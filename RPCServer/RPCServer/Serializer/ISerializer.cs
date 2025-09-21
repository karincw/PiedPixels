using RPCServer.DTO;

namespace RPCServer.Serializer
{
    public interface ISerializer
    {
        public RpcRequestDTO DeSerialize(string data);
        public RpcRequestDTO DeSerialize(byte[] data);
        public string Serialize(RpcResponseDTO data);
    }
}

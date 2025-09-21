using RPCServer.DTO;

namespace RPCServer.Serializer
{
    public static class SerializerUtility
    {
        private static readonly ISerializer jsonSerializer = new JsonRpcSerializer();
        private static readonly ISerializer yamlSerializer = new YamlRpcSerializer();

        public static RpcRequestDTO DeSerializer(string data, DataFormat format)
        {
            switch (format)
            {
                case DataFormat.Json:
                    return jsonSerializer.DeSerialize(data);
                case DataFormat.Yaml:
                    return yamlSerializer.DeSerialize(data);
                default:
                    return null;
            }
        }

    }
}

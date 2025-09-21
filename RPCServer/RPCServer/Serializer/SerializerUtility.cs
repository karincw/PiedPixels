using RPCServer.DTO;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                    throw new NotSupportedException($"Unsupported format: {format}");
            }
        }

        public static RpcRequestDTO DeSerializer(byte[] data, DataFormat format)
        {
            switch (format)
            {
                case DataFormat.Json:
                    return jsonSerializer.DeSerialize(data);
                case DataFormat.Yaml:
                    return yamlSerializer.DeSerialize(data);
                default:
                    throw new NotSupportedException($"Unsupported format: {format}");
            }
        }

        public static byte[] Serializer(RpcResponseDTO data, DataFormat format)
        {
            string result;
            switch (format)
            {
                case DataFormat.Json:
                    result = jsonSerializer.Serialize(data);
                    break;
                case DataFormat.Yaml:
                    result = yamlSerializer.Serialize(data);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported format: {format}");
            }
            return Encoding.UTF8.GetBytes(result);
        }

    }
}

using RPCServer.DTO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace RPCServer.Serializer
{
    public class YamlRpcSerializer : ISerializer
    {
        private static readonly IDeserializer deserializer = new DeserializerBuilder()
        .WithNamingConvention(PascalCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

        private static readonly YamlDotNet.Serialization.ISerializer serializer = new SerializerBuilder()
        .WithNamingConvention(PascalCaseNamingConvention.Instance)
        .Build();

        public RpcRequestDTO DeSerialize(string data)
        {
            return deserializer.Deserialize<RpcRequestDTO>(data);
        }
    }
}

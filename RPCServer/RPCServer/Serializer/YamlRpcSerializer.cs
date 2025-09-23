using RPCServer.DTO;
using System.Text;
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
        .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
        // Preserve : 전부 기록 (null) / OmitNull : Null기록안함 / OmitDefaults : Null과 기본값(0, string.empty)기록안함
        .Build();

        public RpcRequestDTO DeSerialize(string data)
        {
            return deserializer.Deserialize<RpcRequestDTO>(data);
        }

        public RpcRequestDTO DeSerialize(byte[] data)
        {
            return deserializer.Deserialize<RpcRequestDTO>(Encoding.UTF8.GetString(data));
        }

        public string Serialize(RpcResponseDTO data)
        {
            return serializer.Serialize(data);
        }
    }
}

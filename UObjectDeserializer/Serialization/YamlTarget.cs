using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace UObjectDeserializer.Serialization
{
    public class YamlTarget : ISerializationTarget
    {
        private Serializer Serializer { get; }
        
        public YamlTarget()
        {
            Serializer = new SerializerBuilder().WithNamingConvention(new CamelCaseNamingConvention()).Build();
        }

        public string Serialize(object? obj)
        {
            return Serializer.Serialize(obj);
        }

        public string Extension { get; } = ".yaml";
    }
}

namespace UObjectDeserializer.Serialization
{
    public interface ISerializationTarget
    {
        public string Serialize(object? obj);
        public string Extension { get; }
    }
}

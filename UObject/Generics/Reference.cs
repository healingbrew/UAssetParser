using System;

using JetBrains.Annotations;

using UObject.Asset;
using UObject.Enum;

namespace UObject.Generics
{
    [PublicAPI]
    public class Reference : IPropertyObject
    {
        public Name    Ref { get; set; } = new Name();
        public string? Key         { get; set; }

        public void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor, SerializationMode mode)
        {
            Ref.Deserialize(buffer, asset, ref cursor);
            Key = ObjectSerializer.DeserializeString(buffer, ref cursor);
        }

        public void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor)
        {
            Deserialize(buffer, asset, ref cursor, SerializationMode.Normal);
        }

        public void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor)
        {
            Ref.Serialize(ref buffer, asset, ref cursor);
            ObjectSerializer.SerializeString(ref buffer, Key, ref cursor);
        }
    }
}

using System;
using System.Diagnostics;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using UObject.Asset;
using UObject.Generics;

namespace UObject.Properties
{
    [PublicAPI]
    public abstract class AbstractGuidProperty : AbstractProperty
    {
        [JsonIgnore]
        public PropertyGuid Guid => (Tag as PropertyGuidTag)?.Guid ?? new PropertyGuid();

        [JsonIgnore]
        public override PropertyTag? Tag { get; set; }

        public override void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor)
        {
            Tag = new PropertyGuidTag();
            Tag.Deserialize(buffer, asset, ref cursor);
        }

        public override void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor) => Tag?.Serialize(ref buffer, asset, ref cursor);
    }
}

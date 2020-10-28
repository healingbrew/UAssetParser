using System;
using System.Linq;
using System.Text.Json.Serialization;
using DragonLib.IO;
using JetBrains.Annotations;
using UObject.Asset;
using UObject.JSON;

namespace UObject.Generics
{
    [PublicAPI]
    public class Name : ISerializableObject, IValueType<string?>
    {
        [JsonIgnore]
        public int Index { get; set; }

        [JsonIgnore]
        public int InstanceNum { get; set; }

        public void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor)
        {
            Index = SpanHelper.ReadLittleInt(buffer, ref cursor);
            InstanceNum = SpanHelper.ReadLittleInt(buffer, ref cursor);
            if (asset.Names.Length < Index || Index < 0) return;
            Value = asset.Names[Index].Name;

            if (asset.Options?.StripNames != true) return;

            var parts = Value?.Split('_') ?? Array.Empty<string>();

            if (parts.Length >= 3 && parts[^1].Length == 32 && parts[^2].Length > 0 && parts[^2].All(char.IsDigit))
            {
                Value = string.Join('_', parts[..^2]);
            }
        }

        public void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor)
        {
            SpanHelper.WriteLittleInt(ref buffer, Index, ref cursor);
            SpanHelper.WriteLittleInt(ref buffer, InstanceNum, ref cursor);
            // TODO: Recalculate Index.
        }

        public string? Value { get; set; }

        public static implicit operator string(Name? name) => name?.Value ?? "None";

        public override string? ToString() => Value;
    }
}

using System;
using System.Diagnostics;

using JetBrains.Annotations;
using UObject.Generics;

namespace UObject.Asset
{
    [PublicAPI]
    public class ObjectImport : ISerializableObject
    {
        public Name ClassPackage { get; set; } = new Name();
        public Name ClassName { get; set; } = new Name();
        public PackageIndex PackageRef { get; set; } = new PackageIndex();
        public Name ObjectName { get; set; } = new Name();

        public void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor)
        {
            ClassPackage.Deserialize(buffer, asset, ref cursor);
            ClassName.Deserialize(buffer, asset, ref cursor);
            PackageRef.Deserialize(buffer, asset, ref cursor);
            ObjectName.Deserialize(buffer, asset, ref cursor);
        }

        public void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor)
        {
            ClassPackage.Serialize(ref buffer, asset, ref cursor);
            ClassName.Serialize(ref buffer, asset, ref cursor);
            PackageRef.Serialize(ref buffer, asset, ref cursor);
            ObjectName.Serialize(ref buffer, asset, ref cursor);
        }
    }
}

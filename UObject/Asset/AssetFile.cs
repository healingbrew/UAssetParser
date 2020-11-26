using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DragonLib.IO;
using JetBrains.Annotations;

using UObject.Generics;

namespace UObject.Asset
{
    [PublicAPI]
    public class AssetFile
    {
        public AssetFile(Span<byte> uasset, Span<byte> uexp, AssetFileOptions options)
        {
            Options = options.Clone();
            var cursor  = 0;
            var maxSize = Math.Max(uasset.Length, uexp.Length);
            Summary = new PackageFileSummary();
            Summary.Deserialize(uasset, this, ref cursor);
            cursor = Summary.NameOffset;
            Names = ObjectSerializer.DeserializeProperties<NameEntry>(uasset, this, Summary.NameCount, ref cursor);
            cursor = Summary.ImportOffset;
            Imports = ObjectSerializer.AllocateProperties<ObjectImport>(Summary.ImportCount);
            ObjectSerializer.DeserializeProperties(uasset, this, Imports, ref cursor);
            cursor = Summary.ExportOffset;
            Exports = ObjectSerializer.AllocateProperties<ObjectExport>(Summary.ExportCount);
            ObjectSerializer.DeserializeProperties(uasset, this, Exports, ref cursor);
            if(Exports.Any(x => x.SerialOffset < Summary.TotalHeaderSize || x.SerialSize >= maxSize)) throw new InvalidDataException();
            cursor = Summary.PreloadDependencyOffset;
            PreloadDependencies = SpanHelper.ReadStructArray<PreloadDependencyIndex>(uasset, Summary.PreloadDependencyCount, ref cursor);

            foreach (var export in Exports)
            {
                try
                {
                    ExportObjects[export.ObjectName] = ObjectSerializer.DeserializeObject(this, export, uasset, uexp);
                }
                catch (Exception e)
                {
                    if(options.Throw) throw new Exception(export.ClassIndex.Name ?? "None", e);
                }
            }
        }

        public AssetFile() { }

        public AssetFileOptions? Options { get; set; }

        public PackageFileSummary Summary { get; set; } = new PackageFileSummary();
        public NameEntry[] Names { get; set; } = new NameEntry[0];
        public ObjectImport[] Imports { get; set; } = new ObjectImport[0];
        public ObjectExport[] Exports { get; set; } = new ObjectExport[0];
        public PreloadDependencyIndex[] PreloadDependencies { get; set; } = new PreloadDependencyIndex[0];

        public Dictionary<string, ISerializableObject> ExportObjects { get; set; } = new Dictionary<string, ISerializableObject>();
        public bool IsSupported => Exports.All(ObjectSerializer.IsSupported);

        // Update META and Summary, recalculate values
        // Serialize serial data
        // Write data
        public void Serialize() => throw new NotImplementedException();
    }
}

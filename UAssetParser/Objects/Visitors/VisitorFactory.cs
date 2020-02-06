using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UAssetParser.Formats;
using UAssetParser.Objects.Visitors.Unreal;

namespace UAssetParser.Objects.Visitors
{
    public static class VisitorFactory
    {
        public delegate FPropertyTag PropertyVisitor(BinaryReader reader, FPropertyTag baseTag, FPackageFileSummary summary);
        public delegate IExportObject ExportVisitor(BinaryReader reader, FObjectExport export, FPackageFileSummary summary);
        public delegate IStructObject StructVisitor(BinaryReader reader, FPackageFileSummary summary);
        public delegate object EnumerableVisitor(BinaryReader binaryReader, FPackageFileSummary summary, int count);

        public readonly static Dictionary<string, PropertyVisitor> Visitors = new Dictionary<string, PropertyVisitor>();
        public readonly static Dictionary<string, ExportVisitor> ExportVisitors = new Dictionary<string, ExportVisitor>();
        public readonly static Dictionary<string, StructVisitor> StructVisitors = new Dictionary<string, StructVisitor>();
        public readonly static Dictionary<string, EnumerableVisitor> EnumerableVisitors = new Dictionary<string, EnumerableVisitor>
        {
            { "StructProperty", (r,s,c) => {
                var cached = r.BaseStream.Position;
                try {
                    var instance = LSerializer.Deserialize<UStructProperty>(r);
                    instance.Ref(s);
                    var arr = new object[c];
                    for(int i = 0; i < c; ++i)
                    {
                        arr[i] = (object)VisitStruct(r, instance.StructName, s) ?? new UObject(r, s, false, null);
                    }
                    instance.Struct = arr;
                    return instance;
                }
                catch
                {
                    r.BaseStream.Position = cached;
                    return new UObject(r, s, false, null);
                }
            } },
            { "BoolProperty", (r,s,c) => r.ReadByte() == 1 },
            { "IntProperty", (r,s,c) => r.ReadInt32() },
            { "UInt32Property", (r,s,c) => r.ReadUInt32() },
            { "ByteProperty", (r,s,c) =>
            {
                if (c == 1)
                    return r.ReadByte();
                var fk = LSerializer.Deserialize<FName>(r);
                fk.Ref(s);
                return fk;
            } },
            { "FloatProperty", (r,s,c) => r.ReadSingle() },
            { "StrProperty", (r,s,c) => LSerializer.FString(r) },
            { "EnumProperty", (r,s,c) =>
            {
                var fk = LSerializer.Deserialize<FName>(r);
                fk.Ref(s);
                return fk;
            } },
            { "NameProperty", (r,s,c) =>
            {
                var fk = LSerializer.Deserialize<FName>(r);
                fk.Ref(s);
                return fk;
            } },
            { "TextProperty", (r,s,c) =>
            {
                var _ = r.ReadInt64();
                var __ = LSerializer.Deserialize<FPropertyGuid>(r);
                return new object[] { LSerializer.FString(r), LSerializer.FString(r) };
            } },
            { "SoftObjectProperty", (r,s,c) =>
            {
                var fn = LSerializer.Deserialize<FName>(r);
                fn.Ref(s);
                var fs = LSerializer.FString(r);
                if(string.IsNullOrWhiteSpace(fs)) return fn;
                return new object[] { fn, fs };
            } },
            { "ObjectProperty", (r,s,c) =>
            {
                var pi = LSerializer.Deserialize<FPackageIndex>(r);
                pi.Ref(s);
                return pi;
            } }
        };

        [DebuggerHidden]
        public static FPropertyTag Visit(BinaryReader reader, FPropertyTag baseTag, FPackageFileSummary summary)
        {
            if (Visitors.TryGetValue(baseTag.Type.Name, out var visitor)) return visitor(reader, baseTag, summary);
            throw new NotImplementedException("No parser for " + baseTag.Type.Name);
        }

        [DebuggerHidden]
        public static object VisitEnumerable(BinaryReader reader, string type, FPackageFileSummary summary, int count)
        {
            if (EnumerableVisitors.TryGetValue(type, out var visitor)) return visitor(reader, summary, count);
            throw new NotImplementedException("No parser for " + type);
        }

        public static void SetEnumAsByte()
        {
            // if (Visitors.TryGetValue("EnumProperty", out var visitor)) Visitors["ByteProperty"] = visitor;
        }

        [DebuggerHidden]
        internal static IStructObject VisitStruct(BinaryReader reader, string structName, FPackageFileSummary summary)
        {
            if (StructVisitors.TryGetValue(structName, out var visitor)) return visitor(reader, summary);
            return null;
        }

        [DebuggerHidden]
        public static IExportObject VisitSubtype(BinaryReader reader, FObjectExport export, FPackageFileSummary summary)
        {
            if (ExportVisitors.TryGetValue(export.ClassIndex.Name, out var visitor)) return visitor(reader, export, summary);
            return null;
        }

        public static void LoadVisitors(string gameType = "Unreal", Assembly loadFrom = null)
        {
            loadFrom ??= Assembly.GetExecutingAssembly();
            LoadExportVisitors(gameType, loadFrom);
            LoadStructVisitors(gameType, loadFrom);

            var baseType = typeof(FPropertyTag);
            var visitors = loadFrom.GetTypes().Where(x => baseType.IsAssignableFrom(x) && x.GetCustomAttribute<CategoryAttribute>()?.Category == gameType);
            foreach (var visitor in visitors)
            {
                var type = visitor.GetCustomAttribute<DescriptionAttribute>()?.Description;
                if (string.IsNullOrWhiteSpace(type)) throw new InvalidDataException(visitor.FullName + " has no DescriptionAttribute!");
                var method = visitor.GetMethods(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public).Select(CreateDelegate).Where(x => x != null).FirstOrDefault();
                Visitors[type] = method ?? CreateTagDeserializer(visitor);
            }
        }

        public static void LoadExportVisitors(string gameType = "Unreal", Assembly loadFrom = null)
        {
            loadFrom ??= Assembly.GetExecutingAssembly();

            var baseType = typeof(IExportObject);
            var visitors = loadFrom.GetTypes().Where(x => baseType.IsAssignableFrom(x) && x.GetCustomAttribute<CategoryAttribute>()?.Category == gameType);
            foreach (var visitor in visitors)
            {
                var type = visitor.GetCustomAttribute<DescriptionAttribute>()?.Description;
                if (string.IsNullOrWhiteSpace(type)) throw new InvalidDataException(visitor.FullName + " has no DescriptionAttribute!");
                var method = visitor.GetMethods(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public).Select(CreateExportDelegate).Where(x => x != null).FirstOrDefault();
                ExportVisitors[type] = method ?? throw new NotImplementedException(visitor.FullName + " has no Export Delegate!");
            }
        }

        public static void LoadStructVisitors(string gameType = "Unreal", Assembly loadFrom = null)
        {
            loadFrom ??= Assembly.GetExecutingAssembly();

            var baseType = typeof(IStructObject);
            var visitors = loadFrom.GetTypes().Where(x => baseType.IsAssignableFrom(x) && x.GetCustomAttribute<CategoryAttribute>()?.Category == gameType);
            foreach (var visitor in visitors)
            {
                var type = visitor.GetCustomAttribute<DescriptionAttribute>()?.Description;
                if (string.IsNullOrWhiteSpace(type)) throw new InvalidDataException(visitor.FullName + " has no DescriptionAttribute!");
                var method = visitor.GetMethods(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public).Select(CreateStructDelegate).Where(x => x != null).FirstOrDefault();
                StructVisitors[type] = method ?? CreateStructDeserializer(visitor);
            }
        }

        private static PropertyVisitor CreateDelegate(MethodInfo info)
        {
            try
            {
                return (PropertyVisitor)info.CreateDelegate(typeof(PropertyVisitor));
            }
            catch
            {
                return null;
            }
        }

        private static ExportVisitor CreateExportDelegate(MethodInfo info)
        {
            try
            {
                return (ExportVisitor)info.CreateDelegate(typeof(ExportVisitor));
            }
            catch
            {
                return null;
            }
        }

        private static StructVisitor CreateStructDelegate(MethodInfo info)
        {
            try
            {
                return (StructVisitor)info.CreateDelegate(typeof(StructVisitor));
            }
            catch
            {
                return null;
            }
        }

        private static PropertyVisitor CreateTagDeserializer(Type visitor)
        {
            return (r, b, s) => (FPropertyTag)LSerializer.Deserialize(r, visitor);
        }

        private static StructVisitor CreateStructDeserializer(Type visitor)
        {
            return (r, s) => (IStructObject)LSerializer.Deserialize(r, visitor);
        }
    }
}

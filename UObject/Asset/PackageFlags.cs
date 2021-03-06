using JetBrains.Annotations;
using System;

namespace UObject.Asset
{
    [PublicAPI]
    [Flags]
    public enum PackageFlags : uint
    {
        None = 0,
        NewlyCreated = 1 << 0,
        ClientOptional = 1 << 1,
        ServerSideOnly = 1 << 2,
        Unused8 = 1 << 3,
        CompiledIn = 1 << 4,
        ForDiffing = 1 << 5,
        EditorOnly = 1 << 6,
        Developer = 1 << 7,
        UncookedOnly = 1 << 8,
        Unused200 = 1 << 9,
        Unused400 = 1 << 10,
        Unused800 = 1 << 11,
        Unused1000 = 1 << 12,
        UnversionedProperties = 1 << 13,
        ContainsMapData = 1 << 14,
        Unused8000 = 1 << 15,
        Compiling = 1 << 16,
        ContainsMap = 1 << 17,
        RequiresLocalizationGather = 1 << 18,
        Unused80000 = 1 << 19,
        PlayInEditor = 1 << 20,
        ContainsScript = 1 << 21,
        DisallowExport = 1 << 22,
        Unused800000 = 1 << 23,
        Unused1000000 = 1 << 24,
        Unused2000000 = 1 << 25,
        Unused4000000 = 1 << 26,
        Unused8000000 = 1 << 27,
        DynamicImports = 1 << 28,
        RuntimeGenerated = 1 << 29,
        ReloadingForCooker = 1 << 30,
        FilterEditorOnly = 1U << 31
    }
}

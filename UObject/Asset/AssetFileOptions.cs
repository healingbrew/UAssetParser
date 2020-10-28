using JetBrains.Annotations;
using UObject.Enum;

namespace UObject.Asset
{
    [PublicAPI]
    public class AssetFileOptions
    {
        public const int LATEST_UNREAL_VERSION = 524;
        public int UnrealVersion { get; set; } = LATEST_UNREAL_VERSION;
        public UnrealGame Workaround { get; set; } = UnrealGame.None;
        public bool StripNames { get; set; }
        public bool TryDifferentVersion { get; set; }
        public bool Dry { get; set; }
    }
}

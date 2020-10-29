using System.Collections.Generic;
using JetBrains.Annotations;
using UObject.Enum;

namespace UObject.Asset
{
    [PublicAPI]
    public class AssetFileOptions
    {
        public const int LATEST_UNREAL_VERSION = 521;
        public const int LATEST_SUPPORTED_UNREAL_VERSION = 515;
        public const int MINIMUM_SUPPORTED_UNREAL_VERSION = 510;
        public int UnrealVersion { get; set; } = LATEST_SUPPORTED_UNREAL_VERSION;
        public UnrealGame Workaround { get; set; } = UnrealGame.None;
        public bool StripNames { get; set; }
        public bool Dry { get; set; }

        public AssetFileOptions Clone()
        {
            return new AssetFileOptions
            {
                UnrealVersion = UnrealVersion,
                Dry = Dry,
                StripNames = StripNames,
                Workaround = Workaround,
            };
        }
    }
}

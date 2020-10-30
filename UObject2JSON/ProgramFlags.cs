using System.Collections.Generic;
using DragonLib.CLI;
using JetBrains.Annotations;
using UObject.Asset;
using UObject.Enum;

namespace UObject2JSON
{
    [PublicAPI]
    public class ProgramFlags : ICLIFlags
    {
        [UsedImplicitly]
        [CLIFlag("paths", Positional = 0, Category = "Program Arguments", Help = "Paths to load")]
        public List<string> Paths { get; set; } = new List<string>();

        [CLIFlag("output", Aliases = new[] { "o" }, Category = "Program Arguments", Help = "Folder to output JSON files to")]
        public string? OutputFolder { get; set; }

        [CLIFlag("workaround", Default = UnrealGame.None, Aliases = new[] { "w" }, Category = "Program Arguments", Help = "Game-specific workaround")]
        public UnrealGame Workaround { get; set; }

        [CLIFlag("changeset", Aliases = new[] { "c" }, Category = "Program Arguments", Help = "Unreal changeset to parse with if unspecified by the asset, can specify multiple")]
        public List<int>? UnrealVersions { get; set; }

        [CLIFlag("typeless", Default = false, Aliases = new[] { "t" }, Category = "Program Arguments", Help = "Do not store type information")]
        public bool Typeless { get; set; }

        [CLIFlag("force-dict-key", Default = false, Aliases = new[] { "T" }, Category = "Program Arguments", Help = "Assume all dictionary keys are strings")]
        public bool EnforceDictionaryKeys { get; set; }

        [CLIFlag("dry", Default = false, Aliases = new[] { "n" }, Category = "Program Arguments", Help = "Only parse asset information, not actual serial info")]
        public bool Dry { get; set; }

        [CLIFlag("quiet", Default = false, Aliases = new[] { "q" }, Category = "Program Arguments", Help = "Suppress output messages")]
        public bool Quiet { get; set; }

        [CLIFlag("strip", Default = false, Aliases = new[] { "s" }, Category = "Program Arguments", Help = "Strip hashes and indices from DataTable names")]
        public bool StripNames { get; set; }

        [UsedImplicitly]
        [CLIFlag("game", Aliases = new[] { "g" }, Category = "Program Arguments", Help = "Game DLL to load")]
        public List<string>? GameModels { get; set; }
    }
}

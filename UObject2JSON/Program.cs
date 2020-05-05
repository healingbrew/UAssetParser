using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using DragonLib.CLI;
using DragonLib.IO;
using DragonLib.JSON;
using JetBrains.Annotations;
using UObject;
using UObject.Asset;
using UObject.JSON;

namespace UObject2JSON
{
    [PublicAPI]
    internal class Program
    {
        public enum ErrorCodes
        {
            Success = 0,
            Crash = 1,
            FlagError = -1,
            NotSupported = 2
        }

        private static int Main(string[] args)
        {
            Logger.PrintVersion("UAsset");
            var flags = CommandLineFlags.ParseFlags<ProgramFlags>(CommandLineFlags.PrintHelp, args);
            if (flags == null) return (int) ErrorCodes.FlagError;
            var paths = new List<string>();
            // TODO: Move to DragonLib
            foreach (var path in flags.Paths)
            {
                if (Directory.Exists(path))
                    paths.AddRange(Directory.GetFiles(path, "*.uasset", SearchOption.AllDirectories));
                else if (File.Exists(path)) paths.Add(path);
            }

            var settings = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true,
                Converters =
                {
                    flags.Typeless ? (JsonConverter) new GenericTypelessDictionaryConverterFactory() : new GenericDictionaryConverterFactory(),
                    flags.Typeless ? (JsonConverter) new GenericTypelessListConverterFactory() : new GenericListConverterFactory(),
                    new ValueTypeConverterFactory(flags.Typeless),
                    new UnrealObjectConverter(),
                    new NoneStringConverter()
                }
            };

            var options = new AssetFileOptions
            {
                UnrealVersion = flags.UnrealVersion,
                Workaround = flags.Workaround,
                Dry = flags.Dry
            };

            foreach (var path in paths)
            {
                var arg = Path.Combine(Path.GetDirectoryName(path) ?? ".", Path.GetFileNameWithoutExtension(path));
                var uasset = File.ReadAllBytes(arg + ".uasset");
                var uexp = File.Exists(arg + ".uexp") ? File.ReadAllBytes(arg + ".uexp") : Span<byte>.Empty;
                if (!flags.Quiet) Logger.Info("UAsset", arg);
                try
                {
                    var asset = ObjectSerializer.Deserialize(uasset, uexp, options);
                    if (flags.Dry)
                    {
                        if (!asset.IsSupported) return (int) ErrorCodes.NotSupported;
                        continue;
                    }

                    var json = JsonSerializer.Serialize(asset.ExportObjects, settings);

                    if (!string.IsNullOrWhiteSpace(flags.OutputFolder))
                    {
                        arg = Path.Combine(flags.OutputFolder, Path.GetFileName(arg));
                        if (!Directory.Exists(flags.OutputFolder)) Directory.CreateDirectory(flags.OutputFolder);
                    }

                    File.WriteAllText(arg + ".json", json);
                }
                catch (Exception e)
                {
                    Logger.Fatal("UAsset", e);
                    return (int) ErrorCodes.Crash;
                }
            }

            return (int) ErrorCodes.Success;
        }
    }
}

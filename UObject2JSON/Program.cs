﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
        [Flags]
        public enum ErrorCodes
        {
            Success      = 0,
            Crash        = 1 << 0,
            FlagError    = 1 << 1,
            NotSupported = 1 << 2,
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

            var executingDir = Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location) ?? "./";

            foreach (var asmName in flags.GameModels ?? new List<string>())
            {
                if (!flags.Quiet) Logger.Info("UAsset", $"Loading plugin {asmName}");

                if (File.Exists(asmName))
                    ObjectSerializer.LoadGameModel(Path.Combine(executingDir, asmName));
                else if (File.Exists(Path.Combine(executingDir, asmName)))
                    ObjectSerializer.LoadGameModel(Path.Combine(executingDir, asmName));
                else if (File.Exists(Path.Combine(executingDir, $"{asmName}.dll")))
                    ObjectSerializer.LoadGameModel(Path.Combine(executingDir, $"{asmName}.dll"));
                else if (File.Exists(Path.Combine(executingDir, $"UObject.{asmName}")))
                    ObjectSerializer.LoadGameModel(Path.Combine(executingDir, $"UObject.{asmName}"));
                else if (File.Exists(Path.Combine(executingDir, $"UObject.{asmName}.dll")))
                    ObjectSerializer.LoadGameModel(Path.Combine(executingDir, $"UObject.{asmName}.dll"));
                else
                    Logger.Error("UAsset", $"Could not resolve {asmName}!");
            }

            var settings = new JsonSerializerOptions
            {
                Encoder       = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true,
                Converters =
                {
                    flags.Typeless ? (JsonConverter) new GenericTypelessDictionaryConverterFactory() : new GenericDictionaryConverterFactory(),
                    flags.Typeless ? (JsonConverter) new GenericTypelessListConverterFactory() : new GenericListConverterFactory(),
                    new ValueTypeConverterFactory(flags.Typeless),
                    new NameDictionaryConverterFactory(),
                    new UnrealObjectConverter(),
                    new NoneStringConverter(),
                }
            };

            if (flags.UnrealVersions == null || flags.UnrealVersions.Count == 0)
            {
                flags.UnrealVersions = new List<int>
                {
                    AssetFileOptions.LATEST_SUPPORTED_UNREAL_VERSION,
                    513,
                    AssetFileOptions.MINIMUM_SUPPORTED_UNREAL_VERSION,
                };
            }


            var ecode = ErrorCodes.Success;

            foreach (var path in paths)
            {
                var success = false;
                var arg     = Path.Combine(Path.GetDirectoryName(path) ?? ".", Path.GetFileNameWithoutExtension(path));
                var uasset  = File.ReadAllBytes(arg + ".uasset");
                var uexp    = File.Exists(arg + ".uexp") ? File.ReadAllBytes(arg + ".uexp") : Span<byte>.Empty;
                if (!flags.Quiet) Logger.Info("UAsset", $"Parsing {arg}...");
                
                foreach (var unrealVersion in flags.UnrealVersions)
                {
                    var options = new AssetFileOptions
                    {
                        UnrealVersion = unrealVersion,
                        Workaround    = flags.Workaround,
                        Dry           = flags.Dry,
                        StripNames    = flags.StripNames,
                    };

                    if (!flags.Quiet) Logger.Info("UAsset", $"Trying version {unrealVersion}...");

                    try
                    {
                        var asset = ObjectSerializer.Deserialize(uasset, uexp, options);
                        success = true;
                        if (flags.Dry)
                        {
                            if (!asset.IsSupported) ecode |= ErrorCodes.NotSupported;

                            continue;
                        }

                        var json = JsonSerializer.Serialize(asset.ExportObjects, settings);

                        if (!string.IsNullOrWhiteSpace(flags.OutputFolder))
                        {
                            arg = Path.Combine(flags.OutputFolder, Path.GetFileName(arg));
                            if (!Directory.Exists(flags.OutputFolder)) Directory.CreateDirectory(flags.OutputFolder);
                        }

                        File.WriteAllText(arg + ".json", json);
                        break;
                    }
                    catch (Exception e)
                    {
                        Logger.Fatal("UAsset", e);
                    }
                }

                if (!success)
                {
                    ecode |= ErrorCodes.Crash;
                }
            }

            return (int) ecode;
        }
    }
}

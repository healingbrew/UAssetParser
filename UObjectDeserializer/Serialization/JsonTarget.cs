using DragonLib.JSON;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using UObject.JSON;

namespace UObjectDeserializer.Serialization
{
    public class JsonTarget : ISerializationTarget
    {
        private JsonSerializerOptions Settings { get; }
        
        public JsonTarget(ProgramFlags flags)
        {
            Settings = new JsonSerializerOptions
            {
                Encoder       = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true,
                Converters =
                {
                    flags.Typeless ? (JsonConverter) new GenericTypelessDictionaryConverterFactory(flags.EnforceDictionaryKeys, typeof(IValueType<string>)) : new GenericDictionaryConverterFactory(),
                    flags.Typeless ? (JsonConverter) new GenericTypelessListConverterFactory() : new GenericListConverterFactory(),
                    new ValueTypeConverterFactory(flags.Typeless),
                    new NameDictionaryConverterFactory(),
                    new UnrealObjectConverter(),
                    new NoneStringConverter()
                }
            };
        }

        public string Serialize(object? obj)
        {
            return JsonSerializer.Serialize(obj, Settings);
        }

        public string Extension { get; } = ".json";
    }
}

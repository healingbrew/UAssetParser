using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using UObject.Generics;

namespace UObject.JSON
{
    [PublicAPI]
    public class NameDictionaryConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType) return false;

            return typeToConvert.GetGenericTypeDefinition() == typeof(Dictionary<,>) && typeToConvert.GetGenericArguments()[0].IsEquivalentTo(typeof(Name));
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) => (JsonConverter) (Activator.CreateInstance(typeof(NameDictionaryConverter<>).MakeGenericType(typeToConvert.GetGenericArguments()[1])) ?? throw new JsonException());
    }
}

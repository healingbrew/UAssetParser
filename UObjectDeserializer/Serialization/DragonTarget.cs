﻿using DragonLib.XML;

namespace UObjectDeserializer.Serialization
{
    public class DragonTarget : ISerializationTarget
    {
        public string Serialize(object? obj)
        {
            return DragonML.Print(obj, DragonMLSettings.Slim) ?? string.Empty;
        }

        public string Extension { get; } = ".dragonml";
    }
}

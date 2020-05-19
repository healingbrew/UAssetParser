using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace UObject
{
    [PublicAPI]
    public abstract class GameModel
    {
        protected GameModel(Dictionary<string, Type> classTypes, Dictionary<string, Type> propertyTypes, Dictionary<string, Type> structTypes)
        {
        }
    }
}

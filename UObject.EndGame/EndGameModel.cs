using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UObject.EndGame.ObjectModel;

namespace UObject.EndGame
{
    [PublicAPI]
    public class EndGameModel : GameModel
    {
        public EndGameModel(Dictionary<string, Type> classTypes, Dictionary<string, Type> propertyTypes, Dictionary<string, Type> structTypes) : base(classTypes, propertyTypes, structTypes) => classTypes[nameof(EndTextResource)] = typeof(EndTextResource);
    }
}

﻿using System.ComponentModel;
using System.Runtime.Serialization;
using UAssetParser.Formats;

namespace UAssetParser.Objects.Visitors.Unreal
{
    [DataContract, Description("IntProperty"), Category("Unreal")]
    public class UIntProperty : FPropertyTag
    {
        [DataMember]
        public FPropertyGuid Guid { get; set; }

        [DataMember]
        public int Value { get; set; }

        public override object GetValue()
        {
            return Value;
        }
    }
}

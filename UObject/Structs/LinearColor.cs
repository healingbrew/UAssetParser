using System.Runtime.InteropServices;

using JetBrains.Annotations;

namespace UObject.Structs
{
    [PublicAPI]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LinearColor
    {
        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }
        public float A { get; set; }
    }
}

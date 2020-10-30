using System.Runtime.InteropServices;

using JetBrains.Annotations;

namespace UObject.Structs
{
    [PublicAPI]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Color
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte A { get; set; }
    }
}

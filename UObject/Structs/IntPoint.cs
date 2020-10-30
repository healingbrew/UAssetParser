using System.Runtime.InteropServices;

using JetBrains.Annotations;

namespace UObject.Structs
{
    [PublicAPI]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IntPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}

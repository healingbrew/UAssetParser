using System.Runtime.InteropServices;

using JetBrains.Annotations;

namespace UObject.Structs
{
    [PublicAPI]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vector2D
    {
        public float X { get; set; }
        public float Y { get; set; }
    }
}

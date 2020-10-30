using System.Runtime.InteropServices;

using JetBrains.Annotations;

namespace UObject.Structs
{
    [PublicAPI]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Box2D
    {
        public Vector2D Min { get; set; }
        public Vector2D Max { get; set; }
        public byte IsValid { get; set; }
    }
}

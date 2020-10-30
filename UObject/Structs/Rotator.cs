using System.Runtime.InteropServices;

using JetBrains.Annotations;

namespace UObject.Structs
{
    [PublicAPI]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Rotator
    {
        public float Pitch { get; set; }
        public float Yaw   { get; set; }
        public float Roll  { get; set; }
    }
}

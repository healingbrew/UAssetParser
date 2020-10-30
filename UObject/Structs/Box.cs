using System.Runtime.InteropServices;

using JetBrains.Annotations;

namespace UObject.Structs
{
    [PublicAPI]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Box
    {
        public Vector Min { get; set; }
        public Vector Max { get; set; }
        public byte IsValid { get; set; }
    }
}

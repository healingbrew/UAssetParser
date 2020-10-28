using JetBrains.Annotations;

namespace UObject.Structs
{
    [PublicAPI]
    public struct Quat
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }
    }
}

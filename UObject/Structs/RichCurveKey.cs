using System.Runtime.InteropServices;

using JetBrains.Annotations;

namespace UObject.Structs
{
    [PublicAPI]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RichCurveKey
    {
        public byte  InterpolationMode { get; set; }
        public byte  TangentMode       { get; set; }
        public byte  WeightMode        { get; set; }
        public float Time              { get; set; }
        public float Value             { get; set; }
        public float Arrive            { get; set; }
        public float ArriveWeight      { get; set; }
        public float Leave             { get; set; }
        public float LeaveWeight       { get; set; }
    }
}

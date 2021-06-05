
using System.Runtime.InteropServices;

namespace RoboPhredDev.Shipbreaker.SixAxis.Native.HID
{
    [StructLayout(LayoutKind.Sequential)]
    struct HidPCapsRange
    {
        public ushort UsageMin;
        public ushort UsageMax;
        public ushort StringMin;
        public ushort StringMax;
        public ushort DesignatorMin;
        public ushort DesignatorMax;
        public ushort DataIndexMin;
        public ushort DataIndexMax;
    }
}
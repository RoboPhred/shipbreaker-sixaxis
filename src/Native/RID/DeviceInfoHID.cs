using System;
using System.Runtime.InteropServices;

namespace RoboPhredDev.Shipbreaker.SixAxis.Native.RID
{

    [StructLayout(LayoutKind.Sequential)]
    struct DeviceInfoHID
    {
        public int dwVendorId;
        public int dwProductId;
        public int dwVersionNumber;
        public ushort usUsagePage;
        public ushort usUsage;
    }
}

using System;

namespace RoboPhredDev.Shipbreaker.SixAxis.Native.RID
{
    struct DeviceRegistration
    {
        public ushort usUsagePage;

        public ushort usUsage;

        public DeviceFlags dwFlags;

        public IntPtr hwndTarget;
    }
}
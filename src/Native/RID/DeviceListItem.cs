using System;

namespace RoboPhredDev.Shipbreaker.SixAxis.Native.RID
{
    public struct DeviceListItem
    {
        public IntPtr hDevice { get; set; }

        public DeviceType dwType { get; set; }
    }
}
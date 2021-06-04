using System;
using RoboPhredDev.Shipbreaker.SixAxis.Native.RID;

namespace RoboPhredDev.Shipbreaker.SixAxis.Input
{
    class RawInputHidDevice : RawInputDevice
    {
        private readonly DeviceInfoHID deviceInfo;

        public ushort Usage => deviceInfo.usPage;

        public ushort UsagePage => deviceInfo.usUsagePage;

        public RawInputHidDevice(IntPtr handle) : base(handle)
        {
            var info = RIDInterop.GetRawInputDeviceInfo(handle);
            if (info.dwType != DeviceType.RIM_TYPEHID)
            {
                throw new ArgumentException("Handle is not a HID device.");
            }
            this.deviceInfo = info.hid;
        }
    }
}
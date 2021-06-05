
using System;

namespace RoboPhredDev.Shipbreaker.SixAxis.Input
{
    class RawInputMouseDevice : RawInputDevice
    {
        public RawInputMouseDevice(IntPtr handle) : base(handle)
        {
        }

        public override ushort UsagePage => (ushort)Input.UsagePage.GenericDesktop;

        public override ushort Usage => (ushort)GenericDesktopUsage.Mouse;
    }
}
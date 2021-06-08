
using System;

namespace RoboPhredDev.Shipbreaker.SixAxis.RawInput
{
    class RawInputMouseDevice : RawInputDevice
    {
        public RawInputMouseDevice(IntPtr handle) : base(handle)
        {
        }

        public override ushort UsagePage => (ushort)RawInput.UsagePage.GenericDesktop;

        public override ushort Usage => (ushort)GenericDesktopUsage.Mouse;
    }
}
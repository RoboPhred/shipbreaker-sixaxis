
using System;

namespace RoboPhredDev.Shipbreaker.SixAxis.Input
{
    class RawInputKeyboardDevice : RawInputDevice
    {
        public RawInputKeyboardDevice(IntPtr handle) : base(handle)
        {
        }

        public override ushort UsagePage => (ushort)Input.UsagePage.GenericDesktop;

        public override ushort Usage => (ushort)GenericDesktopUsage.Keyboard;
    }
}
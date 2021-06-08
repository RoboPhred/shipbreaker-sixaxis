
using System;

namespace RoboPhredDev.Shipbreaker.SixAxis.RawInput
{
    class RawInputKeyboardDevice : RawInputDevice
    {
        public RawInputKeyboardDevice(IntPtr handle) : base(handle)
        {
        }

        public override ushort UsagePage => (ushort)RawInput.UsagePage.GenericDesktop;

        public override ushort Usage => (ushort)GenericDesktopUsage.Keyboard;
    }
}
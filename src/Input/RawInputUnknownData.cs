using System;
using RoboPhredDev.Shipbreaker.SixAxis.Native.RID;

namespace RoboPhredDev.Shipbreaker.SixAxis.Input
{
    class RawInputUnknownData : RawInputData
    {
        public RawInputUnknownData(IntPtr headerHandle)
        {
            var header = RawInputInterop.GetRawInputHeader(headerHandle);
            Device = RawInputDevice.FromHandle(header.hDevice);
        }

        public override RawInputDevice Device { get; }
    }
}
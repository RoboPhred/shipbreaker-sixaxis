
using System;
using RoboPhredDev.Shipbreaker.SixAxis.Native.RID;

namespace RoboPhredDev.Shipbreaker.SixAxis.RawInput
{
    abstract class RawInputData
    {
        public abstract RawInputDevice Device { get; }

        public static RawInputData FromHandle(IntPtr handle)
        {
            var header = RawInputInterop.GetRawInputHeader(handle);

            return (DeviceType)header.dwType switch
            {
                DeviceType.RIM_TYPEHID => new RawInputHidData(handle),
                _ => new RawInputUnknownData(handle),
            };
        }
    }
}
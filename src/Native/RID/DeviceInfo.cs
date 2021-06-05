

using System.Runtime.InteropServices;

namespace RoboPhredDev.Shipbreaker.SixAxis.Native.RID
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-rid_device_info
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    struct DeviceInfo
    {
        [FieldOffset(0)]
        public int sbSize;
        [FieldOffset(4)]
        public DeviceType dwType;
        [FieldOffset(8)]
        public DeviceInfoMouse mouse;
        [FieldOffset(8)]
        public DeviceInfoKeyboard keyboard;
        [FieldOffset(8)]
        public DeviceInfoHID hid;
    }
}
using System.Runtime.InteropServices;

namespace RoboPhredDev.Shipbreaker.SixAxis.Native.RID
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DeviceInfoMouse
    {
        public int dwId;
        public int dwNumberOfButtons;
        public int dwSampleRate;

        [MarshalAs(UnmanagedType.Bool)]
        public bool fHasHorizontalWheel;
    }
}
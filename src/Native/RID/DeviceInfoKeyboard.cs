using System.Runtime.InteropServices;

namespace RoboPhredDev.Shipbreaker.SixAxis.Native.RID
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DeviceInfoKeyboard
    {
        public int dwType;
        public int dwSubType;
        public int dwKeyboardMode;
        public int dwNumberOfFunctionKeys;
        public int dwNumberOfIndicators;
        public int dwNumberOfKeysTotal;
    }
}
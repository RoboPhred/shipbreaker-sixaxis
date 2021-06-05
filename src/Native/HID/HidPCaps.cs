
using System.Runtime.InteropServices;

namespace RoboPhredDev.Shipbreaker.SixAxis.Native.HID
{

    [StructLayout(LayoutKind.Sequential)]
    struct HidPCaps
    {
        readonly ushort Usage;

        readonly ushort UsagePage;

        public ushort InputReportByteLength;

        public ushort OutputReportByteLength;

        public ushort FeatureReportByteLength;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
        readonly ushort[] Reserved;

        public ushort NumberLinkCollectionNodes;

        public ushort NumberInputButtonCaps;

        public ushort NumberInputValueCaps;

        public ushort NumberInputDataIndices;

        public ushort NumberOutputButtonCaps;

        public ushort NumberOutputValueCaps;

        public ushort NumberOutputDataIndices;

        public ushort NumberFeatureButtonCaps;

        public ushort NumberFeatureValueCaps;

        public ushort NumberFeatureDateIndices;
    }
}

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace RoboPhredDev.Shipbreaker.SixAxis.Native.RID
{
    static class RIDInterop
    {
        [DllImport("user32", SetLastError = true)]
        static extern uint GetRawInputDeviceList([Out] DeviceListItem[] pRawInputDeviceList, ref uint puiNumDevices, uint cbSize);


        [DllImport("user32", SetLastError = true)]
        static extern bool RegisterRawInputDevices(DeviceRegistration[] pRawInputDevices, uint uiNumDevices, uint cbSize);

        [DllImport("user32", SetLastError = true)]
        static extern uint GetRawInputData(IntPtr hRawInput, uint uiCommand, out RawInputDataHeader pData, ref uint pcbSize, uint cbSizeHeader);


        [DllImport("user32", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern uint GetRawInputDeviceInfo(IntPtr hDevice, uint uiBehavior, IntPtr ptr, out uint pcbSize);

        [DllImport("user32", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern uint GetRawInputDeviceInfo(IntPtr hDevice, uint uiBehavior, StringBuilder pData, in uint pcbSize);

        [DllImport("user32", SetLastError = true)]
        static extern uint GetRawInputDeviceInfoA(IntPtr hDevice, uint uiBehavior, out DeviceInfo pData, in uint pcbSize);

        public static DeviceListItem[] GetRawInputDeviceList()
        {
            var size = (uint)Marshal.SizeOf(typeof(DeviceListItem));

            uint deviceCount = 0;
            GetRawInputDeviceList(null, ref deviceCount, size);

            var devices = new DeviceListItem[deviceCount];
            var result = GetRawInputDeviceList(devices, ref deviceCount, size);

            if (result == unchecked((uint)-1))
            {
                throw Win32ErrorException.FromLastWin32Error();
            }

            return devices;
        }

        public static void RegisterDevices(DeviceRegistration[] devices)
        {
            RegisterRawInputDevices(devices, (uint)devices.Length, (uint)Marshal.SizeOf(typeof(DeviceRegistration)));
        }

        public static RawInputDataHeader GetRawInputHeader(IntPtr handle)
        {
            var headerSize = (uint)Marshal.SizeOf(typeof(RawInputDataHeader));
            var headerOutSize = headerSize;

            if (GetRawInputData(handle, (uint)InputDataCommand.RID_HEADER, out var header, ref headerOutSize, headerSize) == unchecked((uint)-1))
            {
                throw Win32ErrorException.FromLastWin32Error();
            }

            return header;
        }

        public static string GetRawInputDeviceName(IntPtr handle)
        {
            GetRawInputDeviceInfo(handle, (uint)DeviceInfoCommand.RIDI_DEVICENAME, IntPtr.Zero, out var size);
            if (size <= 2)
            {
                return null;
            }

            var sb = new StringBuilder((int)size);
            var result = GetRawInputDeviceInfo(handle, (uint)DeviceInfoCommand.RIDI_DEVICENAME, sb, in size);
            if (result == unchecked((uint)-1))
            {
                throw Win32ErrorException.FromLastWin32Error();
            }

            return sb.ToString();
        }

        public static DeviceInfo GetRawInputDeviceInfo(IntPtr handle)
        {
            var deviceInfoSize = (uint)Marshal.SizeOf(typeof(DeviceInfo));
            if (GetRawInputDeviceInfoA(handle, (uint)DeviceInfoCommand.RIDI_DEVICEINFO, out var info, deviceInfoSize) == unchecked((uint)-1))
            {
                throw Win32ErrorException.FromLastWin32Error();
            }

            return info;
        }
    }
}

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace RoboPhredDev.Shipbreaker.SixAxis.Native.RID
{
    static class RawInputInterop
    {
        [DllImport("user32", SetLastError = true)]
        static extern uint GetRawInputDeviceList([Out] DeviceListItem[] pRawInputDeviceList, ref uint puiNumDevices, uint cbSize);

        [DllImport("user32", SetLastError = true)]
        static extern bool RegisterRawInputDevices(DeviceRegistration[] pRawInputDevices, uint uiNumDevices, uint cbSize);

        [DllImport("user32", SetLastError = true)]
        static extern uint GetRawInputData(IntPtr hRawInput, uint uiCommand, IntPtr ptr, ref uint pcbSize, uint cbSizeHeader);
        [DllImport("user32", SetLastError = true)]
        static extern uint GetRawInputData(IntPtr hRawInput, uint uiCommand, out RawInputDataHeader pData, ref uint pcbSize, uint cbSizeHeader);

        [DllImport("user32", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern uint GetRawInputDeviceInfo(IntPtr hDevice, uint uiBehavior, IntPtr ptr, out uint pcbSize);

        [DllImport("user32", SetLastError = true)]
        static extern uint GetRawInputDeviceInfo(IntPtr hDevice, uint uiBehavior, [Out] byte[] pData, in uint pcbSize);

        [DllImport("user32", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern uint GetRawInputDeviceInfo(IntPtr hDevice, uint uiBehavior, StringBuilder pData, in uint pcbSize);

        [DllImport("user32", SetLastError = true)]
        static extern uint GetRawInputDeviceInfoA(IntPtr hDevice, uint uiBehavior, out DeviceInfo pData, in uint pcbSize);

        [DllImport("user32", SetLastError = true)]
        static extern uint GetRawInputData(IntPtr hRawInput, RawInputDataCommand uiBehavior, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);

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
            if (!RegisterRawInputDevices(devices, (uint)devices.Length, (uint)Marshal.SizeOf(typeof(DeviceRegistration))))
            {
                throw Win32ErrorException.FromLastWin32Error();
            }
        }

        public static RawInputDataHeader GetRawInputHeader(IntPtr rawInputHandle)
        {
            var headerSize = (uint)Marshal.SizeOf(typeof(RawInputDataHeader));
            var headerOutSize = headerSize;

            if (GetRawInputData(rawInputHandle, (uint)RawInputDataCommand.RID_HEADER, out var header, ref headerOutSize, headerSize) == unchecked((uint)-1))
            {
                throw Win32ErrorException.FromLastWin32Error();
            }

            return header;
        }

        public static string GetRawInputDeviceName(IntPtr deviceHandle)
        {
            GetRawInputDeviceInfo(deviceHandle, (uint)RawInputDeviceInfoCommand.RIDI_DEVICENAME, IntPtr.Zero, out var size);
            if (size <= 2)
            {
                return null;
            }

            var sb = new StringBuilder((int)size);
            var result = GetRawInputDeviceInfo(deviceHandle, (uint)RawInputDeviceInfoCommand.RIDI_DEVICENAME, sb, in size);
            if (result == unchecked((uint)-1))
            {
                throw Win32ErrorException.FromLastWin32Error();
            }

            return sb.ToString();
        }

        public static DeviceInfo GetRawInputDeviceInfo(IntPtr deviceHandle)
        {
            var deviceInfoSize = (uint)Marshal.SizeOf(typeof(DeviceInfo));
            if (GetRawInputDeviceInfoA(deviceHandle, (uint)RawInputDeviceInfoCommand.RIDI_DEVICEINFO, out var info, deviceInfoSize) == unchecked((uint)-1))
            {
                throw Win32ErrorException.FromLastWin32Error();
            }

            return info;
        }

        public static unsafe RawHid GetRawInputHidData(IntPtr rawInputHandle)
        {
            var headerSize = (uint)Marshal.SizeOf<RawInputDataHeader>();
            uint size = 0;

            GetRawInputData(rawInputHandle, (uint)RawInputDataCommand.RID_INPUT, IntPtr.Zero, ref size, headerSize);

            var bytes = new byte[size];

            fixed (byte* bytesPtr = bytes)
            {
                var result = GetRawInputData(rawInputHandle, (uint)RawInputDataCommand.RID_INPUT, (IntPtr)bytesPtr, ref size, headerSize);
                if (result == unchecked((uint)-1))
                {
                    throw Win32ErrorException.FromLastWin32Error();
                }

                var header = *(RawInputDataHeader*)bytesPtr;

                return RawHid.FromPointer(bytesPtr + headerSize);
            }
        }

        public static byte[] GetRawInputDevicePreparsedData(IntPtr deviceHandle)
        {
            GetRawInputDeviceInfo(deviceHandle, (uint)RawInputDeviceInfoCommand.RIDI_PREPARSEDDATA, IntPtr.Zero, out var size);

            if (size == 0) return null;

            var preparsedData = new byte[size];
            var result = GetRawInputDeviceInfo(deviceHandle, (uint)RawInputDeviceInfoCommand.RIDI_PREPARSEDDATA, preparsedData, in size);
            if (result == unchecked((uint)-1))
            {
                throw Win32ErrorException.FromLastWin32Error();
            }

            return preparsedData;
        }
    }
}
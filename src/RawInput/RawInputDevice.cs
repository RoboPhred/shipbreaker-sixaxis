
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using RoboPhredDev.Shipbreaker.SixAxis.Native.RID;

namespace RoboPhredDev.Shipbreaker.SixAxis.RawInput
{
    abstract class RawInputDevice : IDeviceIdentifier
    {
        public IntPtr Handle { get; }
        public string DeviceName { get; }

        public int VendorId
        {
            get
            {
                if (!DeviceName.Contains("VID_"))
                {
                    return 0;
                }

                var vidStr = DeviceName.Substring(DeviceName.IndexOf("VID_", StringComparison.Ordinal) + 4, 4);
                return int.Parse(vidStr, NumberStyles.HexNumber);
            }
        }
        public int ProductId
        {
            get
            {
                if (!DeviceName.Contains("PID_"))
                {
                    return 0;
                }

                var pidStr = DeviceName.Substring(DeviceName.IndexOf("PID_", StringComparison.Ordinal) + 4, 4);
                return int.Parse(pidStr, NumberStyles.HexNumber);
            }
        }

        public abstract ushort UsagePage { get; }
        public abstract ushort Usage { get; }

        protected RawInputDevice(IntPtr handle)
        {
            this.Handle = handle;
            this.DeviceName = RawInputInterop.GetRawInputDeviceName(handle);
        }

        public static RawInputDevice FromHandle(IntPtr handle)
        {
            var deviceInfo = RawInputInterop.GetRawInputDeviceInfo(handle);

            switch (deviceInfo.dwType)
            {
                case DeviceType.RIM_TYPEMOUSE:
                    return new RawInputMouseDevice(handle);
                case DeviceType.RIM_TYPEKEYBOARD:
                    return new RawInputKeyboardDevice(handle);
                case DeviceType.RIM_TYPEHID:
                    return new RawInputHidDevice(handle);
            }

            throw new ArgumentException($"Device type {deviceInfo.dwType} is not supported.");
        }

        public static IEnumerable<RawInputDevice> GetDevices()
        {
            var devices = RawInputInterop.GetRawInputDeviceList();

            return devices.Select(device => FromHandle(device.hDevice));
        }
    }
}
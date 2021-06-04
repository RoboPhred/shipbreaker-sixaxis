
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using RoboPhredDev.Shipbreaker.SixAxis.Native.RID;

namespace RoboPhredDev.Shipbreaker.SixAxis.Input
{
    abstract class RawInputDevice
    {
        public IntPtr Handle { get; }
        public string DeviceName { get; }

        public virtual int VendorId
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
        public virtual int ProductId
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

        protected RawInputDevice(IntPtr handle)
        {
            this.Handle = handle;
            this.DeviceName = RIDInterop.GetRawInputDeviceName(handle);
        }

        static RawInputDevice FromHandle(IntPtr handle)
        {
            var deviceInfo = RIDInterop.GetRawInputDeviceInfo(handle);

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
            var devices = RIDInterop.GetRawInputDeviceList();

            return devices.Select(device => FromHandle(device.hDevice));
        }
    }
}
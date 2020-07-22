using System;
using System.Linq;
using System.Runtime.InteropServices;
using Linearstar.Windows.RawInput;

namespace RoboPhredDev.Shipbreaker.SixAxis
{

    enum WindowsMessage
    {
        WM_INPUT = 0x00FF
    }

    // https://www.freebsddiary.org/APC/usb_hid_usages.php
    enum UsagePages : ushort
    {
        GenericDesktop = 0x01
    }

    enum GenericDesktopUsage : ushort
    {
        MultiAxisController = 0x08,
        X = 0x30,
        Y = 0x31,
        Z = 0x32,
        Rx = 0x33,
        Ry = 0x34,
        Rz = 0x35
    }

    static class WmInputInterceptor
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", EntryPoint = "SetWindowLongW")]
        private static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);


        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowLongPtr(HandleRef hWnd, int nIndex, IntPtr dwNewLong);

        public delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msgAsInt, IntPtr wParam, IntPtr lParam);

        private static IntPtr windowHandle;
        private static WndProcDelegate originalHandler;

        private static DateTime lastLog = DateTime.UtcNow;

        public static void Enable()
        {
            windowHandle = GetActiveWindow();
            originalHandler = SetWindowHandle(CustomWndProc);

            // Generic Device, Axis Controller
            RawInputDevice.RegisterDevice(new HidUsageAndPage((ushort)UsagePages.GenericDesktop, (ushort)GenericDesktopUsage.MultiAxisController), RawInputDeviceFlags.None, windowHandle);
        }

        public static void Disable()
        {
            SetWindowHandle(originalHandler);
            windowHandle = IntPtr.Zero;
        }

        private static WndProcDelegate SetWindowHandle(WndProcDelegate handle)
        {
            var handlePtr = Marshal.GetFunctionPointerForDelegate(handle);
            IntPtr oldHandler;
            if (IntPtr.Size == 4)
                oldHandler = SetWindowLongPtr32(windowHandle, -4, handlePtr);
            else
                oldHandler = SetWindowLongPtr64(windowHandle, -4, handlePtr);
            return (WndProcDelegate)Marshal.GetDelegateForFunctionPointer(oldHandler, typeof(WndProcDelegate));
        }

        private static IntPtr CustomWndProc(IntPtr hWnd, uint msgAsInt, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                WindowsMessage msg = (WindowsMessage)msgAsInt;
                switch (msg)
                {
                    case WindowsMessage.WM_INPUT:
                        {
                            if (OnWmInput(wParam, lParam))
                            {
                                return (IntPtr)0;
                            }
                            break;
                        }
                    default:
                        break;
                }
                return originalHandler(hWnd, msgAsInt, wParam, lParam);
            }
            catch (Exception ex)
            {
                Logging.Log("Error handling message: {0}\n{1}\n{2}", ex.GetType().Name, ex.Message, ex.StackTrace);
                return IntPtr.Zero;
            }
        }

        private static bool OnWmInput(IntPtr wParam, IntPtr lParam)
        {
            var data = RawInputData.FromHandle(lParam);
            if (data == null)
            {
                return false;
            }
            if (data.Device.UsageAndPage == HidUsageAndPage.Mouse)
            {
                return false;
            }

            if (lastLog > DateTime.UtcNow)
            {
                return true;
            }
            lastLog = DateTime.UtcNow + TimeSpan.FromSeconds(.25);

            Logging.Log("Input from {0}", data.Device.UsageAndPage);
            if (data.Device.UsageAndPage.UsagePage != (ushort)UsagePages.GenericDesktop || data.Device.UsageAndPage.Usage != (ushort)GenericDesktopUsage.MultiAxisController)
            {
                return true;
            }

            var valueData = data as RawInputHidData;
            if (valueData != null)
            {
                Logging.Log("{0}", valueData.ToString());
                Logging.Log("{0} states", valueData.ValueSetStates.Length);

                // 6 states, 1 each for x/y/z, dont know what the others are doing
                var allValues = valueData.ValueSetStates.SelectMany(x => x).ToArray();
                Logging.Log("{0} values", allValues.Length);

                Logging.Log("{0}", string.Join(", ", allValues.Select(x => x.Value.UsageAndPage)));

                var xState = allValues.FirstOrDefault(x => x.Value.UsageAndPage.Usage == (ushort)GenericDesktopUsage.X);
                var yState = allValues.FirstOrDefault(x => x.Value.UsageAndPage.Usage == (ushort)GenericDesktopUsage.Y);
                var zState = allValues.FirstOrDefault(x => x.Value.UsageAndPage.Usage == (ushort)GenericDesktopUsage.Z);
                var rxState = allValues.FirstOrDefault(x => x.Value.UsageAndPage.Usage == (ushort)GenericDesktopUsage.Rx);
                var ryState = allValues.FirstOrDefault(x => x.Value.UsageAndPage.Usage == (ushort)GenericDesktopUsage.Ry);
                var rzState = allValues.FirstOrDefault(x => x.Value.UsageAndPage.Usage == (ushort)GenericDesktopUsage.Rz);

                Logging.Log("Found | Translation {0},{1},{2} | Rotation {3}, {4}, {5}", xState != null, yState != null, zState != null, rxState != null, ryState != null, rzState != null);
                Logging.Log("Scaled | Translation {0},{1},{2} | Rotation {3}, {4}, {5}", xState?.ScaledValue, yState?.ScaledValue, zState?.ScaledValue, rxState?.ScaledValue, ryState?.ScaledValue, rzState?.ScaledValue);
                Logging.Log("Curent | Translation {0},{1},{2} | Rotation {3}, {4}, {5}", valueToString(xState), valueToString(yState), valueToString(zState), valueToString(rxState), valueToString(ryState), valueToString(rzState));
            }
            return true;
        }

        private static string valueToString(HidValueState state)
        {
            if (state == null)
            {
                return "null";
            }
            try
            {
                return state.CurrentValue.ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}

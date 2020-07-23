using System;
using System.Linq;
using Linearstar.Windows.RawInput;
using Linearstar.Windows.RawInput.Native;
using RoboPhredDev.Shipbreaker.SixAxis.Input;

namespace RoboPhredDev.Shipbreaker.SixAxis
{

    static class InputHandler
    {
        public static void Initialize()
        {
            Logging.Log("Initializing InputHandler");

            WmInputInterceptor.OnInput += HandleInput;

            var windowHandle = WmInputInterceptor.WindowHandle;

            // Generic Device, Axis Controller
            RawInputDevice.RegisterDevice(new HidUsageAndPage((ushort)UsagePage.GenericDesktop, (ushort)GenericDesktopUsage.MultiAxisController), RawInputDeviceFlags.None, windowHandle);
        }

        private static void HandleInput(object sender, WmInputEventArgs e)
        {
            var data = e.Data;

            // Game already registers and uses Mouse, so pass that back.
            if (data.Device.UsageAndPage == HidUsageAndPage.Mouse)
            {
                return;
            }

            e.Handled = true;

            // Check to see if its from a multi axis controller.
            if (data.Device.UsageAndPage.UsagePage != (ushort)UsagePage.GenericDesktop || data.Device.UsageAndPage.Usage != (ushort)GenericDesktopUsage.MultiAxisController)
            {
                return;
            }


            // TODO: Lots of these properties mask user32 lookups to decode data that we should cache.
            //  Remove the RawInput.Sharp library and implement everything ourselves so we can cache the device data. 

            var valueData = data as RawInputHidData;
            if (valueData != null)
            {
                // RawInput.Sharp reports all values even though they may not apply to this particular report.
                // We need to check for failure to read the values.

                // This is remarkably inefficient, lots of hid.dll and user32.dll calls doing what we could be caching
                //  or doing ourselves.  Should replace RawInput.Sharp with directly using user32 apis.

                var allValues = valueData.ValueSetStates.SelectMany(x => x).ToArray();

                var x = TryGetScaledValue(allValues, (ushort)GenericDesktopUsage.X);
                if (x.HasValue)
                {
                    InputAggregator.Instance.X = x.Value;
                }

                var y = TryGetScaledValue(allValues, (ushort)GenericDesktopUsage.Y);
                if (y.HasValue)
                {
                    InputAggregator.Instance.Y = y.Value;
                }

                var z = TryGetScaledValue(allValues, (ushort)GenericDesktopUsage.Z);
                if (z.HasValue)
                {
                    InputAggregator.Instance.Z = z.Value;
                }

                var rx = TryGetScaledValue(allValues, (ushort)GenericDesktopUsage.Rx);
                if (rx.HasValue)
                {
                    InputAggregator.Instance.RX = rx.Value;
                }

                var ry = TryGetScaledValue(allValues, (ushort)GenericDesktopUsage.Ry);
                if (ry.HasValue)
                {
                    InputAggregator.Instance.RY = ry.Value;
                }

                var rz = TryGetScaledValue(allValues, (ushort)GenericDesktopUsage.Rz);
                if (rz.HasValue)
                {
                    InputAggregator.Instance.RZ = rz.Value;
                }
            }
        }

        private static int? TryGetScaledValue(HidValueState[] allValues, ushort usage)
        {
            var state = allValues.FirstOrDefault(x => x.Value.UsageAndPage.Usage == usage);
            if (state == null)
            {
                return null;
            }
            try
            {
                return state.ScaledValue;
            }
            catch (Win32ErrorException)
            {
                return null;
            }
        }
    }
}
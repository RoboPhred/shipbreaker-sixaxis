using System;
using System.Linq;
using Linearstar.Windows.RawInput;
using Linearstar.Windows.RawInput.Native;
using RoboPhredDev.Shipbreaker.SixAxis.Input;

namespace RoboPhredDev.Shipbreaker.SixAxis
{

    static class InputHandler
    {
        public static float X { get; set; }
        public static float Y { get; set; }
        public static float Z { get; set; }

        public static float RX { get; set; }
        public static float RY { get; set; }
        public static float RZ { get; set; }

        public static void Initialize()
        {
            Logging.Log("Initializing InputHandler");

            WmInputInterceptor.OnInput += HandleInput;

            var windowHandle = WmInputInterceptor.WindowHandle;
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

                var x = TryGetNormalizedValue(allValues, (ushort)GenericDesktopUsage.X);
                if (x.HasValue)
                {
                    X = x.Value;
                }

                var y = TryGetNormalizedValue(allValues, (ushort)GenericDesktopUsage.Y);
                if (y.HasValue)
                {
                    Y = y.Value;
                }

                var z = TryGetNormalizedValue(allValues, (ushort)GenericDesktopUsage.Z);
                if (z.HasValue)
                {
                    Z = z.Value;
                }

                var rx = TryGetNormalizedValue(allValues, (ushort)GenericDesktopUsage.Rx);
                if (rx.HasValue)
                {
                    RX = rx.Value;
                }

                var ry = TryGetNormalizedValue(allValues, (ushort)GenericDesktopUsage.Ry);
                if (ry.HasValue)
                {
                    RY = ry.Value;
                }

                var rz = TryGetNormalizedValue(allValues, (ushort)GenericDesktopUsage.Rz);
                if (rz.HasValue)
                {
                    RZ = rz.Value;
                }
            }
        }

        private static float? TryGetNormalizedValue(HidValueState[] allValues, ushort usage)
        {
            var state = allValues.FirstOrDefault(x => x.Value.UsageAndPage.Usage == usage);
            if (state == null)
            {
                return null;
            }
            try
            {
                if (!state.ScaledValue.HasValue)
                {
                    return null;
                }
                var value = state.ScaledValue.Value;
                var min = (float)state.Value.MinPhysicalValue;
                var max = (float)state.Value.MaxPhysicalValue;

                // Cant find any documentation on what a 'scaled value' actually is.
                // Mimicing the chromium gamepad code found at 
                //  https://chromium.googlesource.com/chromium/src/+/refs/tags/70.0.3524.1/device/gamepad/raw_input_gamepad_device_win.cc
                // Seems to imply the value is relative to the physical value of the joystick.
                // Best guess is the scaled value is calculating the logical value back to physical value units.

                // The following algorithm is what chromium uses to normalize its gamepad axis.
                //  Seems to be getting value as a percentage of the total range of the physical value,
                //  then reinterpreting it so that min is -1 and max is 1.
                return (2.0f * (value - min) / (max - min)) - 1.0f;
            }
            catch (Win32ErrorException)
            {
                return null;
            }
        }
    }
}
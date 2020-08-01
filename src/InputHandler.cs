using System;
using System.Collections.Generic;
using System.Linq;
using Linearstar.Windows.RawInput;
using Linearstar.Windows.RawInput.Native;
using RoboPhredDev.Shipbreaker.SixAxis.Config;
using UnityEngine;

namespace RoboPhredDev.Shipbreaker.SixAxis
{
    class InputIdentifier : IEquatable<InputIdentifier>
    {
        public int VendorId { get; set; }
        public int ProductId { get; set; }
        public ShipbreakerAxisType Axis { get; set; }

        public InputIdentifier(int vendorId, int productId, ShipbreakerAxisType axis)
        {
            this.VendorId = vendorId;
            this.ProductId = productId;
            this.Axis = axis;
        }

        public bool Equals(InputIdentifier other)
        {
            return this.VendorId == other.VendorId && this.ProductId == other.ProductId && this.Axis == other.Axis;
        }

        public override int GetHashCode()
        {
            return this.VendorId ^ this.ProductId ^ this.Axis.GetHashCode();
        }
    }

    static class InputHandler
    {
        private static List<InputMapping> sInputMappings;

        private static Dictionary<InputIdentifier, float> sInputs = new Dictionary<InputIdentifier, float>();

        public static Vector3 Translation
        {
            get
            {
                var xInputs = (from input in sInputs
                               where input.Key.Axis == ShipbreakerAxisType.X
                               select input.Value).ToArray();
                var yInputs = (from input in sInputs
                               where input.Key.Axis == ShipbreakerAxisType.Y
                               select input.Value).ToArray();
                var zInputs = (from input in sInputs
                               where input.Key.Axis == ShipbreakerAxisType.Z
                               select input.Value).ToArray();
                return new Vector3(
                    Mathf.Clamp(xInputs.Sum(), -1, 1),
                    Mathf.Clamp(yInputs.Sum(), -1, 1),
                    Mathf.Clamp(zInputs.Sum(), -1, 1)
                );
            }
        }

        public static Vector3 Rotation
        {
            get
            {
                var xInputs = (from input in sInputs
                               where input.Key.Axis == ShipbreakerAxisType.Rx
                               select input.Value).ToArray();
                var yInputs = (from input in sInputs
                               where input.Key.Axis == ShipbreakerAxisType.Ry
                               select input.Value).ToArray();
                var zInputs = (from input in sInputs
                               where input.Key.Axis == ShipbreakerAxisType.Rz
                               select input.Value).ToArray();
                return new Vector3(
                    Mathf.Clamp(xInputs.Sum(), -1, 1),
                    Mathf.Clamp(yInputs.Sum(), -1, 1),
                    Mathf.Clamp(zInputs.Sum(), -1, 1)
                );
            }
        }

        public static void Initialize(List<InputMapping> inputMappings)
        {
            sInputMappings = inputMappings;
            WindowMessageInterceptor.OnMessage += HandleMessage;
        }

        private static void HandleMessage(object sender, WindowMessageEventArgs e)
        {
            // Handle WM_INPUT
            if (e.Message != 0x00FF)
            {
                return;
            }

            try
            {
                var data = RawInputData.FromHandle(e.LParam);

                // Game already registers and uses Mouse, so pass that back.
                if (data.Device.UsageAndPage == HidUsageAndPage.Mouse)
                {
                    return;
                }

                // Mark the message as handled and return 0 for consumed.
                e.Result = IntPtr.Zero;

                ProcessInput(data);
            }
            catch (Exception ex)
            {
                Logging.Log($"Failed to process window message: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private static void ProcessInput(RawInputData data)
        {
            var hidData = data as RawInputHidData;
            if (hidData == null)
            {
                return;
            }

            foreach (var mapping in GetMappingsForDevice(hidData.Device))
            {
                ProcessInputMapping(hidData, mapping);
            }
        }

        private static IEnumerable<InputMapping> GetMappingsForDevice(RawInputDevice device)
        {
            return from mapping in sInputMappings
                   where mapping.ContainsDevice(device.VendorId, device.ProductId)
                   select mapping;
        }

        private static void ProcessInputMapping(RawInputHidData data, InputMapping mapping)
        {
            var allValues = data.ValueSetStates.SelectMany(x => x).ToArray();

            // TODO: This relies on a complex process of encountering errors, encoding those errors into exceptions, and throwing exceptions
            //  On the whole, this is probably very slow.
            // We can speed up the whole thing by forgoing RawInput.Sharp and implementing it ourselves, caching device data and checking
            //  what reports each axis is under.

            foreach (var axis in mapping.Axes)
            {
                var maybeValue = TryGetNormalizedValue(allValues, axis.AxisUsage);
                if (!maybeValue.HasValue)
                {
                    continue;
                }

                var value = maybeValue.Value;
                if (axis.Invert)
                {
                    value = -value;
                }

                var inputId = new InputIdentifier(data.Device.VendorId, data.Device.ProductId, axis.GameAxis);
                sInputs[inputId] = value;
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
                // This is really stupid, but RawInput.Sharp does not check what report we have or if the value is included in the report.
                //  Generating the exception just to ignore it takes a lot of time, we should implement the check to avoid it.
                return null;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using RoboPhredDev.Shipbreaker.SixAxis.Config;
using RoboPhredDev.Shipbreaker.SixAxis.Input;
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
                               select input.Value);
                var yInputs = (from input in sInputs
                               where input.Key.Axis == ShipbreakerAxisType.Y
                               select input.Value);
                var zInputs = (from input in sInputs
                               where input.Key.Axis == ShipbreakerAxisType.Z
                               select input.Value);
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
                               select input.Value);
                var yInputs = (from input in sInputs
                               where input.Key.Axis == ShipbreakerAxisType.Ry
                               select input.Value);
                var zInputs = (from input in sInputs
                               where input.Key.Axis == ShipbreakerAxisType.Rz
                               select input.Value);
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

                if (data.Device.UsagePage == (uint)UsagePage.GenericDesktop && data.Device.Usage == (uint)GenericDesktopUsage.Mouse)
                {
                    // Game already registers and uses Mouse, so pass that back.
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
            if (!(data is RawInputHidData hidData))
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
            foreach (var axis in mapping.Axes)
            {
                var value = data.GetInputValue(axis.AxisUsage);
                if (value == null)
                {
                    continue;
                }

                var inputId = new InputIdentifier(data.Device.VendorId, data.Device.ProductId, axis.GameAxis);

                var normalizedValue = value.NormalizedValue;

                if (axis.Invert)
                {
                    normalizedValue = -normalizedValue;
                }

                sInputs[inputId] = normalizedValue;

                SixAxisPlugin.Instance.ReceivedControllerInput();
            }
        }
    }
}
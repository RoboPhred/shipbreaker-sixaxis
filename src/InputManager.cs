
using System;
using System.Collections.Generic;
using System.Linq;
using RoboPhredDev.Shipbreaker.SixAxis.RawInput;
using UnityEngine;

namespace RoboPhredDev.Shipbreaker.SixAxis
{
    static class InputManager
    {
        private static readonly Dictionary<string, InputDevice> devices = new();

        private static readonly Dictionary<IDeviceSpecification, IInputMap> inputMaps = new();

        public static Vector3 Translation
        {
            get
            {
                var xInputs = CollectAxisValues(ShipbreakerAxisType.X).ToArray();
                var yInputs = CollectAxisValues(ShipbreakerAxisType.Y).ToArray();
                var zInputs = CollectAxisValues(ShipbreakerAxisType.Z).ToArray();
                return new Vector3(
                    CombineAxisValues(xInputs),
                    CombineAxisValues(yInputs),
                    CombineAxisValues(zInputs)
                );
            }
        }

        public static Vector3 Rotation
        {
            get
            {
                var xInputs = CollectAxisValues(ShipbreakerAxisType.Rx);
                var yInputs = CollectAxisValues(ShipbreakerAxisType.Ry);
                var zInputs = CollectAxisValues(ShipbreakerAxisType.Rz);
                return new Vector3(
                    CombineAxisValues(xInputs),
                    CombineAxisValues(yInputs),
                    CombineAxisValues(zInputs)
                );
            }
        }

        public static void RegisterInputMap(IDeviceSpecification specification, IInputMap inputMap)
        {
            inputMaps.Add(specification, inputMap);
        }

        public static void HandleInput(RawInputHidData data)
        {
            Debug.Log("Got input from device" + data.Device.DeviceName);
            try
            {
                var controller = GetInputDevice(data.Device);

                var frame = controller.HandleInput(data);
                ProcessButtons(controller, frame);
            }
            catch (Exception ex)
            {
                Logging.Log($"Error handling input: {ex.Message}\n\n{ex.StackTrace}");
            }
        }

        private static InputDevice GetInputDevice(RawInputDevice device)
        {
            var deviceName = device.DeviceName;

            if (!devices.TryGetValue(deviceName, out var controller))
            {
                controller = new InputDevice(deviceName, device.VendorId, device.ProductId, device.UsagePage);
                var newMapping = inputMaps.FirstOrDefault(x => x.Key.SpecificationMatches(device));
                devices.Add(deviceName, controller);
                Logging.Log(new Dictionary<string, string>
                    {
                        {"VendorId", device.VendorId.ToString("X")},
                        {"ProductId", device.ProductId.ToString("X")},
                        {"DeviceName", deviceName},
                        {"ConfigFile", (newMapping.Key != null) ? newMapping.Value.FileName : null},
                    }, $"New device discovered.   Device {device.DeviceName} " + ((newMapping.Key != null) ? $"has input mapping {newMapping.Value.FileName}" : "has no input mapping."));
            }

            return controller;
        }

        private static void ProcessButtons(InputDevice device, InputDeviceFrame frame)
        {
            foreach (var mapping in inputMaps.Where(x => x.Key.SpecificationMatches(device)))
            {
                foreach (var pressed in frame.ButtonsPressed)
                {
                    foreach (var m in mapping.Value.Buttons.Where(x => x.Usage == pressed.Usage))
                    {
                        m.Command.Press();
                    }
                }

                foreach (var released in frame.ButtonsReleased)
                {
                    foreach (var m in mapping.Value.Buttons.Where(x => x.Usage == released.Usage))
                    {
                        m.Command.Release();
                    }
                }
            }
        }

        private static IEnumerable<float> CollectAxisValues(ShipbreakerAxisType axisType)
        {
            return from map in inputMaps
                   from axisMapping in map.Value.Axes
                   where axisMapping.GameAxis == axisType
                   from device in from d in devices.Values where map.Key.SpecificationMatches(d) select d
                   let axisValue = device.GetAxisValue(new PageAndUsage(device.UsagePage, axisMapping.Usage))
                   where axisValue.HasValue
                   select axisValue.Value * axisMapping.Scale;
        }

        private static float CombineAxisValues(IEnumerable<float> values)
        {
            // Make sure at least one value is in the collection, otherwise max and min throw an error.
            var positives = values.Where(value => value > 0).Append(0).Max();
            var negatives = values.Where(value => value < 0).Append(0).Min();
            return positives + negatives;
        }
    }
}
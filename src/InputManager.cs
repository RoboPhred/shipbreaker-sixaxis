
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
            var deviceName = data.Device.DeviceName;

            if (!devices.TryGetValue(deviceName, out var controller))
            {
                controller = new InputDevice(data.Device.DeviceName, data.Device.VendorId, data.Device.ProductId, data.Device.UsagePage);
                var mapping = inputMaps.FirstOrDefault(x => x.Key.SpecificationMatches(data.Device));
                devices.Add(deviceName, controller);
                Logging.Log(new Dictionary<string, string>
                    {
                        {"VendorId", data.Device.VendorId.ToString("X")},
                        {"ProductId", data.Device.ProductId.ToString("X")},
                        {"DeviceName", deviceName},
                        {"ConfigFile", (mapping.Key != null) ? mapping.Value.FileName : null},
                    }, $"New device discovered.   Device {data.Device.DeviceName} " + ((mapping.Key != null) ? $"has input mapping {mapping.Value.FileName}" : "has no input mapping."));
            }

            controller.HandleInput(data);
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
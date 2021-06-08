
using System.Collections.Generic;
using RoboPhredDev.Shipbreaker.SixAxis.RawInput;

namespace RoboPhredDev.Shipbreaker.SixAxis
{
    static class ControllerManager
    {
        private static readonly Dictionary<string, Controller> controllers = new();

        public static Controller GetController(string deviceName)
        {
            if (controllers.TryGetValue(deviceName, out var controller))
            {
                return controller;
            }
            return null;
        }

        public static void HandleInput(RawInputHidData data)
        {
            var deviceName = data.Device.DeviceName;

            if (!controllers.TryGetValue(deviceName, out var controller))
            {
                controller = new Controller(deviceName);
                controllers.Add(deviceName, controller);
            }

            controller.HandleInput(data);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using RoboPhredDev.Shipbreaker.SixAxis.RawInput;

namespace RoboPhredDev.Shipbreaker.SixAxis
{
    class InputDevice : IDeviceIdentifier
    {
        public string DeviceName { get; }
        public int VendorId { get; }
        public int ProductId { get; }

        public ushort UsagePage { get; }

        private readonly Dictionary<PageAndUsage, float> normalizedAxes = new();
        private readonly HashSet<PageAndUsage> activeButtons = new();

        public InputDevice(string deviceName, int vendorId, int productId, ushort usagePage)
        {
            DeviceName = deviceName;
            VendorId = vendorId;
            ProductId = productId;
            UsagePage = usagePage;
        }

        public float? GetAxisValue(PageAndUsage axisIdentifier)
        {
            if (normalizedAxes.TryGetValue(axisIdentifier, out var value))
            {
                return value;
            }
            return null;
        }

        public PageAndUsage[] GetButtonsPressed()
        {
            return activeButtons.ToArray();
        }

        public bool GetButtonPressed(PageAndUsage buttonIdentifier)
        {
            return activeButtons.Contains(buttonIdentifier);
        }

        public void HandleInput(RawInputHidData data)
        {
            foreach (var pair in data.GetAllValues())
            {
                var normalized = pair.Value.NormalizedValue;
                normalizedAxes[pair.Key] = normalized;
            }

            // We only get a button entry if we received a report containing that button.
            // Buttons not in the reports are presumed to mantain their previously seen value.
            foreach (var pair in data.GetAllButtons())
            {
                if (pair.Value)
                {
                    if (activeButtons.Add(pair.Key))
                    {
                        Logging.Log($"Button {pair.Key} pressed.");
                    }
                }
                else
                {
                    if (activeButtons.Remove(pair.Key))
                    {
                        Logging.Log($"Button {pair.Key} released.");
                    }
                }
            }
        }
    }
}
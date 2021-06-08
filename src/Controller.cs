
using System.Collections.Generic;
using RoboPhredDev.Shipbreaker.SixAxis.RawInput;

namespace RoboPhredDev.Shipbreaker.SixAxis
{
    class Controller
    {
        public string DeviceName { get; }

        private readonly Dictionary<UsageAndPage, float> normalizedAxes = new();
        private readonly HashSet<UsageAndPage> activeButtons = new();

        public Controller(string deviceName)
        {
            DeviceName = deviceName;
        }

        public float? GetAxisValue(UsageAndPage axisIdentifier)
        {
            if (normalizedAxes.TryGetValue(axisIdentifier, out var value))
            {
                return value;
            }
            return null;
        }

        public bool GetButtonPressed(UsageAndPage buttonIdentifier)
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
                        Logging.Log("Button pressed: {0}", pair.Key.ToString());
                    }
                }
                else
                {
                    if (activeButtons.Remove(pair.Key))
                    {
                        Logging.Log("Button Released: {0}", pair.Key.ToString());
                    }
                }
            }
        }
    }
}
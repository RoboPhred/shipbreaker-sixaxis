using System.Collections.Generic;
using RoboPhredDev.Shipbreaker.SixAxis.RawInput;

namespace RoboPhredDev.Shipbreaker.SixAxis
{
    class InputDeviceFrame
    {
        public InputDeviceFrame(HashSet<PageAndUsage> buttonsPressed, HashSet<PageAndUsage> buttonsReleased)
        {
            ButtonsPressed = buttonsPressed;
            ButtonsReleased = buttonsReleased;
        }

        public HashSet<PageAndUsage> ButtonsPressed { get; }
        public HashSet<PageAndUsage> ButtonsReleased { get; }
    }
}
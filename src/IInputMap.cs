
using System.Collections.Generic;

namespace RoboPhredDev.Shipbreaker.SixAxis
{
    interface IInputMap
    {
        string FileName { get; }
        IReadOnlyCollection<IAxisMapping> Axes { get; }
        IReadOnlyCollection<IButtonMapping> Buttons { get; }
    }
}
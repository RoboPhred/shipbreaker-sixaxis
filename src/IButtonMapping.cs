
using RoboPhredDev.Shipbreaker.SixAxis.ButtonCommands;

namespace RoboPhredDev.Shipbreaker.SixAxis
{
    interface IButtonMapping
    {
        ushort Usage { get; }

        string Device { get; }

        IButtonCommand Command { get; }
    }
}

using System;
using YamlDotNet.Serialization;

namespace RoboPhredDev.Shipbreaker.SixAxis.Config
{
    class ConfiguredAxisMapping : IAxisMapping
    {
        // TODO: Might have multiple axes with the same usage.
        //  Seems to be seperated by Link / Collection. Is there a raw ID we can get?
        //  Other HID tools seem to get an axis index.  They might be using the index of the axis in its report

        [YamlMember(Alias = "device")]
        public string Device { get; set; }

        [YamlMember(Alias = "axisUsage")]
        public ushort AxisUsage { get; set; }

        [YamlMember(Alias = "invert")]
        public bool Invert { get; set; }

        [YamlMember(Alias = "gameAxis")]
        public ShipbreakerAxisType GameAxis { get; set; }

        [YamlMember(Alias = "scale")]
        public float Scale { get; set; } = 1.0f;

        [YamlMember(Alias = "deadZone")]
        public float DeadZone { get; set; }

        public ushort Usage => this.AxisUsage;

        public float GetValue(float normalizedValue)
        {
            if (Math.Abs(normalizedValue) <= this.DeadZone)
            {
                return 0.0f;
            }

            return Math.Min(1, Math.Max(-1, normalizedValue * this.Scale * (this.Invert ? -1.0f : 1.0f)));
        }
    }
}
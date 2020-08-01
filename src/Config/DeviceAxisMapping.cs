using YamlDotNet.Serialization;

namespace RoboPhredDev.Shipbreaker.SixAxis.Config
{
    class DeviceAxisMapping
    {
        // TODO: Might have multiple axes with the same usage.
        //  Seems to be seperated by Link / Collection. Is there a raw ID we can get?
        //  Other HID tools seem to get an axis index.  They might be using the index of the axis in its report

        [YamlMember(Alias = "axisUsage")]
        public ushort AxisUsage { get; set; }

        [YamlMember(Alias = "invert")]
        public bool Invert { get; set; }

        [YamlMember(Alias = "gameAxis")]
        public ShipbreakerAxisType GameAxis { get; set; }
    }
}
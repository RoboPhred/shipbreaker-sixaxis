
using RoboPhredDev.Shipbreaker.SixAxis.Native.HID;

namespace RoboPhredDev.Shipbreaker.SixAxis.RawInput
{
    class RawInputHidValue
    {
        private HidPValueCaps valueCaps;
        private readonly int value;
        private readonly bool isRaw;

        public RawInputHidValue(HidPValueCaps valueCaps, ushort usage, int value, bool isRaw)
        {
            this.valueCaps = valueCaps;
            Usage = usage;
            this.value = value;
            this.isRaw = isRaw;

            if (usage == 0x30)
            {
                Logging.Log($"usage {usage} isAbsolute {valueCaps.IsAbsolute} {this}");
            }
        }

        public ushort Usage { get; }
        public ushort Page => valueCaps.UsagePage;

        public float NormalizedValue
        {
            get
            {
                var min = isRaw ? (float)valueCaps.LogicalMin : (float)valueCaps.PhysicalMin;
                var max = isRaw ? (float)valueCaps.LogicalMax : (float)valueCaps.PhysicalMax;

                return (2.0f * (this.value - min) / (max - min)) - 1.0f;
            }
        }

        public override string ToString()
        {
            return $"{value} (raw: {isRaw}) ({NormalizedValue}, {valueCaps.LogicalMin}-{valueCaps.LogicalMax}, {valueCaps.PhysicalMin}-{valueCaps.PhysicalMax})";
        }
    }
}
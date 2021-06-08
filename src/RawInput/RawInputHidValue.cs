
using RoboPhredDev.Shipbreaker.SixAxis.Native.HID;

namespace RoboPhredDev.Shipbreaker.SixAxis.RawInput
{
    class RawInputHidValue
    {
        private HidPValueCaps valueCaps;

        public RawInputHidValue(ushort usage, ushort page, int scaledValue, HidPValueCaps valueCaps)
        {
            this.valueCaps = valueCaps;

            Usage = usage;
            Page = page;
            ScaledValue = scaledValue;
        }

        public ushort Usage { get; }
        public ushort Page { get; }
        public int ScaledValue { get; }

        public float NormalizedValue
        {
            get
            {
                var min = (float)valueCaps.PhysicalMin;
                var max = (float)valueCaps.PhysicalMax;

                return (2.0f * (ScaledValue - min) / (max - min)) - 1.0f;
            }
        }
    }
}
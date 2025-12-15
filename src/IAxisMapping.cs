
namespace RoboPhredDev.Shipbreaker.SixAxis
{
    interface IAxisMapping
    {
        // For now, assuming page is the same page as the device.
        //ushort UsagePage { get; }
        ushort Usage { get; }

        string Device { get; }

        ShipbreakerAxisType GameAxis { get; }

        float GetValue(float normalizedValue);
    }
}
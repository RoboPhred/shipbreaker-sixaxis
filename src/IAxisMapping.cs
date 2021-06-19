
namespace RoboPhredDev.Shipbreaker.SixAxis
{
    interface IAxisMapping
    {
        // For now, assuming page is the same page as the device.
        //ushort UsagePage { get; }
        ushort Usage { get; }

        ShipbreakerAxisType GameAxis { get; }
        float Scale { get; }
    }
}
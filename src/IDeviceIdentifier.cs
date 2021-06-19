namespace RoboPhredDev.Shipbreaker.SixAxis
{
    interface IDeviceIdentifier
    {

        string DeviceName { get; }

        int VendorId { get; }

        int ProductId { get; }
    }
}

namespace RoboPhredDev.Shipbreaker.SixAxis
{
    interface IDeviceSpecification
    {

        string DeviceName { get; set; }

        int? VendorId { get; set; }

        int? ProductId { get; set; }
    }

    static class DeviceSpecificationUtils
    {
        public static bool SpecificationMatches(this IDeviceSpecification specification, IDeviceIdentifier identifier)
        {
            if (!string.IsNullOrEmpty(specification.DeviceName) && identifier.DeviceName != specification.DeviceName)
            {
                return false;
            }

            if (specification.VendorId.HasValue && identifier.VendorId != specification.VendorId)
            {
                return false;
            }

            if (specification.ProductId.HasValue && identifier.ProductId != specification.ProductId)
            {
                return false;
            }

            return true;
        }
    }
}
using YamlDotNet.Serialization;

namespace RoboPhredDev.Shipbreaker.SixAxis.Config
{
    class ConfiguredDeviceSpecification : IDeviceSpecification
    {

        [YamlMember(Alias = "deviceName")]
        public string DeviceName { get; set; }

        [YamlMember(Alias = "vendorId")]
        public int? VendorId { get; set; }

        [YamlMember(Alias = "productId")]
        public int? ProductId { get; set; }
    }
}
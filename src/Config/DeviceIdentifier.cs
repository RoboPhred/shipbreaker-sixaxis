using YamlDotNet.Serialization;

namespace RoboPhredDev.Shipbreaker.SixAxis.Config
{
    class DeviceIdentifier
    {
        [YamlMember(Alias = "vendorId")]
        public int? VendorIds { get; set; }

        [YamlMember(Alias = "productId")]
        public int? ProductId { get; set; }
    }
}
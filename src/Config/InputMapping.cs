using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;

namespace RoboPhredDev.Shipbreaker.SixAxis.Config
{
    class InputMapping
    {
        [YamlIgnore()]
        public string FilePath { get; set; }

        [YamlMember(Alias = "devices")]
        public List<DeviceIdentifier> Devices { get; set; } = new List<DeviceIdentifier>();

        [YamlMember(Alias = "axes")]
        public List<DeviceAxisMapping> Axes { get; set; } = new List<DeviceAxisMapping>();

        public bool ContainsDevice(int vendorId, int productId)
        {
            return this.Devices.FirstOrDefault(x => x.VendorIds == vendorId && x.ProductId == productId) != null;
        }

        public static InputMapping Load(string filePath)
        {
            var text = File.ReadAllText(filePath);
            var deserializer = new Deserializer();
            var mapping = deserializer.Deserialize<InputMapping>(text);
            mapping.FilePath = filePath;
            return mapping;
        }

        public static List<InputMapping> LoadAllMappings(string directoryPath)
        {
            var configPaths = from path in Directory.EnumerateFiles(directoryPath)
                              where path.EndsWith(".yaml") || path.EndsWith(".yml")
                              select path;

            var mappings = new List<InputMapping>();
            foreach (var path in configPaths)
            {
                try
                {
                    var mapping = Load(path);
                    mappings.Add(mapping);
                }
                catch (Exception e)
                {
                    Logging.Log($"Failed to load input configuration at \"{path}\": {e.Message}");
                }
            }

            return mappings;
        }
    }
}
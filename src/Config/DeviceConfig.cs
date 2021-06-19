using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;

namespace RoboPhredDev.Shipbreaker.SixAxis.Config
{
    class DeviceConfig : IInputMap
    {
        [YamlIgnore]
        public string FileName { get; set; }

        [YamlMember(Alias = "devices")]
        public List<ConfiguredDeviceSpecification> Devices { get; set; } = new List<ConfiguredDeviceSpecification>();

        [YamlMember(Alias = "axes")]
        public List<ConfiguredAxisMapping> Axes { get; set; } = new List<ConfiguredAxisMapping>();

        // TODO: Load from yaml
        public List<IButtonMapping> Buttons { get; set; } = new List<IButtonMapping>();

        IReadOnlyCollection<IAxisMapping> IInputMap.Axes => this.Axes;
        IReadOnlyCollection<IButtonMapping> IInputMap.Buttons => this.Buttons;

        public static DeviceConfig Load(string filePath)
        {
            var text = File.ReadAllText(filePath);
            var deserializer = new Deserializer();
            var mapping = deserializer.Deserialize<DeviceConfig>(text);
            mapping.FileName = Path.GetFileName(filePath);
            return mapping;
        }

        public static List<DeviceConfig> LoadAllMappings(string directoryPath)
        {
            var configPaths = from path in Directory.EnumerateFiles(directoryPath)
                              where path.EndsWith(".yaml") || path.EndsWith(".yml")
                              select path;

            var mappings = new List<DeviceConfig>();
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
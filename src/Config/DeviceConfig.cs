using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RoboPhredDev.Shipbreaker.SixAxis.Yaml;

namespace RoboPhredDev.Shipbreaker.SixAxis.Config
{
    class DeviceConfig : IInputMap
    {
        [YamlDotNet.Serialization.YamlIgnore]
        public string FileName { get; set; }

        [YamlDotNet.Serialization.YamlMember(Alias = "devices")]
        public List<ConfiguredDeviceSpecification> Devices { get; set; } = new();

        [YamlDotNet.Serialization.YamlMember(Alias = "axes")]
        public List<ConfiguredAxisMapping> Axes { get; set; } = new();

        // TODO: Load from yaml
        [YamlDotNet.Serialization.YamlIgnore]
        public List<IButtonMapping> Buttons { get; set; } = new();

        IReadOnlyCollection<IAxisMapping> IInputMap.Axes => this.Axes;
        IReadOnlyCollection<IButtonMapping> IInputMap.Buttons => this.Buttons;

        public static DeviceConfig Load(string filePath)
        {
            return Deserializer.Deserialize<DeviceConfig>(filePath);
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
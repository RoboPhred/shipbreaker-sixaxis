using RoboPhredDev.Shipbreaker.SixAxis.ButtonCommands;
using RoboPhredDev.Shipbreaker.SixAxis.Yaml;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace RoboPhredDev.Shipbreaker.SixAxis.Config
{
    class ConfiguredButtonMapping : IButtonMapping, IAfterYamlDeserialization
    {
        [YamlMember(Alias = "device")]
        public string Device { get; set; }

        [YamlMember(Alias = "buttonUsage")]
        public ushort ButtonUsage { get; set; }

        [YamlMember(Alias = "command")]
        public IButtonCommand Command { get; set; }

        public ushort Usage => this.ButtonUsage;

        public void AfterDeserialized(Mark start, Mark end)
        {
            if (ButtonUsage == 0)
            {
                throw new YamlException(start, end, "Button mapping must specify a buttonUsage greater than or equal to 1");
            }

            if (Command == null)
            {
                throw new YamlException(start, end, "Button mapping must specify a command");
            }
        }
    }
}
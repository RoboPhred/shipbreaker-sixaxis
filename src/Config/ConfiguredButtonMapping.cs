using RoboPhredDev.Shipbreaker.SixAxis.ButtonCommands;
using YamlDotNet.Serialization;

namespace RoboPhredDev.Shipbreaker.SixAxis.Config
{
    class ConfiguredButtonMapping : IButtonMapping
    {
        [YamlMember(Alias = "buttonUsage")]
        public ushort ButtonUsage { get; set; }

        // TODO: Parse from yaml
        public IButtonCommand Command { get; set; }

        public ushort Usage => this.ButtonUsage;
    }
}
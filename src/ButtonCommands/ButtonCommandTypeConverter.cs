using System;
using System.Linq;
using RoboPhredDev.Shipbreaker.SixAxis.Yaml;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace RoboPhredDev.Shipbreaker.SixAxis.ButtonCommands
{
    internal class ButtonCommandTypeConverter : YamlDotNet.Serialization.IYamlTypeConverter
    {
        private static readonly Type[] ButtonCommandTypes = (from type in typeof(IButtonCommand).Assembly.GetTypes()
                                                             where typeof(IButtonCommand).IsAssignableFrom(type)
                                                             where !type.IsAbstract
                                                             select type).ToArray();
        public bool Accepts(Type type)
        {
            return type == typeof(IButtonCommand);
        }

        public object ReadYaml(IParser parser, Type type)
        {
            var start = parser.Current.Start;
            if (parser.TryConsume<Scalar>(out var scalar))
            {
                return CreateButtonCommand(scalar.Value, null, start);
            }

            var replayParser = ReplayParser.FromMapping(parser);

            replayParser.Start();
            var partialCommand = Deserializer.Deserialize<PartialCommand>(replayParser);
            replayParser.Reset();

            if (string.IsNullOrWhiteSpace(partialCommand.CommandType))
            {
                throw new YamlException(start, start, "Button command is invalid.");
            }

            replayParser.Start();
            return CreateButtonCommand(partialCommand.CommandType, replayParser, start);
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            throw new NotSupportedException();
        }

        private IButtonCommand CreateButtonCommand(string name, IParser optionsParser, Mark start)
        {
            var commandType = GetButtonCommandType(name, start);
            if (optionsParser == null)
            {
                return (IButtonCommand)Activator.CreateInstance(commandType);
            }
            else
            {
                return (IButtonCommand)Deserializer.Deserialize(optionsParser, commandType);
            }
        }

        private Type GetButtonCommandType(string name, Mark start)
        {
            var match = Array.Find(ButtonCommandTypes, x => x.Name == name) ?? Array.Find(ButtonCommandTypes, x => x.Name == name + "Command");
            if (match == null)
            {
                throw new YamlException(start, start, $"Unknown button command \"{name}\"");
            }
            return match;
        }

        private class PartialCommand
        {
            [YamlDotNet.Serialization.YamlMember(Alias = "commandType")]
            public string CommandType { get; set; }
        }
    }
}
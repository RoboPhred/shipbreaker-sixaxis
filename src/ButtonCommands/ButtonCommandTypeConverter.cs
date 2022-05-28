using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BBI.Unity.Game;
using InControl;
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
                                                             where type.Name.EndsWith("Command")
                                                             select type).ToArray();

        private static readonly string[] PlayerActionCommandNames = (from field in typeof(GameplayActions).GetFields(BindingFlags.Public | BindingFlags.Instance).Concat(typeof(GameplayActions).GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
                                                                     where field.FieldType == typeof(PlayerAction)
                                                                     select field.Name).ToArray();

        static ButtonCommandTypeConverter()
        {
            Logging.Log("Registered custom command handlers: " + string.Join(", ", ButtonCommandTypes.Select(t => t.Name)));
            Logging.Log("Registered game command handlers: " + string.Join(", ", PlayerActionCommandNames));
        }

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
            try
            {
                var buttonCommand = GetButtonCommand(name, optionsParser);
                if (buttonCommand != null)
                {
                    return buttonCommand;
                }

                var playerActionCommand = GetPlayerActionCommand(name);
                if (playerActionCommand != null)
                {
                    return playerActionCommand;
                }
            }
            catch (Exception e)
            {
                Logging.Log(new Dictionary<string, string> {
                    { "Command", name },
                    { "Mark", start.ToString()},
                    { "Error", e.Message }
                }, $"Could not create button command {name} at {start}: {e.Message}");
                return new NullCommand();
            }

            Logging.Log(new Dictionary<string, string> {
                { "Command", name },
                { "Mark", start.ToString()},
            }, $"Unknown command {name} at {start}");
            return new NullCommand();
        }

        private IButtonCommand GetButtonCommand(string name, IParser optionsParser)
        {
            var commandType = Array.Find(ButtonCommandTypes, x => x.Name == name) ?? Array.Find(ButtonCommandTypes, x => x.Name == name + "Command");
            if (commandType == null)
            {
                return null;
            }

            if (optionsParser == null)
            {
                return (IButtonCommand)Activator.CreateInstance(commandType);
            }
            else
            {
                return (IButtonCommand)Deserializer.Deserialize(optionsParser, commandType);
            }
        }

        private IButtonCommand GetPlayerActionCommand(string name)
        {
            if (Array.Find(PlayerActionCommandNames, x => x == name) != null)
            {
                return new PlayerActionCommandProxy(name);
            }

            // Also check for "m" + name, for the mRotateHead* commands.
            if (Array.Find(PlayerActionCommandNames, x => x == "m" + name) != null)
            {
                return new PlayerActionCommandProxy("m" + name);
            }

            return null;
        }

        private class PartialCommand
        {
            [YamlDotNet.Serialization.YamlMember(Alias = "commandType")]
            public string CommandType { get; set; }
        }
    }
}
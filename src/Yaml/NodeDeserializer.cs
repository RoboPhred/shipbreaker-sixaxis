using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace RoboPhredDev.Shipbreaker.SixAxis.Yaml
{
    /// <summary>
    /// Wraps a <see cref="INodeDeserializer"/> with our own modifications.
    /// </summary>
    public class NodeDeserializer : INodeDeserializer
    {
        private readonly INodeDeserializer nodeDeserializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeDeserializer"/> class.
        /// </summary>
        /// <param name="nodeDeserializer">The ancestor deserializer to use.</param>
        public NodeDeserializer(INodeDeserializer nodeDeserializer)
        {
            this.nodeDeserializer = nodeDeserializer;
        }

        /// <inheritdoc/>
        public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
        {
            Mark start = null;
            if (reader.Accept<ParsingEvent>(out var parsingEvent))
            {
                start = parsingEvent?.Start;
            }

            if (this.nodeDeserializer.Deserialize(reader, expectedType, nestedObjectDeserializer, out value))
            {
                if (value is IAfterYamlDeserialization afterDeserialized)
                {
                    afterDeserialized.AfterDeserialized(start, reader.Current?.End);
                }

                return true;
            }

            return false;
        }
    }
}

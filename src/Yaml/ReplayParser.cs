using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace RoboPhredDev.Shipbreaker.SixAxis
{
    /// <summary>
    /// A parser meant to replay previously parsed events.
    /// </summary>
    internal class ReplayParser : IParser
    {
        private readonly List<ParsingEvent> replayEvents = new();

        private IEnumerator<ParsingEvent> replayEnumerator = null;

        /// <inheritdoc/>
        public ParsingEvent Current => this.replayEnumerator != null ? this.replayEnumerator.Current : throw new InvalidOperationException("ReplayParser has not been activated.");

        /// <summary>
        /// Enqueue a parsing event to emit when the parser starts.
        /// </summary>
        /// <param name="parsingEvent">The event to enqueue.</param>
        public void Enqueue(ParsingEvent parsingEvent)
        {
            if (this.replayEnumerator != null)
            {
                throw new InvalidOperationException("Cannot enqueue to a ReplayParser that has already been started.");
            }

            this.replayEvents.Add(parsingEvent);
        }

        /// <summary>
        /// Start the parser for its parse operations.
        /// </summary>
        public void Start()
        {
            if (this.replayEnumerator != null)
            {
                throw new InvalidOperationException("This ReplayParser has already been started.");
            }

            this.replayEnumerator = this.replayEvents.GetEnumerator();
        }

        public void Reset()
        {
            this.replayEnumerator = null;
        }

        /// <inheritdoc/>
        public bool MoveNext()
        {
            return this.replayEnumerator.MoveNext();
        }

        /// <summary>
        /// Builds a replay parser containing the next mapping.
        /// </summary>
        /// <param name="parser">The parser to read the mapping from.</param>
        /// <returns>A ReplayParser that can re-emit the mapping.</returns>
        public static ReplayParser FromMapping(IParser parser)
        {
            var replayParser = new ReplayParser();

            var mappingStart = parser.Consume<MappingStart>();
            replayParser.Enqueue(mappingStart);
            MappingEnd mappingEnd;
            while (!parser.TryConsume(out mappingEnd))
            {
                var depth = 0;
                do
                {
                    var next = parser.Consume<ParsingEvent>();
                    depth += next.NestingIncrease;
                    replayParser.Enqueue(next);
                }
                while (depth > 0);
            }
            replayParser.Enqueue(mappingEnd);

            return replayParser;
        }
    }
}

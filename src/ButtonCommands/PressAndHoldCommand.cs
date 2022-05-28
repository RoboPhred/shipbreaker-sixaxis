
using System;
using YamlDotNet.Serialization;

namespace RoboPhredDev.Shipbreaker.SixAxis.ButtonCommands
{
    class PressAndHoldCommand : IButtonCommand, IUpdatable
    {
        private long? downTick;

        private long executeTick;
        private ExecutingPress executingPress;

        [YamlMember(Alias = "shortPress")]
        public IButtonCommand ShortPress { get; set; }

        [YamlMember(Alias = "longPress")]
        public IButtonCommand LongPress { get; set; }

        [YamlMember(Alias = "duration")]
        public float Duration { get; set; } = 0.8f;

        public PressAndHoldCommand()
        {
            SixAxisPlugin.Instance.AddUpdatable(this);
        }

        public void Press()
        {
            downTick = DateTime.Now.Ticks;
        }

        public void Release()
        {
            // Check if we had a short press.
            if (downTick.HasValue && downTick.Value + (long)(Duration * TimeSpan.TicksPerSecond) > DateTime.Now.Ticks)
            {
                this.ShortPress?.Press();

                // Record that we are short pressing so the button logic can run in Update()
                this.executeTick = DateTime.Now.Ticks;
                this.executingPress = ExecutingPress.Short;
            }

            // Long presses hold the button down until released by the player.
            if (executingPress == ExecutingPress.Long)
            {
                this.LongPress?.Release();
                executeTick = 0;
                executingPress = ExecutingPress.None;
            }

            downTick = null;
        }

        public void Update()
        {
            if (downTick.HasValue && downTick.Value + (long)(Duration * TimeSpan.TicksPerSecond) <= DateTime.Now.Ticks)
            {
                this.LongPress?.Press();

                this.executeTick = DateTime.Now.Ticks;
                this.executingPress = ExecutingPress.Long;
            }

            // If we released on a short press, then we need to hold the short button down for a period of time for the game to register.
            // Give it a half second.
            // For long presses, we will keep holding the binding button down until the real button is released.
            if (executingPress == ExecutingPress.Short && executeTick + (0.5 * TimeSpan.TicksPerSecond) <= DateTime.Now.Ticks)
            {
                this.ShortPress?.Release();
                executeTick = 0;
                executingPress = ExecutingPress.None;
            }
        }

        private enum ExecutingPress
        {
            None,
            Short,
            Long
        }
    }
}
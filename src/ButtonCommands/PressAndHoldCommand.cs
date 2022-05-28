
using System;
using YamlDotNet.Serialization;

namespace RoboPhredDev.Shipbreaker.SixAxis.ButtonCommands
{
    class PressAndHoldCommand : IButtonCommand, IUpdatable
    {
        private long? downTick;
        private bool isTrackingPress;

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
            isTrackingPress = true;
        }

        public void Release()
        {
            if (isTrackingPress)
            {
                isTrackingPress = false;
                this.ShortPress.Press();

                this.executeTick = DateTime.Now.Ticks;
                this.executingPress = ExecutingPress.Short;
            }
        }

        public void Update()
        {
            if (isTrackingPress && downTick + (long)(Duration * TimeSpan.TicksPerSecond) <= DateTime.Now.Ticks)
            {
                isTrackingPress = false;
                this.LongPress.Press();

                this.executeTick = DateTime.Now.Ticks;
                this.executingPress = ExecutingPress.Long;
            }

            if (executingPress != ExecutingPress.None && executeTick + (0.5 * TimeSpan.TicksPerSecond) <= DateTime.Now.Ticks)
            {
                switch (executingPress)
                {
                    case ExecutingPress.Short:
                        this.ShortPress.Release();
                        break;
                    case ExecutingPress.Long:
                        this.LongPress.Release();
                        break;
                }
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

using InControl;

namespace RoboPhredDev.Shipbreaker.SixAxis.ButtonCommands
{
    abstract class PlayerActionCommand : IButtonCommand
    {

        private readonly RemotedBindingSource bindingSource = new();

        protected PlayerActionCommand()
        {
            ControlsReadyMonitor.RunWhenControlsReady(() => GetPlayerAction().AddBinding(bindingSource));
        }

        protected abstract PlayerAction GetPlayerAction();

        public void Press()
        {
            bindingSource.State = true;
        }

        public void Release()
        {
            bindingSource.State = false;
        }
    }
}

using BBI.Unity.Game;
using InControl;

namespace RoboPhredDev.Shipbreaker.SixAxis.ButtonCommands
{
    abstract class PlayerActionCommand : IButtonCommand
    {

        private RemotedBindingSource bindingSource = new();

        protected PlayerActionCommand()
        {
            GameplayActionsMonitor.RunWhenGameplayActionsCreated((actions) =>
            {
                this.bindingSource = new();
                GetPlayerAction(actions).AddBinding(bindingSource);
            });
        }

        protected abstract PlayerAction GetPlayerAction(GameplayActions actions);

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
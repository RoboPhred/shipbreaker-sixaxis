
using BBI.Unity.Game;

namespace RoboPhredDev.Shipbreaker.SixAxis.ButtonCommands
{
    class PlaceTetherCommand : IButtonCommand
    {
        private RemotedBindingSource bindingSource = new RemotedBindingSource();

        public PlaceTetherCommand()
        {
            ControlsReadyMonitor.RunWhenControlsReady(() =>
            {
                LynxControls.Instance.GameplayActions.PlaceTether.AddBinding(bindingSource);
            });
        }

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
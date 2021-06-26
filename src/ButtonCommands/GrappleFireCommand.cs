
using BBI.Unity.Game;

namespace RoboPhredDev.Shipbreaker.SixAxis.ButtonCommands
{
    class GrappleFireCommand : IButtonCommand
    {
        private RemotedBindingSource bindingSource = new RemotedBindingSource();

        public GrappleFireCommand()
        {
            ControlsReadyMonitor.RunWhenControlsReady(() =>
            {
                LynxControls.Instance.GameplayActions.GrappleFire.AddBinding(bindingSource);
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

using BBI.Unity.Game;

namespace RoboPhredDev.Shipbreaker.SixAxis.ButtonCommands
{
    class SelectGrappleCommand : IButtonCommand
    {
        private RemotedBindingSource grappleCommand = new RemotedBindingSource();

        public SelectGrappleCommand()
        {
            ControlsReadyMonitor.RunWhenControlsReady(() =>
            {
                LynxControls.Instance.GameplayActions.SelectGrapple.AddBinding(grappleCommand);
            });
        }

        public void Press()
        {
            grappleCommand.State = true;
        }

        public void Release()
        {
            grappleCommand.State = false;
        }
    }
}
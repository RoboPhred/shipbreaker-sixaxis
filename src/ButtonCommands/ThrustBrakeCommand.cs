
using BBI.Unity.Game;

namespace RoboPhredDev.Shipbreaker.SixAxis.ButtonCommands
{
    class ThrustBrakeCommand : IButtonCommand
    {
        private RemotedBindingSource bindingSource = new RemotedBindingSource();

        public ThrustBrakeCommand()
        {
            ControlsReadyMonitor.RunWhenControlsReady(() =>
            {
                // FIXME: Neither of these are working

                LynxControls.Instance.GameplayActions.ThrustBrakeLeft.AddBinding(bindingSource);
                LynxControls.Instance.GameplayActions.ThrustBrakeRight.AddBinding(bindingSource);
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
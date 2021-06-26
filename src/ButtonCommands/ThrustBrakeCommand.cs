
using BBI.Unity.Game;

namespace RoboPhredDev.Shipbreaker.SixAxis.ButtonCommands
{
    class ThrustBrakeCommand : IButtonCommand
    {
        private RemotedBindingSource bindingSource1 = new RemotedBindingSource();
        private RemotedBindingSource bindingSource2 = new RemotedBindingSource();

        public ThrustBrakeCommand()
        {
            ControlsReadyMonitor.RunWhenControlsReady(() =>
            {
                LynxControls.Instance.GameplayActions.ThrustBrakeLeft.AddBinding(bindingSource1);
                LynxControls.Instance.GameplayActions.ThrustBrakeRight.AddBinding(bindingSource2);
            });
        }

        public void Press()
        {
            bindingSource1.State = true;
            bindingSource2.State = true;
        }

        public void Release()
        {
            bindingSource1.State = false;
            bindingSource2.State = false;
        }
    }
}
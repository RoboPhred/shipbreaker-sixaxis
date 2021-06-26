
using BBI.Unity.Game;

namespace RoboPhredDev.Shipbreaker.SixAxis.ButtonCommands
{
    class ThrustBrakeCommand : IButtonCommand
    {
        private readonly RemotedBindingSource brakeLeft = new();
        private readonly RemotedBindingSource brakeRight = new();

        public ThrustBrakeCommand()
        {
            ControlsReadyMonitor.RunWhenControlsReady(() =>
            {
                // Controller needs to send both of these to brake.
                // Since we tell the game that our input is from a controller, we need to send both.
                LynxControls.Instance.GameplayActions.ThrustBrakeLeft.AddBinding(brakeLeft);
                LynxControls.Instance.GameplayActions.ThrustBrakeRight.AddBinding(brakeRight);
            });
        }

        public void Press()
        {
            brakeLeft.State = true;
            brakeRight.State = true;
        }

        public void Release()
        {
            brakeLeft.State = false;
            brakeRight.State = false;
        }
    }
}
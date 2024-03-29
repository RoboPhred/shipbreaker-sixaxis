
using BBI.Unity.Game;

namespace RoboPhredDev.Shipbreaker.SixAxis.ButtonCommands
{
    class ThrustBrakeCommand : IButtonCommand
    {
        private RemotedBindingSource brakeLeft = new();
        private RemotedBindingSource brakeRight = new();

        public ThrustBrakeCommand()
        {
            GameplayActionsMonitor.RunWhenGameplayActionsCreated((actions) =>
            {
                // We need to create a new binding source each time, as the old source will refuse to re-bind.
                this.brakeLeft = new();
                this.brakeRight = new();

                // Controller needs to send both of these to brake.
                // Since we tell the game that our input is from a controller, we need to send both.
                actions.ThrustBrakeLeft.AddBinding(brakeLeft);
                actions.ThrustBrakeRight.AddBinding(brakeRight);
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
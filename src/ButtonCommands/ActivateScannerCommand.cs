
using BBI.Unity.Game;

namespace RoboPhredDev.Shipbreaker.SixAxis.ButtonCommands
{
    class ActivateScannerCommand : IButtonCommand
    {
        private RemotedBindingSource activateBindingSource = new();
        private RemotedBindingSource cycleNextBindingSource = new();

        public bool CycleIfActive { get; set; }

        public ActivateScannerCommand()
        {
            GameplayActionsMonitor.RunWhenGameplayActionsCreated((actions) =>
            {
                // We need to create a new binding source each time, as the old source will refuse to re-bind.
                this.activateBindingSource = new();
                this.cycleNextBindingSource = new();
                actions.ActivateScanner.AddBinding(activateBindingSource);
                actions.ScanCycleRight.AddBinding(cycleNextBindingSource);
            });
        }

        public void Press()
        {
            if (CycleIfActive && EquipmentControllerInstance.Instance.CurrentEquipment == EquipmentController.Equipment.Scanner)
            {
                cycleNextBindingSource.State = true;
            }
            else
            {
                activateBindingSource.State = true;
            }
        }

        public void Release()
        {
            cycleNextBindingSource.State = false;
            activateBindingSource.State = false;
        }
    }
}
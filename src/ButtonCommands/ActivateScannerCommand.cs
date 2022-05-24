
using BBI.Unity.Game;

namespace RoboPhredDev.Shipbreaker.SixAxis.ButtonCommands
{
    class ActivateScannerCommand : IButtonCommand
    {
        private readonly RemotedBindingSource activateBindingSource = new();
        private readonly RemotedBindingSource cycleNextBindingSource = new();

        private bool didCycleNext = false;

        public bool CycleIfActive { get; set; }

        public ActivateScannerCommand()
        {
            GameplayActionsMonitor.RunWhenGameplayActionsCreated((actions) =>
            {
                actions.ActivateScanner.AddBinding(activateBindingSource);
                actions.ScanCycleRight.AddBinding(cycleNextBindingSource);
            });
        }

        public void Press()
        {
            if (CycleIfActive && EquipmentControllerInstance.Instance.CurrentEquipment == EquipmentController.Equipment.Scanner)
            {
                cycleNextBindingSource.State = true;
                didCycleNext = true;
            }
            else
            {
                activateBindingSource.State = true;
            }
        }

        public void Release()
        {
            if (didCycleNext)
            {
                didCycleNext = false;
                cycleNextBindingSource.State = false;
            }
            else
            {
                activateBindingSource.State = false;
            }
        }
    }
}
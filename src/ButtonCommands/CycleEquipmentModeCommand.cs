
using BBI.Unity.Game;
using InControl;

namespace RoboPhredDev.Shipbreaker.SixAxis.ButtonCommands
{
    class CycleEquipmentMode : PlayerActionCommand
    {
        protected override PlayerAction GetPlayerAction()
        {
            return LynxControls.Instance.GameplayActions.CycleEquipmentMode;
        }
    }
}
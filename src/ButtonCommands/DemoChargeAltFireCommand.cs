
using BBI.Unity.Game;
using InControl;

namespace RoboPhredDev.Shipbreaker.SixAxis.ButtonCommands
{
    class DemoChargeAltFireCommand : PlayerActionCommand
    {
        protected override PlayerAction GetPlayerAction()
        {
            return LynxControls.Instance.GameplayActions.DemoChargeAltFire;
        }
    }
}

using BBI.Unity.Game;
using InControl;

namespace RoboPhredDev.Shipbreaker.SixAxis.ButtonCommands
{
    class CutterAltFireCommand : PlayerActionCommand
    {
        protected override PlayerAction GetPlayerAction()
        {
            return LynxControls.Instance.GameplayActions.CutterAltFire;
        }
    }
}
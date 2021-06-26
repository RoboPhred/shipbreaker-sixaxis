
using BBI.Unity.Game;
using InControl;

namespace RoboPhredDev.Shipbreaker.SixAxis.ButtonCommands
{
    class GrappleFireCommand : PlayerActionCommand
    {
        protected override PlayerAction GetPlayerAction()
        {
            return LynxControls.Instance.GameplayActions.GrappleFire;
        }
    }
}
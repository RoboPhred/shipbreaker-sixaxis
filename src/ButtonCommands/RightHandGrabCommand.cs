
using BBI.Unity.Game;
using InControl;

namespace RoboPhredDev.Shipbreaker.SixAxis.ButtonCommands
{
    class RightHandGrabCommand : PlayerActionCommand
    {
        protected override PlayerAction GetPlayerAction(GameplayActions actions)
        {
            return actions.RightHandGrab;
        }
    }
}
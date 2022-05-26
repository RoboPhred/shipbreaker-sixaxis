
using BBI.Unity.Game;
using InControl;

namespace RoboPhredDev.Shipbreaker.SixAxis.ButtonCommands
{
    class ThrustMoveUpCommand : PlayerActionCommand
    {
        protected override PlayerAction GetPlayerAction(GameplayActions actions)
        {
            return actions.ThrustMoveUp;
        }
    }
}

using BBI.Unity.Game;
using InControl;

namespace RoboPhredDev.Shipbreaker.SixAxis.ButtonCommands
{
    class RollBodyLeftCommand : PlayerActionCommand
    {
        protected override PlayerAction GetPlayerAction(GameplayActions actions)
        {
            return actions.RollBodyLeft;
        }
    }
}
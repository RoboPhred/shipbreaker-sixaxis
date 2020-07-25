using BBI.Unity.Game;
using HarmonyLib;

namespace RoboPhredDev.Shipbreaker.SixAxis
{
    [HarmonyPatch(typeof(ThrustController), "Update")]
    static class ThrustInjector
    {

        static void Prefix()
        {
            // TODO: Override value of LynxControls.Instance.GameplayActions ThrustHorizontal, ThrustVertical, ThrustDepth
        }
    }
}

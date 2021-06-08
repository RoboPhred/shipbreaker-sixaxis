
using BBI.Unity.Game;
using HarmonyLib;
using UnityEngine;

namespace RoboPhredDev.Shipbreaker.SixAxis
{
    [HarmonyPatch(typeof(BBI.OrientationController), "get_MouseSensitivity")]
    static class RotationSensitivityInjector
    {
        static bool Prefix(BBI.OrientationController __instance, ref Vector3 __result)
        {
            var disableMouseLook = Reflection.GetPrivateField<bool>(__instance, "mDisableMouseLook");
            if (disableMouseLook)
            {
                __result = Vector3.zero;
                return false;
            }

            __result = new Vector3(0.7f * LynxControlsUserOptions.MouseSensitivity, 0.7f * LynxControlsUserOptions.MouseSensitivity, 150 * LynxControlsUserOptions.RollSensitivity);
            return false;
        }
    }
}



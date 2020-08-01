
using Carbon.Core.Unity;
using HarmonyLib;

namespace RoboPhredDev.Shipbreaker.SixAxis
{
    [HarmonyPatch(typeof(UnityLogHooks), "SetUp")]
    sealed class CarbonLogSuppressor
    {
        static bool Prefix()
        {
            // Should only be turned off when debugging.
            // Logging.Log("Supressing carbon logs");
            // return false;
            return true;
        }
    }
}
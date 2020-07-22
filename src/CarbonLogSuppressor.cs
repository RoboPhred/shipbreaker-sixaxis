
using Carbon.Core.Unity;
using HarmonyLib;

namespace RoboPhredDev.Shipbreaker.SixAxis
{
    [HarmonyPatch(typeof(UnityLogHooks), "SetUp")]
    sealed class CarbonLogSuppressor
    {
        static bool Prefix()
        {
            // TODO: Should only be turned on when debugging.
            Logging.Log("Supressing carbon logs");
            return false;
        }
    }
}
using BBI.Unity.Game;
using HarmonyLib;

namespace RoboPhredDev.Shipbreaker.SixAxis
{
    [HarmonyPatch(typeof(EquipmentController), "Start")]
    static class EquipmentControllerInstance
    {
        public static EquipmentController Instance { get; private set; }
        static void Postfix(EquipmentController __instance)
        {
            Instance = __instance;
        }
    }
}



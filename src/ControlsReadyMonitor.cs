using System;
using System.Collections.Generic;
using BBI.Unity.Game;
using HarmonyLib;

namespace RoboPhredDev.Shipbreaker.SixAxis
{
    [HarmonyPatch(typeof(LynxControls), "Start")]
    static class ControlsReadyMonitor
    {
        private static bool initialized = false;
        private static readonly List<Action> pendingActions = new();

        public static void RunWhenControlsReady(Action action)
        {
            if (initialized)
            {
                action();
            }
            else
            {
                pendingActions.Add(action);
            }
        }

        static void Postfix()
        {
            initialized = true;
            foreach (var action in pendingActions)
            {
                action();
            }
            pendingActions.Clear();
        }
    }
}



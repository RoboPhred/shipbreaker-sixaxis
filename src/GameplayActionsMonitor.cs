using System;
using System.Collections.Generic;
using BBI.Unity.Game;
using HarmonyLib;

namespace RoboPhredDev.Shipbreaker.SixAxis
{
    [HarmonyPatch(typeof(GameplayActions), "CreateWithDefaultBindings")]
    static class GameplayActionsMonitor
    {
        private static GameplayActions actionSet = null;
        private static readonly List<Action<GameplayActions>> pendingActions = new();

        public static GameplayActions CurrentGameplayActions
        {
            get {
                return actionSet;
            }
        }

        public static void RunWhenGameplayActionsCreated(Action<GameplayActions> action)
        {
            // if (actionSet != null)
            // {
            //     action(actionSet);
            // }

            // pendingActions.Add(action);
        }

        static void Postfix(GameplayActions __instance)
        {
            // actionSet = __instance;
            // foreach (var action in pendingActions)
            // {
            //     action(actionSet);
            // }
        }
    }
}



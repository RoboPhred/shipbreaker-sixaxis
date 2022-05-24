using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using BBI.Unity.Game;
using HarmonyLib;
using UnityEngine;

namespace RoboPhredDev.Shipbreaker.SixAxis
{
    [HarmonyPatch(typeof(ThrustController), "Update")]
    static class ThrustInjector
    {

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase method)
        {
            var instructionList = instructions.ToList();

            var insertPoint = instructionList.FindIndex((x) =>
            {
                if (x.opcode != OpCodes.Ldfld)
                {
                    return false;
                }
                var info = x.operand as FieldInfo;
                if (info == null)
                {
                    return false;
                }

                return info.Name == "ThrustDepth";
            });

            if (insertPoint == -1)
            {
                Logging.Log("Failed to patch ThrustController.Update.  The injection point was not found.  Thrust commands will not function.");
                return instructions;
            }

            // Jump over additional instructions
            insertPoint += 3;

            instructionList.Insert(insertPoint++, new CodeInstruction(OpCodes.Ldloc_1));
            instructionList.Insert(insertPoint++, new CodeInstruction(OpCodes.Call, typeof(ThrustInjector).GetMethod("ApplySixAxisTranslation", BindingFlags.Static | BindingFlags.NonPublic)));
            instructionList.Insert(insertPoint++, new CodeInstruction(OpCodes.Stloc_1));

            Logging.Log("Successfully patched ThrustController.Update");

            return instructionList;
        }

        static Vector3 ApplySixAxisTranslation(Vector3 vector)
        {
            // If gameplay actions are disabled, do not send data.
            // The game will corrupt if it gets any thrust vector input while paused.
            if (ActionBlockerService.Instance.IsBlockingAnyAndAllActions())
            {
                return vector;
            }

            return VectorUtils.Clamp(vector + InputManager.Translation, -1f, 1f);
        }
    }
}

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

            // Find the last call to GetOneAxisInputControlValue to inject at
            // FIXME: We should instead look for the LDLOCA.S for rawValue, but for some reason every single LocalIndex from that returns 19 even for different locals
            var insertPoint = instructionList.FindLastIndex((x) =>
            {
                if (x.opcode != OpCodes.Call)
                {
                    return false;
                }

                if (!(x.operand is MethodInfo info))
                {
                    return false;
                }

                return info.Name == "GetOneAxisInputControlValue";
            });

            if (insertPoint == -1)
            {
                Logging.Log("Failed to patch ThrustController.Update.  The injection point was not found.  Thrust commands will not function.");
                return instructions;
            }

            // Skip over the final storage calls, so we can inject with the previous value.
            insertPoint += 2;

            // Insert right at the rawVector assignment, pushing it out after our translation application.
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

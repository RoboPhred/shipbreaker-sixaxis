
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using BBI;
using BBI.Unity.Game;
using HarmonyLib;
using UnityEngine;

namespace RoboPhredDev.Shipbreaker.SixAxis
{
    [HarmonyPatch(typeof(BBI.OrientationController), "HandleAxisRotation")]
    static class OrientationInjector
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var found = false;
            foreach (var instruction in instructions)
            {
                yield return instruction;
                if (found)
                {
                    continue;
                }

                // Look for the call to HandleRollAudio and hyjack the value of 'vector' after it.
                // TODO: Probably want to do this right after 'vector = LynxControls.Instance.GameplayActions.RotateBody.Vector;'

                if (instruction.opcode != OpCodes.Call)
                {
                    continue;
                }
                var method = (MethodInfo)instruction.operand;
                if (method.Name != "HandleRollAudio")
                {
                    continue;
                }

                found = true;

                yield return new CodeInstruction(OpCodes.Ldloc_0);
                yield return new CodeInstruction(OpCodes.Call, typeof(OrientationInjector).GetMethod("ApplySixAxisRotation", BindingFlags.Static | BindingFlags.NonPublic));
                yield return new CodeInstruction(OpCodes.Stloc_0);
            }
        }

        static Vector3 ApplySixAxisRotation(Vector3 vector)
        {
            // TODO: Merge our input with existing input
            var rotation = new Vector3(InputHandler.RZ, InputHandler.RX, -InputHandler.RY);
            return rotation;
        }
    }
}



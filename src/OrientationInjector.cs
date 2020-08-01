
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
            // TODO: We should make a post-processor for HandleRollAudio and trigger roll audio if
            //  - It has not been triggered
            //  - we have a roll axis value

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
            // If gameplay actions are disabled, do not send data.
            // Unlike ThrustController, orientation handles input while paused just fine.
            // However, the base game never registers input during this time, so we should
            // mirror it.
            if (LynxControls.Instance.GameplayActions.Enabled == false)
            {
                return vector;
            }

            return InputHandler.Rotation;
        }
    }
}



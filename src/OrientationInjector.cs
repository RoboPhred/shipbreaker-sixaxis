
using BBI;
using BBI.Unity.Game;
using HarmonyLib;
using UnityEngine;

namespace RoboPhredDev.Shipbreaker.SixAxis
{
    [HarmonyPatch(typeof(BBI.OrientationController), "HandleAxisRotation")]
    static class OrientationInjector
    {
        static void Postfix(OrientationController __instance)
        {
            ApplyRotation(__instance, new Vector3(InputHandler.RZ, InputHandler.RX, -InputHandler.RY));
        }

        static void ApplyRotation(OrientationController orientationController, Vector3 vec)
        {

            // TODO: Something is wrong with this, rotation along one side of one axis is half the speed of the other.  All axes seem slower than usual control input

            // The following code was pilfered from OrientationController.HandleAxisRotation
            float deltaTime = Mathf.Clamp(Time.deltaTime, 0.0f, Reflection.GetPrivateField<float>(orientationController, "m_MaxDeltaTime"));
            var sensitivityCurve = Reflection.GetPrivateField<AnimationCurve>(orientationController, "m_ControllerSensitivityCurve");
            var controllerSensitivity = Reflection.GetPrivateProperty<Vector3>(orientationController, "ControllerSensitivity");
            var mRigidbodyReference = Reflection.GetPrivateField<Rigidbody>(orientationController, "mRigidbodyReference");
            var maxVelocity = Reflection.GetPrivateField<Vector3>(orientationController, "m_ControllerMaxVelocity");
            var mGrabController = Reflection.GetPrivateField<GrabController>(orientationController, "mGrabController");
            var mCurrentAxisDrag = Reflection.GetPrivateField<Vector3>(orientationController, "mCurrentAxisDrag");
            var m_FreeFloatAngularDrag = Reflection.GetPrivateField<float>(orientationController, "m_FreeFloatAngularDrag");

            var mAxisVelocityX = Reflection.GetPrivateField<Vector3>(orientationController, "mAxisVelocityX");
            var mAxisVelocityY = Reflection.GetPrivateField<Vector3>(orientationController, "mAxisVelocityY");
            var mAxisVelocityZ = Reflection.GetPrivateField<Vector3>(orientationController, "mAxisVelocityZ");

            // TODO: HandleRollAudio calls audio based on inputs, we should reproduce it based on our axis inputs.

            vec *= sensitivityCurve.Evaluate(vec.magnitude);

            float absZ = Mathf.Abs(vec.z);
            vec.z *= controllerSensitivity.z + absZ;

            float halfDeltaTimeSquared = (float)((double)deltaTime * (double)deltaTime * 0.5);
            Vector3 incrementVelocityX = Vector3.up * vec.x * halfDeltaTimeSquared;
            Vector3 incrementVelocityY = Vector3.right * -vec.y * halfDeltaTimeSquared;
            Vector3 incrementVelocityZ = Vector3.forward * -vec.z * halfDeltaTimeSquared;
            mRigidbodyReference.MoveRotation(
                mRigidbodyReference.rotation
                * Quaternion.Euler(mAxisVelocityX + incrementVelocityX)
                * Quaternion.Euler(mAxisVelocityY + incrementVelocityY)
                * Quaternion.Euler(mAxisVelocityZ + incrementVelocityZ)
            );

            if ((double)vec.sqrMagnitude > 0.0)
            {
                mAxisVelocityX += incrementVelocityX;
                mAxisVelocityY += incrementVelocityY;
                mAxisVelocityZ += incrementVelocityZ;
                mAxisVelocityX = Vector3.ClampMagnitude(mAxisVelocityX, maxVelocity.x);
                mAxisVelocityY = Vector3.ClampMagnitude(mAxisVelocityY, maxVelocity.y);
                mAxisVelocityZ = Vector3.ClampMagnitude(mAxisVelocityZ, maxVelocity.z);
                mRigidbodyReference.angularDrag = 0.0f;
            }
            else
            {
                var constrained = mGrabController.LeftHand.ConstraintsEnabled && (double)mGrabController.LeftHand.GrabMassRatio > 0.0 || mGrabController.RightHand.ConstraintsEnabled && (double)mGrabController.RightHand.GrabMassRatio > 0.0;
                mRigidbodyReference.angularDrag = constrained ? 0.0f : m_FreeFloatAngularDrag;
            }

            float dragX = (float)(1.0 + (double)mCurrentAxisDrag.x * (double)deltaTime);
            float dragY = (float)(1.0 + (double)mCurrentAxisDrag.y * (double)deltaTime);
            mAxisVelocityX /= dragX;
            mAxisVelocityY /= dragY;

            if ((double)vec.z == 0.0 || (double)Vector3.Dot(incrementVelocityZ, mAxisVelocityZ) < 0.0)
            {
                mAxisVelocityZ /= (float)(1.0 + (double)mCurrentAxisDrag.z * (double)deltaTime);
            }

            // This is now being called twice per update, this might get us in trouble...
            Reflection.CallPrivateMethod(orientationController, "HandleRotationAudio", mAxisVelocityX, mAxisVelocityY, mAxisVelocityZ);

            // Write back new rotations
            Reflection.SetPrivateField(orientationController, "mAxisVelocityX", mAxisVelocityX);
            Reflection.SetPrivateField(orientationController, "mAxisVelocityY", mAxisVelocityY);
            Reflection.SetPrivateField(orientationController, "mAxisVelocityZ", mAxisVelocityZ);
        }
    }
}



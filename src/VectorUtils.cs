
using UnityEngine;

namespace RoboPhredDev.Shipbreaker.SixAxis
{
    static class VectorUtils
    {
        public static Vector3 Average(Vector3 a, Vector3 b)
        {
            return new Vector3
            {
                x = (a.x + b.x) / 2.0f,
                y = (a.y + b.y) / 2.0f,
                z = (a.z + b.z) / 2.0f
            };
        }
    }
}
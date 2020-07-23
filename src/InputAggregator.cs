using System;
using UnityEngine;

namespace RoboPhredDev.Shipbreaker.SixAxis
{

    class InputAggregator : MonoBehaviour
    {
        private static InputAggregator _instance;

        public static InputAggregator Instance
        {
            get
            {
                return _instance;
            }
        }

        public static void Initialize()
        {
            if (_instance == null)
            {
                _instance = new GameObject("SixAxisInputAggregator").AddComponent<InputAggregator>();
                DontDestroyOnLoad(_instance.gameObject);
            }
        }

        private static DateTime lastLog = DateTime.UtcNow;

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public int RX { get; set; }
        public int RY { get; set; }
        public int RZ { get; set; }

        private void Update()
        {
            // Originally wrote this thinking I would need to clear the inputs every frame, but WM_INPUT
            // is only called on change, not repeatedly.
            // Keeping this here for logging for now.

            if (lastLog > DateTime.UtcNow)
            {
                return;
            }
            lastLog = DateTime.UtcNow + TimeSpan.FromSeconds(.25);
            Logging.Log($"Input: {this.X} {this.Y} {this.Z} | {this.RX} {this.RY} {this.RZ}");
        }
    }
}
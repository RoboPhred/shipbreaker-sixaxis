using System;
using RoboPhredDev.Shipbreaker.SixAxis.RawInput;
using UnityEngine;

namespace RoboPhredDev.Shipbreaker.SixAxis
{
    static class RawInputReceiver
    {
        public static void Initialize()
        {
            WindowMessageInterceptor.OnMessage += HandleMessage;
        }

        private static void HandleMessage(object sender, WindowMessageEventArgs e)
        {
            // Handle WM_INPUT
            if (e.Message != 0x00FF)
            {
                return;
            }

            try
            {
                var data = RawInputData.FromHandle(e.LParam);

                if (data.Device.UsagePage == (uint)UsagePage.GenericDesktop && data.Device.Usage == (uint)GenericDesktopUsage.Mouse)
                {
                    // Game already registers and uses Mouse, so pass that back.
                    return;
                }

                // Mark the message as handled and return 0 for consumed.
                e.Result = IntPtr.Zero;

                ProcessInput(data);
            }
            catch (Exception ex)
            {
                Logging.Log($"Failed to process window message: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private static void ProcessInput(RawInputData data)
        {
            if (!(data is RawInputHidData hidData))
            {
                return;
            }

            InputManager.HandleInput(hidData);
        }
    }
}
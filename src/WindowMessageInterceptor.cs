using System;
using RoboPhredDev.Shipbreaker.SixAxis.Native.Window;

namespace RoboPhredDev.Shipbreaker.SixAxis
{

    class WindowMessageEventArgs : EventArgs
    {
        public IntPtr? Result { get; set; }

        public uint Message { get; }
        public IntPtr WParam { get; private set; }
        public IntPtr LParam { get; private set; }

        public WindowMessageEventArgs(uint message, IntPtr wParam, IntPtr lParam)
        {
            this.Message = message;
            this.WParam = wParam;
            this.LParam = lParam;
        }
    }

    static class WindowMessageInterceptor
    {

        private static IntPtr sWindowHandle;
        private static WndProcDelegate sOriginalHandler;

        public static event EventHandler<WindowMessageEventArgs> OnMessage;

        public static void Enable(IntPtr windowHandle)
        {
            if (sWindowHandle == windowHandle)
            {
                return;
            }

            Logging.Log("Enabling window message interceptor.");

            sWindowHandle = windowHandle;
            sOriginalHandler = WindowInterop.SetWindowProc(windowHandle, CustomWndProc);
        }

        public static void Disable()
        {
            WindowInterop.SetWindowProc(sWindowHandle, sOriginalHandler);
            sWindowHandle = IntPtr.Zero;
        }

        private static IntPtr CustomWndProc(IntPtr hWnd, uint msgAsInt, IntPtr wParam, IntPtr lParam)
        {

            if (OnMessage != null)
            {
                var args = new WindowMessageEventArgs(msgAsInt, wParam, lParam);
                OnMessage(null, args);
                if (args.Result.HasValue)
                {
                    return args.Result.Value;
                }
            }

            return sOriginalHandler(hWnd, msgAsInt, wParam, lParam);
        }
    }
}

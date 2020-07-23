using System;
using Linearstar.Windows.RawInput;
using RoboPhredDev.Shipbreaker.SixAxis.Native;

namespace RoboPhredDev.Shipbreaker.SixAxis
{

    class WmInputEventArgs : EventArgs
    {
        public bool Handled { get; set; }

        public RawInputData Data { get; private set; }

        public WmInputEventArgs(RawInputData data)
        {
            this.Data = data;
        }
    }

    static class WmInputInterceptor
    {

        private static IntPtr windowHandle;
        private static WndProcDelegate originalHandler;

        public static event EventHandler<WmInputEventArgs> OnInput;

        public static IntPtr WindowHandle
        {
            get
            {
                return windowHandle;
            }
        }

        public static void Initialize()
        {
            windowHandle = User32.GetActiveWindow();
            originalHandler = User32.SetWindowProc(windowHandle, CustomWndProc);
        }

        private static IntPtr CustomWndProc(IntPtr hWnd, uint msgAsInt, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                switch (msgAsInt)
                {
                    case 0x00FF: // WM_INPUT
                        {
                            if (OnWmInput(wParam, lParam))
                            {
                                return (IntPtr)0;
                            }
                            break;
                        }
                    default:
                        break;
                }
                return originalHandler(hWnd, msgAsInt, wParam, lParam);
            }
            catch (Exception ex)
            {
                Logging.Log("Error handling message: {0}\n{1}\n{2}", ex.GetType().Name, ex.Message, ex.StackTrace);
                return IntPtr.Zero;
            }
        }

        private static bool OnWmInput(IntPtr wParam, IntPtr lParam)
        {
            if (OnInput != null)
            {
                var data = RawInputData.FromHandle(lParam);
                var args = new WmInputEventArgs(data);
                OnInput(null, args);
                return args.Handled;
            }

            return false;
        }

    }
}

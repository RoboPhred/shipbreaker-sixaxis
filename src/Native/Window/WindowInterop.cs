using System;
using System.Runtime.InteropServices;

namespace RoboPhredDev.Shipbreaker.SixAxis.Native.Window
{
    static class WindowInterop
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", EntryPoint = "SetWindowLongW")]
        public static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW")]
        public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);



        public static WndProcDelegate SetWindowProc(IntPtr windowHandle, WndProcDelegate wndProc)
        {
            var handlePtr = Marshal.GetFunctionPointerForDelegate(wndProc);
            IntPtr oldHandler;
            if (IntPtr.Size == 4)
                oldHandler = SetWindowLongPtr32(windowHandle, -4, handlePtr);
            else
                oldHandler = SetWindowLongPtr64(windowHandle, -4, handlePtr);
            return (WndProcDelegate)Marshal.GetDelegateForFunctionPointer(oldHandler, typeof(WndProcDelegate));
        }
    }
}

using System;
using System.Runtime.InteropServices;

namespace RoboPhredDev.Shipbreaker.SixAxis.Native
{
    public class Win32ErrorException : Exception
    {
        public int ErrorCode { get; }

        public Win32ErrorException(int win32ErrorCode, string message)
            : base(message)
        {
            this.ErrorCode = win32ErrorCode;
        }

        public static Win32ErrorException FromLastWin32Error()
        {
            var code = Marshal.GetLastWin32Error();
            var message = MessageFormatter.FormatMessage(code);
            return new Win32ErrorException(code, message);
        }
    }
}
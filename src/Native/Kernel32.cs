
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace RoboPhredDev.Shipbreaker.SixAxis.Native
{
    static class Kernel32
    {
        public const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;

        [DllImport("kernel32", SetLastError = true)]
        static extern uint FormatMessage(uint dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId, StringBuilder lpBuffer, int nSize, IntPtr Arguments);

        public static string FormatMessage(int errorCode)
        {
            var message = new StringBuilder(255);
            var charsWritten = FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM, IntPtr.Zero, (uint)errorCode, 0, message, message.Capacity, IntPtr.Zero);
            if (charsWritten == 0)
            {
                return $"Failed to format error message from code {errorCode}.";
            }

            return message.ToString();
        }
    }
}
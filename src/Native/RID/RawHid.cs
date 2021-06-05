
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace RoboPhredDev.Shipbreaker.SixAxis.Native.RID
{
    struct RawHid
    {
        public int dwSizeHid;
        public int dwCount;
        public byte[] rawData;

        public ArraySegment<byte>[] ToHidReports()
        {
            var elementSize = dwSizeHid;
            var rawDataArray = rawData;

            return Enumerable.Range(0, dwCount)
                             .Select(x => new ArraySegment<byte>(rawDataArray, elementSize * x, elementSize))
                             .ToArray();
        }

        public static unsafe RawHid FromPointer(void* ptr)
        {
            var result = new RawHid();
            var intPtr = (int*)ptr;

            result.dwSizeHid = intPtr[0];
            result.dwCount = intPtr[1];
            result.rawData = new byte[result.dwSizeHid * result.dwCount];
            Marshal.Copy(new IntPtr(&intPtr[2]), result.rawData, 0, result.rawData.Length);

            return result;
        }
    }
}
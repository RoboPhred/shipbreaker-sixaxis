
using System;
using System.Runtime.InteropServices;

namespace RoboPhredDev.Shipbreaker.SixAxis.Native.HID
{
    static class HidInterop
    {
        [DllImport("hid")]
        static extern NtStatus HidP_GetCaps(IntPtr preparsedData, out HidPCaps capabilities);

        [DllImport("hid")]
        static extern NtStatus HidP_GetValueCaps(HidPReportType reportType, [Out] HidPValueCaps[] valueCaps, ref ushort valueCapsLength, IntPtr preparsedData);

        [DllImport("hid")]
        static extern NtStatus HidP_GetScaledUsageValue(HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, out int usageValue, IntPtr preparsedData, byte[] report, uint reportLength);

        public static HidPCaps GetCaps(IntPtr preparsedData)
        {
            var result = HidP_GetCaps(preparsedData, out var caps);
            if (result != NtStatus.Success)
            {
                throw new InvalidOperationException(result.ToString());
            }
            return caps;
        }

        public static HidPValueCaps[] GetValueCaps(HidPReportType reportType, IntPtr preparsedData)
        {
            var caps = GetCaps(preparsedData);
            var count = reportType switch
            {
                HidPReportType.HidP_Input => caps.NumberInputValueCaps,
                HidPReportType.HidP_Output => caps.NumberOutputValueCaps,
                HidPReportType.HidP_Feature => caps.NumberFeatureValueCaps,
                _ => throw new ArgumentException("Unknown reportType", nameof(reportType))
            };

            var valueCaps = new HidPValueCaps[count];
            var result = HidP_GetValueCaps(reportType, valueCaps, ref count, preparsedData);
            if (result != NtStatus.Success)
            {
                throw new InvalidOperationException(result.ToString());
            }

            if (valueCaps.Length > count)
            {
                var newCaps = new HidPValueCaps[count];
                Array.Copy(valueCaps, newCaps, count);
                valueCaps = newCaps;
            }

            return valueCaps;
        }

        public static int? GetScaledUsageValue(HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, IntPtr preparsedData, byte[] report, uint reportLength)
        {
            var result = HidP_GetScaledUsageValue(reportType, usagePage, linkCollection, usage, out int value, preparsedData, report, reportLength);
            // FIXME: Allowing incompatible report ids through for now, until we figure out how to detect what the id of a report is.
            if (result == NtStatus.Null || result == NtStatus.IncompatibleReportId)
            {
                return null;
            }
            else if (result != NtStatus.Success)
            {
                throw new InvalidOperationException(result.ToString());
            }

            return value;
        }
    }
}

using System;
using System.Runtime.InteropServices;

namespace RoboPhredDev.Shipbreaker.SixAxis.Native.HID
{
    static class HidInterop
    {
        [DllImport("hid")]
        private static extern NtStatus HidP_GetCaps(IntPtr preparsedData, out HidPCaps capabilities);

        [DllImport("hid")]
        private static extern NtStatus HidP_GetValueCaps(HidPReportType reportType, [Out] HidPValueCaps[] valueCaps, ref ushort valueCapsLength, IntPtr preparsedData);

        [DllImport("hid")]
        private static extern NtStatus HidP_GetButtonCaps(HidPReportType reportType, [Out] HidPButtonCaps[] buttonCaps, ref ushort buttonCapsLength, IntPtr preparsedData);

        [DllImport("hid")]
        private static extern NtStatus HidP_GetUsageValue(HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, out uint usageValue, IntPtr preparsedData, byte[] report, uint reportLength);

        [DllImport("hid")]
        private static extern NtStatus HidP_GetScaledUsageValue(HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, out uint usageValue, IntPtr preparsedData, byte[] report, uint reportLength);

        [DllImport("hid")]
        private static extern NtStatus HidP_GetUsages(HidPReportType reportType, ushort usagePage, ushort linkCollection, [Out] ushort[] usageList, ref uint usageLength, IntPtr preparsedData, byte[] report, uint reportLength);


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

        public static HidPButtonCaps[] GetButtonCaps(HidPReportType reportType, IntPtr preparsedData)
        {
            var caps = GetCaps(preparsedData);
            var count = reportType switch
            {
                HidPReportType.HidP_Input => caps.NumberInputValueCaps,
                HidPReportType.HidP_Output => caps.NumberOutputValueCaps,
                HidPReportType.HidP_Feature => caps.NumberFeatureValueCaps,
                _ => throw new ArgumentException("Unknown reportType", nameof(reportType))
            };

            if (count == 0)
            {
                return new HidPButtonCaps[0];
            }
            var prevCount = count;

            var buttonCaps = new HidPButtonCaps[count];
            var result = HidP_GetButtonCaps(reportType, buttonCaps, ref count, preparsedData);
            if (result == NtStatus.UsageNotFound)
            {
                // FIXME: What device is causing this?
                // Logging.Log($"WARNING: Got UsageNotFound when querying {reportType} {prevCount} buttons from device.  Assuming no buttons.");
                return new HidPButtonCaps[0];
            }

            if (result != NtStatus.Success)
            {
                throw new InvalidOperationException(result.ToString());
            }

            if (buttonCaps.Length > count)
            {
                var newCaps = new HidPButtonCaps[count];
                Array.Copy(buttonCaps, newCaps, count);
                buttonCaps = newCaps;
            }

            return buttonCaps;
        }

        public static int? GetUsageValue(HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, IntPtr preparsedData, byte[] report, uint reportLength)
        {
            var result = HidP_GetUsageValue(reportType, usagePage, linkCollection, usage, out uint value, preparsedData, report, reportLength);
            // FIXME: Allowing incompatible report ids through for now, until we figure out how to detect what the id of a report is.
            if (result == NtStatus.Null || result == NtStatus.IncompatibleReportId)
            {
                return null;
            }
            else if (result != NtStatus.Success)
            {
                throw new InvalidOperationException($"{result} while getting usage value for {usagePage} {linkCollection} {usage}");
            }

            unchecked
            {
                return (short)(ushort)value;
            }
        }

        public static int? GetScaledUsageValue(HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, IntPtr preparsedData, byte[] report, uint reportLength)
        {
            var result = HidP_GetScaledUsageValue(reportType, usagePage, linkCollection, usage, out uint value, preparsedData, report, reportLength);
            // FIXME: Allowing incompatible report ids through for now, until we figure out how to detect what the id of a report is.
            if (result == NtStatus.Null || result == NtStatus.IncompatibleReportId)
            {
                return null;
            }
            else if (result != NtStatus.Success)
            {
                throw new InvalidOperationException($"{result} while getting scaled usage value for {usagePage} {linkCollection} {usage}");
            }

            unchecked
            {
                return (short)(ushort)value;
            }
        }

        public static ushort[] GetUsages(HidPReportType reportType, ushort usagePage, ushort linkCollection, IntPtr preparsedData, byte[] report, uint reportLength)
        {
            uint usageLength = 0;
            var result = HidP_GetUsages(reportType, usagePage, linkCollection, null, ref usageLength, preparsedData, report, reportLength);
            if (result == NtStatus.IncompatibleReportId)
            {
                return null;
            }

            // Expect BufferTooSmall when we pass a zero length.  Length will get reset to the number of usages available.
            if (result != NtStatus.Success && result != NtStatus.BufferTooSmall)
            {
                throw new InvalidOperationException(result.ToString());
            }

            if (usageLength == 0)
            {
                return new ushort[0];
            }

            var usageList = new ushort[usageLength];
            result = HidP_GetUsages(reportType, usagePage, linkCollection, usageList, ref usageLength, preparsedData, report, reportLength);

            if (result != NtStatus.Success)
            {
                throw new InvalidOperationException(result.ToString());
            }

            return usageList;
        }
    }
}
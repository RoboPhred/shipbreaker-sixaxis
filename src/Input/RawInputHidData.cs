using System;
using System.Collections.Generic;
using System.Linq;
using RoboPhredDev.Shipbreaker.SixAxis.Native.HID;
using RoboPhredDev.Shipbreaker.SixAxis.Native.RID;

namespace RoboPhredDev.Shipbreaker.SixAxis.Input
{
    class RawInputHidData : RawInputData
    {
        private Dictionary<ushort, RawInputHidValue> inputValues = new Dictionary<ushort, RawInputHidValue>();

        public unsafe RawInputHidData(IntPtr headerHandle)
        {
            var header = RawInputInterop.GetRawInputHeader(headerHandle);
            if (header.dwType != (uint)DeviceType.RIM_TYPEHID)
            {
                throw new ArgumentException("Expected a handle to a HID device header.");
            }

            Device = RawInputDevice.FromHandle(header.hDevice);

            var preparsedData = RawInputInterop.GetRawInputDevicePreparsedData(header.hDevice);
            var reports = RawInputInterop.GetRawInputHidData(headerHandle).ToHidReports();

            HidPValueCaps[] valueCaps;
            fixed (byte* preparsedDataPtr = preparsedData)
            {
                valueCaps = HidInterop.GetValueCaps(HidPReportType.HidP_Input, (IntPtr)preparsedDataPtr);

                foreach (var cap in valueCaps)
                {
                    // FIXME: There must be a way to read the report id of the reports we have...
                    // Without that, we just have to try every cap against every report.
                    // This is really, really stupid.
                    foreach (var reportSeg in reports)
                    {
                        var report = reportSeg.ToArray();

                        ushort usageMin;
                        ushort usageMax;
                        if (cap.IsRange)
                        {
                            usageMin = cap.Range.UsageMin;
                            usageMax = cap.Range.UsageMax;
                        }
                        else
                        {
                            usageMax = usageMin = cap.NotRange.Usage;
                        }

                        for (var usage = usageMin; usage <= usageMax; usage++)
                        {
                            var value = HidInterop.GetScaledUsageValue(HidPReportType.HidP_Input, cap.UsagePage, cap.LinkCollection, usage, (IntPtr)preparsedDataPtr, report, (uint)report.Length);
                            if (value.HasValue)
                            {
                                if (value.Value < cap.PhysicalMin || value.Value > cap.PhysicalMax)
                                {
                                    continue;
                                }

                                var valueThing = new RawInputHidValue(usage, cap.UsagePage, value.Value, cap);
                                inputValues.Add(usage, valueThing);
                            }
                        }
                    }
                }
            }
        }

        public override RawInputDevice Device { get; }

        public RawInputHidValue GetInputValue(ushort usage)
        {
            if (inputValues.TryGetValue(usage, out var value))
            {
                return value;
            }
            return null;
        }
    }
}
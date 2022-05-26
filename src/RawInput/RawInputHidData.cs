using System;
using System.Collections.Generic;
using System.Linq;
using RoboPhredDev.Shipbreaker.SixAxis.Native.HID;
using RoboPhredDev.Shipbreaker.SixAxis.Native.RID;

namespace RoboPhredDev.Shipbreaker.SixAxis.RawInput
{
    class RawInputHidData : RawInputData
    {
        private readonly Dictionary<PageAndUsage, RawInputHidValue> inputValues = new();

        private readonly Dictionary<PageAndUsage, bool> buttonStatus = new();

        public unsafe RawInputHidData(IntPtr headerHandle)
        {
            var header = RawInputInterop.GetRawInputHeader(headerHandle);
            if (header.dwType != (uint)DeviceType.RIM_TYPEHID)
            {
                throw new ArgumentException("Expected a handle to a HID device header.");
            }

            Device = RawInputDevice.FromHandle(header.hDevice);

            var preparsedData = RawInputInterop.GetRawInputDevicePreparsedData(header.hDevice);
            var reports = RawInputInterop.GetRawInputHidData(headerHandle).ToHidReports().Select(report => report.ToArray()).ToArray();

            fixed (byte* preparsedDataPtr = preparsedData)
            {
                GetValues((IntPtr)preparsedDataPtr, reports);
                GetButtons((IntPtr)preparsedDataPtr, reports);
            }
        }

        private unsafe void GetValues(IntPtr preparsedDataPtr, byte[][] reports)
        {
            var valueCaps = HidInterop.GetValueCaps(HidPReportType.HidP_Input, preparsedDataPtr);

            foreach (var cap in valueCaps)
            {
                // FIXME: There must be a way to read the report id of the reports we have...
                // Without that, we just have to try every cap against every report.
                // This is really, really stupid.
                for (var i = 0; i < reports.Length; i++)
                {
                    var report = reports[i];

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
                        try
                        {
                            RawInputHidValue hidValue = null;

                            if (cap.IsAbsolute)
                            {
                                var value = HidInterop.GetUsageValue(HidPReportType.HidP_Input, cap.UsagePage, cap.LinkCollection, usage, preparsedDataPtr, report, (uint)report.Length);
                                if (value.HasValue)
                                {
                                    hidValue = new RawInputHidValue(cap, usage, value.Value, true);
                                }
                            }
                            else
                            {
                                var value = HidInterop.GetScaledUsageValue(HidPReportType.HidP_Input, cap.UsagePage, cap.LinkCollection, usage, preparsedDataPtr, report, (uint)report.Length);
                                if (value.HasValue)
                                {
                                    // We are getting lots of -1s from the legacy spacemouse pro when no input is available.
                                    // Is this normal?  Should we be filtering this out?

                                    hidValue = new RawInputHidValue(cap, usage, value.Value, false);
                                }
                            }

                            if (hidValue != null)
                            {
                                inputValues.Add(new PageAndUsage(cap.UsagePage, usage), hidValue);
                            }
                        }
                        catch (InvalidOperationException ex)
                        {
                            Logging.Log($"While reading from device ${this.Device.DeviceName} report {i} page {cap.UsagePage} collection {cap.LinkCollection} usage {usage}: {ex.Message}");
                        }
                    }
                }
            }
        }

        private unsafe void GetButtons(IntPtr preparsedDataPtr, byte[][] reports)
        {
            var buttonCaps = HidInterop.GetButtonCaps(HidPReportType.HidP_Input, preparsedDataPtr);

            // We can grab all usages per page and link collection, so build up a list of what we need.
            var pagesAndCollections = (from cap in buttonCaps
                                       select new UsagePageAndCollection(cap.UsagePage, cap.LinkCollection)).Distinct().ToArray();

            foreach (var pair in pagesAndCollections)
            {
                foreach (var report in reports)
                {
                    var usages = HidInterop.GetUsages(HidPReportType.HidP_Input, pair.UsagePage, pair.LinkCollection, preparsedDataPtr, report, (uint)report.Length);
                    if (usages == null)
                    {
                        continue;
                    }

                    foreach (var cap in buttonCaps)
                    {
                        if (cap.UsagePage != pair.UsagePage || cap.LinkCollection != pair.LinkCollection)
                        {
                            continue;
                        }

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
                            buttonStatus.Add(new PageAndUsage(pair.UsagePage, usage), usages.Contains(usage));
                        }
                    }
                }
            }
        }

        public override RawInputDevice Device { get; }

        public IEnumerable<KeyValuePair<PageAndUsage, RawInputHidValue>> GetAllValues()
        {
            return inputValues;
        }

        public IEnumerable<KeyValuePair<PageAndUsage, bool>> GetAllButtons()
        {
            return buttonStatus;
        }

        public RawInputHidValue GetInputValue(PageAndUsage usageAndPage)
        {
            if (inputValues.TryGetValue(usageAndPage, out var value))
            {
                return value;
            }
            return null;
        }

        public bool? GetButtonStatus(PageAndUsage usageAndPage)
        {
            if (buttonStatus.TryGetValue(usageAndPage, out var value))
            {
                return value;
            }
            return null;
        }
    }
}
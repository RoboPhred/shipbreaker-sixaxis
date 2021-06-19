
using System;

namespace RoboPhredDev.Shipbreaker.SixAxis.RawInput
{
    struct UsagePageAndCollection : IEquatable<PageAndUsage>
    {
        public UsagePageAndCollection(ushort usagePage, ushort linkCollection)
        {
            UsagePage = usagePage;
            LinkCollection = linkCollection;
        }

        public ushort UsagePage { get; }
        public ushort LinkCollection { get; }

        public bool Equals(PageAndUsage other)
        {
            return UsagePage == other.UsagePage && LinkCollection == other.Usage;
        }
    }
}
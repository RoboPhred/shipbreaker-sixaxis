
using System;
using System.Globalization;

namespace RoboPhredDev.Shipbreaker.SixAxis.RawInput
{
    struct UsageAndPage : IEquatable<UsageAndPage>
    {
        public UsageAndPage(ushort usagePage, ushort usage)
        {
            UsagePage = usagePage;
            Usage = usage;
        }

        public ushort UsagePage { get; }
        public ushort Usage { get; }

        public bool Equals(UsageAndPage other)
        {
            return UsagePage == other.UsagePage && Usage == other.Usage;
        }

        public override string ToString()
        {
            return $"{UsagePage}:{Usage}";
        }

        public static UsageAndPage Parse(string str)
        {
            var parts = str.Split(':');
            if (parts.Length != 2)
            {
                throw new FormatException();
            }
            return new UsageAndPage(ushort.Parse(parts[0], NumberStyles.HexNumber | NumberStyles.AllowHexSpecifier), ushort.Parse(parts[1], NumberStyles.HexNumber | NumberStyles.AllowHexSpecifier));
        }
    }
}
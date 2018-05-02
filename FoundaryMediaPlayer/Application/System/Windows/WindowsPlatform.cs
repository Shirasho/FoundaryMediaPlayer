using System;
using Foundary;

namespace FoundaryMediaPlayer.Application.Windows
{
    public sealed class FWindowsPlatform : APlatform
    {
        /// <inheritdoc />
        public override FOperatingSystemInfo OperatingSystem { get; } = CreateOSObject();

        private static FOperatingSystemInfo CreateOSObject()
        {
            var osObj = Environment.OSVersion;
            var osVer = osObj.Version;

            return new FOperatingSystemInfo
            {
                Name = GetOSName(osObj),
                Version = new Version(osVer.Major, osVer.Minor, osVer.Build)
            };
        }

        private static string GetOSName(OperatingSystem os)
        {
            switch (os.Platform)
            {
                case PlatformID.Win32Windows:
                    switch (os.Version.Minor)
                    {
                        case 0: return "Windows 95";
                        case 10:
                            switch (os.Version.Revision.ToString())
                            {
                                case "2222A": return "Windows 98 Second Edition";
                                default: return "Windows 98";
                            }
                        case 90: return "Windows Me";
                    }
                    break;
                case PlatformID.Win32NT:
                    switch (os.Version.Major)
                    {
                        case 3: return "Windows NT 3.51";
                        case 4: return "Windows NT 4.0";
                        case 5:
                            switch (os.Version.Minor)
                            {
                                case 0: return "Windows 2000";
                                case 1: return "Windows XP";
                                case 2: return "Windows 2003";
                            }
                            break;
                        case 6:
                            switch (os.Version.Minor)
                            {
                                case 0: return "Windows Vista";
                                case 1: return "Windows 7";
                                case 2: return "Windows 8";
                                case 3: return "Windows 8.1";
                            }
                            break;
                        case 10: return "Windows 10";
                    }
                    break;
            }

            throw new RuntimeException("Unknown or unsupported Windows OS version.");
        }
    }
}

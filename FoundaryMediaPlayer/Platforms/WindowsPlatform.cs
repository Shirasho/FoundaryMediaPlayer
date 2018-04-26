using System;
using System.ComponentModel;
using System.Diagnostics;
using Foundary;
using FoundaryMediaPlayer.Configuration;
using FoundaryMediaPlayer.Platforms.Windows;
using log4net.Core;
using log4net.Layout;
using Win32ErrorCode = PInvoke.Win32ErrorCode;
using Kernel32 = PInvoke.Kernel32;

namespace FoundaryMediaPlayer.Platforms
{
    /// <summary>
    /// Windows platform.
    /// </summary>
    public sealed class WindowsPlatform : Platform
    {
        /// <inheritdoc />
        public override OperatingSystemInfo OperatingSystem { get; } = CreateOSObject();

        private Kernel32.SafeObjectHandle _ConsoleOutputStream { get; set; }
        private Kernel32.SafeObjectHandle _ConsoleInputStream { get; set; }

        private bool _bConsoleAllocated { get; set; }

        /// <inheritdoc />
        public override object InitializeConsole(bool bForceInitializeConsole, CLOptions options)
        {
            if (bForceInitializeConsole ||
                (Kernel32.AttachConsole(Process.GetCurrentProcess().Id) || Kernel32.AttachConsole(-1))
                && Kernel32.GetLastError() != Win32ErrorCode.ERROR_ACCESS_DENIED)
            {
                _bConsoleAllocated = Kernel32.AllocConsole();
            }

            // Visual Studio changes the STD output handle to the Visual Studio application
            // when a debugger is attached. If we opt to use a console window and the window
            // was successfully allocated we need to redirect the output back to the standard
            // STD handle.
            if (_bConsoleAllocated && Debugger.IsAttached)
            {
                _ConsoleOutputStream = InitializeOutStream();
                _ConsoleInputStream = InitializeInStream();

                Kernel32.SetStdHandle(Kernel32.StdHandle.STD_OUTPUT_HANDLE, _ConsoleOutputStream.DangerousGetHandle());
                Kernel32.SetStdHandle(Kernel32.StdHandle.STD_INPUT_HANDLE, _ConsoleInputStream.DangerousGetHandle());
            }

            return null;
        }

        /// <inheritdoc />
        public override bool DisposeConsole(object console)
        {
            _ConsoleOutputStream?.Dispose();
            _ConsoleInputStream?.Dispose();

            return !_bConsoleAllocated || Kernel32.FreeConsole();
        }

        /// <inheritdoc />
        public override void ConfigureConsoleAppender(object console)
        {
            var appender = new ConsoleAppender
            {
                Threshold = Level.All,
                Layout = new PatternLayout("[%date][%-5level][%thread]%ndc - %message%newline")
            };
            AddAppender(appender);
        }

        private static Kernel32.SafeObjectHandle InitializeOutStream()
        {
            var handle = Kernel32.CreateFile(
                "CONOUT$",
                new Kernel32.ACCESS_MASK((uint)Kernel32.ACCESS_MASK.GenericRight.GENERIC_WRITE),
                Kernel32.FileShare.FILE_SHARE_WRITE,
                IntPtr.Zero,
                Kernel32.CreationDisposition.OPEN_EXISTING,
                Kernel32.CreateFileFlags.FILE_ATTRIBUTE_NORMAL,
                Kernel32.SafeObjectHandle.Null
            );

            if (handle.IsInvalid)
                throw new Win32Exception();
            return handle;
        }

        private static Kernel32.SafeObjectHandle InitializeInStream()
        {
            var handle = Kernel32.CreateFile(
                "CONIN$",
                new Kernel32.ACCESS_MASK((uint)Kernel32.ACCESS_MASK.GenericRight.GENERIC_READ),
                Kernel32.FileShare.FILE_SHARE_READ,
                IntPtr.Zero,
                Kernel32.CreationDisposition.OPEN_EXISTING,
                Kernel32.CreateFileFlags.FILE_ATTRIBUTE_NORMAL,
                Kernel32.SafeObjectHandle.Null
            );
            if (handle.IsInvalid)
                throw new Win32Exception();
            return handle;
        }

        private static OperatingSystemInfo CreateOSObject()
        {
            var osObj = Environment.OSVersion;
            var osVer = osObj.Version;

            return new OperatingSystemInfo
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

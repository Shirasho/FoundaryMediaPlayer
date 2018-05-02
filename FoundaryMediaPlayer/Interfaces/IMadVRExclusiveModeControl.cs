using System;
using System.Runtime.InteropServices;
using System.Security;

namespace FoundaryMediaPlayer.Interfaces
{
    [ComImport]
    [SuppressUnmanagedCodeSecurity]
    [Guid("88A69329-3CD3-47D6-ADEF-89FA23AFC7F3")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMadVRExclusiveModeControl
    {
        [PreserveSig]
        int DisableExclusiveMode(
            IntPtr disable
        );
    };
}

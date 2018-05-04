using System;
using System.Runtime.InteropServices;
using System.Security;

namespace FoundaryMediaPlayer.Interfaces
{
    [ComImport, SuppressUnmanagedCodeSecurity]
    [Guid("D2D3A520-7CFA-46EB-BA3B-6194A028781C")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMadVRSeekbarControl
    {
        [PreserveSig]
        int DisableSeekbar(IntPtr disable);
    };
}

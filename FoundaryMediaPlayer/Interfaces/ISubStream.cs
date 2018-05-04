using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using DirectShowLib;

namespace FoundaryMediaPlayer.Interfaces
{
    [ComImport, SuppressUnmanagedCodeSecurity]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("DE11E2FB-02D3-45e4-A174-6B7CE2783BDB")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public interface ISubStream : IPersist
    {
        [MethodImpl(MethodImplOptions.PreserveSig)]
        int GetStreamCount();

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int GetStreamInfo([In] int i, [MarshalAs(UnmanagedType.LPWStr), Out] string ppName, [MarshalAs(UnmanagedType.U8), In] ulong pLCID);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int GetStream();

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int SetStream([In] int iStream);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int Reload();

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int SetSourceTargetInfo([MarshalAs(UnmanagedType.LPWStr), In] string yuvMatrix, [In] int targetBlackLevel, [In] int targetWhiteLevel);
    }
}

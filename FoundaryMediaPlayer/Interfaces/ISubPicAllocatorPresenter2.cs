using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using FoundaryMediaPlayer.Interop;
using FoundaryMediaPlayer.Interop.Windows;

namespace FoundaryMediaPlayer.Interfaces
{
    [ComImport, SuppressUnmanagedCodeSecurity]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("767AEBA8-A084-488a-89C8-F6B74E53A90F")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public interface ISubPicAllocatorPresenter2 : ISubPicAllocatorPresenter
    {
        [MethodImpl(MethodImplOptions.PreserveSig)]
        int SetPixelShader2([MarshalAs(UnmanagedType.LPWStr), In] string pSrcData, [MarshalAs(UnmanagedType.LPWStr), In] string pTarget,
                            bool bScreenSpace);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        SIZE GetVisibleVideoSize();

        [MethodImpl(MethodImplOptions.PreserveSig)]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool IsRendering();

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int SetIsRendering(bool bIsRendering);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int SetDefaultVideoAngle([In] Vector v);
    }
}

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace FoundaryMediaPlayer.Interfaces
{
    [ComImport, SuppressUnmanagedCodeSecurity]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("C8334466-CD1E-4ad1-9D2D-8EE8519BD180")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public interface ISubPicQueue
    {
        [MethodImpl(MethodImplOptions.PreserveSig)]
        int SetSubPicProvider([In] ISubPicProvider pSubPicProvider);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int GetSubPicProvider([Out] out ISubPicProvider pSubPicProvider);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int SetFPS(double fps);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int SetTime(long rtNow);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int Invalidate(long rtInvalidate = -1);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool LookupSubPic(long rtNow, [Out] out ISubPic pSubPic);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int GetStats(ref int nSubPics, ref long rtNow, ref long rtstart, ref long rtStop);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int GetStats(int nSubPic, ref long rtStart, ref long rtStop);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool LookupSubPic(long rtNow, bool bAdviseBlocking, [In] ISubPic pSubPic);
    }
}

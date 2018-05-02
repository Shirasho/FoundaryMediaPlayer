using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace FoundaryMediaPlayer.Interfaces
{
    [SuppressUnmanagedCodeSecurity]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("2D0EBE73-BA82-4E90-859B-C7C48ED3650F")]
    [ComImport]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public interface IDSMChapterBag
    {
        [MethodImpl(MethodImplOptions.PreserveSig)]
        uint ChapGetCount();

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int ChapGet([In] ulong iIndex, [Out] out long prt, [MarshalAs(UnmanagedType.LPWStr), Out] out string ppName);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int ChapSet([In] ulong iIndex, [In] long rt, [MarshalAs(UnmanagedType.LPWStr), In] string pName);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int ChapAppend([In] long rt, [MarshalAs(UnmanagedType.LPWStr), In] string pName);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int ChapRemoveAt([In] ulong iIndex);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int ChapRemoveAll();

        //TODO: Is this right?
        [MethodImpl(MethodImplOptions.PreserveSig)]
        long ChapLookup([In] ref long prt, [MarshalAs(UnmanagedType.LPWStr), Out] out string ppName);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int ChapSort();
    }
}

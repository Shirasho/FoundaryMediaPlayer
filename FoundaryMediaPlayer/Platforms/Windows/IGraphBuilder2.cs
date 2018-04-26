#pragma warning disable 1591

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using DirectShowLib;

namespace FoundaryMediaPlayer.Platforms.Windows
{
    [SuppressUnmanagedCodeSecurity]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("165BE9D6-0929-4363-9BA3-580D735AA0F6")]
    [ComImport]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal interface IGraphBuilder2 : IFilterGraph2
    {
        [MethodImpl(MethodImplOptions.PreserveSig)]
        int IsPinDirection([In] IPin pPin, [In] PinDirection dir);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int IsPinConnected([In] IPin pPin);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int ConnectFilter([In] IBaseFilter pBF, [In] IPin pPinIn);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int ConnectFilter([Out] out IPin pPinOut, [In] IBaseFilter pBF);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int ConnectFilterDirect([Out] out IPin pPinOut, [In] IBaseFilter pBF, [MarshalAs(UnmanagedType.LPStruct), In] AMMediaType pmt);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int NukeDownstream([MarshalAs(UnmanagedType.IUnknown), In] object pUnk);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int FindInterface([MarshalAs(UnmanagedType.LPStruct), In] ref Guid iid, [Out] out IntPtr ppv, [In] bool bRemove);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int AddToROT();

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int RemoveFromROT();
    }
}

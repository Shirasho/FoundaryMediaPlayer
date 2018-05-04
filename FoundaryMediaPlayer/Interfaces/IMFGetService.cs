using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace FoundaryMediaPlayer.Interfaces
{
    [ComImport, SuppressUnmanagedCodeSecurity]
    [Guid("fa993888-4383-415a-a930-dd472a8cf6f7")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public interface IMFGetService
    {
        [MethodImpl(MethodImplOptions.PreserveSig)]
        int GetService([MarshalAs(UnmanagedType.LPStruct), In] Guid guidService, 
                       [MarshalAs(UnmanagedType.LPStruct), In] Guid riid, 
                       [Out] out IntPtr ppvObject);
    }
}

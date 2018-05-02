using System.Runtime.InteropServices;
using System.Security;

namespace FoundaryMediaPlayer.Interfaces
{
    [ComImport]
    [SuppressUnmanagedCodeSecurity]
    [Guid("ABA34FDA-DD22-4E00-9AB4-4ABF927D0B0C")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMadVRTextOsd
    {
        [PreserveSig]
        int OsdDisplayMessage(
            [In, MarshalAs(UnmanagedType.LPWStr)] string text, 
            [In, MarshalAs(UnmanagedType.U4)] uint milliseconds
        );

        [PreserveSig]
        int OsdClearMessage();
    };
}

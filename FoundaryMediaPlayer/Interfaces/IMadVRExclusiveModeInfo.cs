using System.Runtime.InteropServices;
using System.Security;

namespace FoundaryMediaPlayer.Interfaces
{
    [ComImport]
    [SuppressUnmanagedCodeSecurity]
    [Guid("D6EE8031-214E-4E9E-A3A7-458925F933AB")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMadVRExclusiveModeInfo
    {
        [PreserveSig]
        //void IsExclusiveModeActive([Out, MarshalAs(UnmanagedType.I1)] out bool Status);
        int IsExclusiveModeActive();

        [PreserveSig]
        void IsMadVRSeekbarEnabled(
            [Out, MarshalAs(UnmanagedType.I1)] out bool status
        );
    };
}

using System.Runtime.InteropServices;
using System.Security;
using DirectShowLib;

namespace FoundaryMediaPlayer.Interfaces
{
    [ComImport, ComVisible(true), SuppressUnmanagedCodeSecurity]
    [Guid("AB52FC9C-2415-4dca-BC1C-8DCC2EAE8151")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDirectVobSub3 : IBaseFilter
    {
        [PreserveSig]
        int OpenSubtitles([MarshalAs(UnmanagedType.LPWStr)] string fn);

        [PreserveSig]
        int SkipAutoloadCheck([MarshalAs(UnmanagedType.SysInt)] bool bSkip);
    }
}

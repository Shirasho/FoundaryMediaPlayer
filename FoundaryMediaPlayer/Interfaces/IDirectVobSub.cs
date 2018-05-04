using System;
using System.Runtime.InteropServices;
using System.Security;
using ControlzEx.Standard;
using DirectShowLib;

namespace FoundaryMediaPlayer.Interfaces
{
    [ComImport, ComVisible(true), SuppressUnmanagedCodeSecurity]
    [Guid("EBE1FB08-3957-47ca-AF13-5827E5442E56")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDirectVobSub : IBaseFilter
    {
        [PreserveSig]
        int get_FileName(/*[MarshalAs(UnmanagedType.LPWStr, SizeConst = 260)]ref StringBuilder*/ IntPtr fn);

        [PreserveSig]
        int put_FileName([MarshalAs(UnmanagedType.LPWStr)] string fn);

        [PreserveSig]
        int get_LanguageCount(out int nLangs);

        [PreserveSig]
        int get_LanguageName(int iLanguage, [MarshalAs(UnmanagedType.LPWStr)] out string ppName);

        [PreserveSig]
        int get_SelectedLanguage(ref int iSelected);

        [PreserveSig]
        int put_SelectedLanguage(int iSelected);

        [PreserveSig]
        int get_HideSubtitles(ref bool fHideSubtitles);

        [PreserveSig]
        int put_HideSubtitles([MarshalAs(UnmanagedType.I1)] bool fHideSubtitles);

        [PreserveSig]
        int get_PreBuffering(ref bool fDoPreBuffering);

        [PreserveSig]
        int put_PreBuffering([MarshalAs(UnmanagedType.I1)] bool fDoPreBuffering);

        [PreserveSig]
        int get_Placement(ref bool fOverridePlacement, ref int xperc, ref int yperc);

        [PreserveSig]
        int put_Placement([MarshalAs(UnmanagedType.I1)] bool fOverridePlacement, int xperc, int yperc);

        [PreserveSig]
        int get_VobSubSettings(ref bool fBuffer, ref bool fOnlyShowForcedSubs, ref bool fPolygonize);

        [PreserveSig]
        int put_VobSubSettings([MarshalAs(UnmanagedType.I1)] bool fBuffer, [MarshalAs(UnmanagedType.I1)] bool fOnlyShowForcedSubs, 
                               [MarshalAs(UnmanagedType.I1)] bool fPolygonize);

        [PreserveSig]
#pragma warning disable 618
        int get_TextSettings(LOGFONT lf, int lflen, ref uint color, ref bool fShadow, ref bool fOutline, ref bool fAdvancedRenderer);
#pragma warning restore 618

        [PreserveSig]
#pragma warning disable 618
        int put_TextSettings(LOGFONT lf, int lflen, uint color, bool fShadow, bool fOutline, bool fAdvancedRenderer);
#pragma warning restore 618

        [PreserveSig]
        int get_Flip(ref bool fPicture, ref bool fSubtitles);

        [PreserveSig]
        int put_Flip([MarshalAs(UnmanagedType.I1)] bool fPicture, [MarshalAs(UnmanagedType.I1)] bool fSubtitles);

        [PreserveSig]
        int get_OSD(ref bool fOSD);

        [PreserveSig]
        int put_OSD([MarshalAs(UnmanagedType.I1)] bool fOSD);
    }
}

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace FoundaryMediaPlayer.Interfaces
{
    [ComVisible(true)]
    [ComImport]
    [SuppressUnmanagedCodeSecurity]
    [Guid("774A919D-EA95-4A87-8A1E-F48ABE8499C7")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ILAVSplitterSettings
    {
        [PreserveSig]
        int SetRuntimeConfig(
            bool runtime
        );
        
        [PreserveSig]
        int GetPreferredLanguages(
            [MarshalAs(UnmanagedType.LPWStr)] out string langs
        );
        
        [PreserveSig]
        int SetPreferredLanguages(
            [MarshalAs(UnmanagedType.LPWStr)] string langs
        );
        
        [PreserveSig]
        int GetPreferredSubtitleLanguages(
            [MarshalAs(UnmanagedType.LPWStr)] out string langs
        );
        
        [PreserveSig]
        int SetPreferredSubtitleLanguages(
            [MarshalAs(UnmanagedType.LPWStr)]string langs
        );
        
        [PreserveSig]
        ELAVSubtitleMode GetSubtitleMode();
        
        [PreserveSig]
        int SetSubtitleMode(
            ELAVSubtitleMode mode
        );
        
        [PreserveSig]
        [Obsolete("Use advanced subtitle mode instead.")]
        bool GetSubtitleMatchingLanguage();
        
        [PreserveSig]
        [Obsolete("Use advanced subtitle mode instead.")]
        int SetSubtitleMatchingLanguage(
            bool mode
        );
        
        [PreserveSig]
        bool GetPGSForcedStream();
        
        [PreserveSig]
        int SetPGSForcedStream(
            bool enabled
        );
        
        [PreserveSig]
        bool GetPGSOnlyForced();
        
        [PreserveSig]
        int SetPGSOnlyForced(
            bool forced
        );
        
        [PreserveSig]
        int GetVC1TimestampMode();
        
        [PreserveSig]
        int SetVC1TimestampMode(
            short enabled
        );
        
        [PreserveSig]
        int SetSubstreamsEnabled(
            bool enabled
        );
        
        [PreserveSig]
        bool GetSubstreamsEnabled();
        
        [PreserveSig]
        int SetVideoParsingEnabled(
            bool enabled
        );
        
        [PreserveSig]
        bool GetVideoParsingEnabled();
        
        [PreserveSig]
        int SetFixBrokenHDPVR(
            bool enabled
        );
        
        [PreserveSig]
        bool GetFixBrokenHDPVR();
        
        [PreserveSig]
        int SetFormatEnabled(
            [MarshalAs(UnmanagedType.LPStr)] string strFormat, 
            bool bEnabled
        );
        
        [PreserveSig]
        bool IsFormatEnabled(
            [MarshalAs(UnmanagedType.LPStr)] string strFormat
        );
        
        [PreserveSig]
        int SetStreamSwitchRemoveAudio(
            bool enabled
        );
        
        [PreserveSig]
        bool GetStreamSwitchRemoveAudio();
        
        [PreserveSig]
        int GetAdvancedSubtitleConfig(
            [MarshalAs(UnmanagedType.LPWStr)] out string ec
        );
        
        [PreserveSig]
        int SetAdvancedSubtitleConfig(
            [MarshalAs(UnmanagedType.LPWStr)] string config
        );
    }
}

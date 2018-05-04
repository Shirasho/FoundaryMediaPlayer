using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace FoundaryMediaPlayer.Interfaces
{
    [ComImport]
    [Guid("4158A22B-6553-45D0-8069-24716F8FF171")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public interface ILAVAudioSettings
    {
        [PreserveSig]
        int SetRuntimeConfig(bool bRuntimeConfig);
        
        [PreserveSig]
        int GetDRC(out bool pbDRCEnabled, out int piDRCLevel);

        [PreserveSig]
        int SetDRC(bool bDRCEnabled, int iDRCLevel);
        
        [PreserveSig]
        bool GetFormatConfiguration(ELAVAudioCodec aCodec);

        [PreserveSig]
        int SetFormatConfiguration(ELAVAudioCodec aCodec, bool bEnabled);
        
        [PreserveSig]
        bool GetBitstreamConfig(ELAVBitstreamCodec bsCodec);

        [PreserveSig]
        int SetBitstreamConfig(ELAVBitstreamCodec bsCodec, bool bEnabled);
        
        [PreserveSig]
        bool GetDTSHDFraming();

        [PreserveSig]
        int SetDTSHDFraming(bool bHDFraming);
        
        [PreserveSig]
        bool GetAutoAVSync();

        [PreserveSig]
        int SetAutoAVSync(bool bAutoSync);
        
        [PreserveSig]
        bool GetOutputStandardLayout();

        [PreserveSig]
        int SetOutputStandardLayout(bool bStdLayout);
        
        [PreserveSig]
        bool GetExpandMono();

        [PreserveSig]
        int SetExpandMono(bool bExpandMono);
        
        [PreserveSig]
        bool GetExpand61();

        [PreserveSig]
        int SetExpand61(bool bExpand61);
        
        [PreserveSig]
        bool GetAllowRawSPDIFInput();

        [PreserveSig]
        int SetAllowRawSPDIFInput(bool bAllow);
        
        [PreserveSig]
        bool GetSampleFormat(ELAVAudioSampleFormat format);

        [PreserveSig]
        int SetSampleFormat(ELAVAudioSampleFormat format, bool bEnabled);
        
        [PreserveSig]
        int GetAudioDelay(out bool pbEnabled, out int pDelay);

        [PreserveSig]
        int SetAudioDelay(bool bEnabled, int delay);
        
        [PreserveSig]
        int SetMixingEnabled(bool bEnabled);

        [PreserveSig]
        bool GetMixingEnabled();
        
        [PreserveSig]
        int SetMixingLayout(ELAVAudioMixingLayout dwLayout);

        [PreserveSig]
        ELAVAudioMixingLayout GetMixingLayout();

        [PreserveSig]
        int SetMixingFlags(ELAVAudioMixingFlag dwFlags);

        [PreserveSig]
        ELAVAudioMixingFlag GetMixingFlags();
        
        [PreserveSig]
        int SetMixingMode(ELAVAudioMixingMode mixingMode);

        [PreserveSig]
        ELAVAudioMixingMode GetMixingMode();
        
        [PreserveSig]
        int SetMixingLevels(int dwCenterLevel, int dwSurroundLevel, int dwLFELevel);

        [PreserveSig]
        int GetMixingLevels(out int dwCenterLevel, out int dwSurroundLevel, out int dwLFELevel);
        
        [PreserveSig]
        int SetTrayIcon(bool bEnabled);

        [PreserveSig]
        bool GetTrayIcon();

        [PreserveSig]
        int SetSampleConvertDithering(bool bEnabled);

        [PreserveSig]
        bool GetSampleConvertDithering();
    }
}

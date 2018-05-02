using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace FoundaryMediaPlayer.Interfaces
{
    [ComImport]
    [Guid("FA40D6E9-4D38-4761-ADD2-71A9EC5FD32F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public interface ILAVVideoSettings
    {
        [PreserveSig]
        int SetRuntimeConfig(
            bool bRuntimeConfig
        );
        
        [PreserveSig]
        bool GetFormatConfiguration(
            ELAVVideoCodec vCodec
        );

        [PreserveSig]
        int SetFormatConfiguration(
            ELAVVideoCodec vCodec, 
            bool bEnabled
        );
        
        [PreserveSig]
        int SetNumThreads(
            uint dwNum
        );
        
        [PreserveSig]
        int GetNumThreads();
        
        [PreserveSig]
        int SetStreamAR(
            bool bStreamAR
        );
        
        [PreserveSig]
        bool GetStreamAR();
        
        [PreserveSig]
        bool GetPixelFormat(
            ELAVOutputPixelFormat pixFmt
        );

        [PreserveSig]
        int SetPixelFormat(
            ELAVOutputPixelFormat pixFmt, 
            bool bEnabled
        );
        
        [PreserveSig]
        int SetRGBOutputRange(
            uint dwRange
        );
        
        [PreserveSig]
        int GetRGBOutputRange();
        
        [PreserveSig]
        int SetDeintFieldOrder(
            ELAVDeinterlaceFieldOrder fieldOrder
        );
        
        [PreserveSig]
        ELAVDeinterlaceFieldOrder GetDeintFieldOrder();
        
        [PreserveSig]
        int SetDeintAggressive(
            bool bAggressive
        );
        
        [PreserveSig]
        bool GetDeintAggressive();
        
        [PreserveSig]
        int SetDeintForce(
            bool bForce
        );
        
        [PreserveSig]
        bool GetDeintForce();
        
        [PreserveSig]
        int CheckHWAccelSupport(
            ELAVHardwareAcceleration hwAccel
        );
        
        [PreserveSig]
        int SetHWAccel(
            ELAVHardwareAcceleration hwAccel
        );
        
        [PreserveSig]
        ELAVHardwareAcceleration GetHWAccel();
        
        [PreserveSig]
        int SetHWAccelCodec(
            ELAVVideoHardwareCodec hwAccelCodec, 
            bool bEnabled
        );
        
        [PreserveSig]
        bool GetHWAccelCodec(
            ELAVVideoHardwareCodec hwAccelCodec
        );
        
        [PreserveSig]
        int SetHWAccelDeintMode(
            ELAVDeinterlaceMode deintMode
        );
        
        [PreserveSig]
        ELAVDeinterlaceMode GetHWAccelDeintMode();
        
        [PreserveSig]
        int SetHWAccelDeintOutput(
            ELAVDeinterlaceOutput deintOutput
        );
        
        [PreserveSig]
        ELAVDeinterlaceOutput GetHWAccelDeintOutput();
        
        [PreserveSig]
        int SetHWAccelDeintHQ(
            bool bHQ
        );
        
        [PreserveSig]
        bool GetHWAccelDeintHQ();
        
        [PreserveSig]
        int SetSWDeintMode(
            ELAVSoftwareDeinterlaceMode deintMode
        );
        
        [PreserveSig]
        ELAVSoftwareDeinterlaceMode GetSWDeintMode();
        
        [PreserveSig]
        int SetSWDeintOutput(
            ELAVDeinterlaceOutput deintOutput
        );
        
        [PreserveSig]
        ELAVDeinterlaceOutput GetSWDeintOutput();
        
        [PreserveSig]
        int SetDeintTreatAsProgressive(
            bool bEnabled
        );
        
        [PreserveSig]
        bool GetDeintTreatAsProgressive();
        
        [PreserveSig]
        bool SetDitherMode(
            ELAVDitherMode ditherMode
        );
        
        [PreserveSig]
        ELAVDitherMode GetDitherMode();
        
        [PreserveSig]
        bool SetUseMSWMV9Decoder(
            bool bEnabled
        );
        
        [PreserveSig]
        bool GetUseMSWMV9Decoder();
        
        [PreserveSig]
        int SetDVDVideoSupport(
            bool bEnabled
        );
        
        [PreserveSig]
        bool GetDVDVideoSupport();
        
        [PreserveSig]
        int SetHWAccelResolutionFlags(
            int dwResFlags
        );
        
        [PreserveSig]
        int GetHWAccelResolutionFlags();
        
        [PreserveSig]
        int SetTrayIcon(
            bool bEnabled
        );
        
        [PreserveSig]
        bool GetTrayIcon();
        
        [PreserveSig]
        int SetDeinterlacingMode(
            ELAVDeinterlaceMode deintMode
        );
        
        [PreserveSig]
        ELAVDeinterlaceMode GetDeinterlacingMode();
    }
}

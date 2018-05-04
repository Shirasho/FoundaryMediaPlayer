using System;
using System.Runtime.InteropServices;

namespace FoundaryMediaPlayer.Interfaces
{
    [ComImport]
    [Guid("A668B8F2-BA87-4F63-9D41-768F7DE9C50E")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ILAVAudioStatus
    {
        [PreserveSig]
        bool IsSampleFormatSupported(ELAVAudioSampleFormat sfCheck);

        [PreserveSig]
        int GetDecodeDetails(IntPtr pCodec, IntPtr pDecodeFormat, out int pnChannels, out int pSampleRate,
                             out uint pChannelMask);

        [PreserveSig]
        int GetOutputDetails(IntPtr pOutputFormat, out int pnChannels, out int pSampleRate, out uint pChannelMask);

        [PreserveSig]
        int EnableVolumeStats();

        [PreserveSig]
        int DisableVolumeStats();

        [PreserveSig]
        int GetChannelVolumeAverage(ushort nChannel, out float pfDb);
    }
}

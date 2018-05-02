using System.Diagnostics.CodeAnalysis;

namespace FoundaryMediaPlayer.Interfaces
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum ELAVAudioCodec
    {
        AAC = 1,
        AC3,
        EAC3,
        DTS,
        MP2,
        MP3,
        TRUEHD,
        FLAC,
        VORBIS,
        LPCM,
        PCM,
        WAVPACK,
        TTA,
        WMA2,
        WMAPRO,
        Cook,
        RealAudio,
        WMALL,
        ALAC,
        Opus,
        AMR,
        Nellymoser,
        MSPCM,
        Truespeech,
        TAK,

        COUNT = TAK
    }
}

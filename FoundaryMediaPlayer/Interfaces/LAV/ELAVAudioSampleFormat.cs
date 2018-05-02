using System.Diagnostics.CodeAnalysis;

namespace FoundaryMediaPlayer.Interfaces
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum ELAVAudioSampleFormat
    {
        Sixteen = 1,
        TwentyFour,
        ThirtyTwo,
        U8,
        FP32,
        Bitstream,

        COUNT = Bitstream
    }
}

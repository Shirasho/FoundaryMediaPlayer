using System.Diagnostics.CodeAnalysis;

namespace FoundaryMediaPlayer.Interfaces
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum ELAVAudioMixingLayout
    {
        Stereo = 3,
        Quadraphonic = 1539,
        FiveDotOne = 63,
        SixDotOne = 1807,
        SevenDotOne = 1599
    }
}

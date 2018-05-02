using System.Diagnostics.CodeAnalysis;

namespace FoundaryMediaPlayer.Interfaces
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum ELAVAudioMixingMode
    {
        None,
        Dolby,
        DPLII,

        COUNT = DPLII + 1
    }
}

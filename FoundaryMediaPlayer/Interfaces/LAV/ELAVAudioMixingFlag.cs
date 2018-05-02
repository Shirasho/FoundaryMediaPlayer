using System.Diagnostics.CodeAnalysis;

namespace FoundaryMediaPlayer.Interfaces
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum ELAVAudioMixingFlag
    {
        UntouchedStereo = 0x0001,
        NormalizeMatrix = 0x0002,
        ClipProtection = 0x0004
    }
}

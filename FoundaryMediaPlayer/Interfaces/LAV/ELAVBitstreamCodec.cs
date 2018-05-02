using System.Diagnostics.CodeAnalysis;

namespace FoundaryMediaPlayer.Interfaces
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum ELAVBitstreamCodec
    {
        AC3 = 1,
        EAC3,
        TRUEHD,
        DTS,
        DTSHD,

        COUNT = DTSHD
    }
}

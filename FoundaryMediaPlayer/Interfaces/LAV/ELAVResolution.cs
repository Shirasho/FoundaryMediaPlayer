using System.Diagnostics.CodeAnalysis;

namespace FoundaryMediaPlayer.Interfaces
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum ELAVResolution
    {
        SD = 0x0001,
        HD = 0x0002,
        UHD = 0x0004
    }
}

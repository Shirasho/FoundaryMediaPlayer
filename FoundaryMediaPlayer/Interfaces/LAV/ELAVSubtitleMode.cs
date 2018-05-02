using System.Diagnostics.CodeAnalysis;

namespace FoundaryMediaPlayer.Interfaces
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum ELAVSubtitleMode
    {
        Disabled,
        ForcedOnly,
        Default,
        Advanced
    }
}

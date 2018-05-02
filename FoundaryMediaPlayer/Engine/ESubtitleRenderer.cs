using System.Diagnostics.CodeAnalysis;

namespace FoundaryMediaPlayer.Engine
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum ESubtitleRenderer
    {
        VSFilter,
        XYSubFilter,
        ASSFilter,
        Internal
    }
}

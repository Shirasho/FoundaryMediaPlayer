using System.Diagnostics.CodeAnalysis;

#pragma warning disable 1591

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

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum EVideoRenderer
    {
        Default,
        Null,
        VMR9Windowed,
        VMR9Renderless,
        EVR,
        EVRCustom,
        MadVR,
        Sync
    }
}

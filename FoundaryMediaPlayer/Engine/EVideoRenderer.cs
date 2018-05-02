using System.Diagnostics.CodeAnalysis;

namespace FoundaryMediaPlayer.Engine
{
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

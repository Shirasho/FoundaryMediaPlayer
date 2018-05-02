using System.Diagnostics.CodeAnalysis;

namespace FoundaryMediaPlayer.Interfaces
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum ELAVDeinterlaceFieldOrder
    {
        Auto,
        TopFieldFirst,
        BottomFieldFirst,
    }
}

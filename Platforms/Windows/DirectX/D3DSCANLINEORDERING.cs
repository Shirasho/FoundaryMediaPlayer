using System.Diagnostics.CodeAnalysis;

namespace FoundaryMediaPlayer.Platforms.Windows.DirectX
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal enum D3DSCANLINEORDERING
    {
        D3DSCANLINEORDERING_UNKNOWN = 0,
        D3DSCANLINEORDERING_PROGRESSIVE = 1,
        D3DSCANLINEORDERING_INTERLACED = 2,
    }
}

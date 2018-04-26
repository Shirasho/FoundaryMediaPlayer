using System;
using System.Diagnostics.CodeAnalysis;

namespace FoundaryMediaPlayer.Platforms.Windows.DirectX
{
    [Flags]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal enum D3DSWAPEFFECT 
    {
        D3DSWAPEFFECT_DISCARD = 1,
        D3DSWAPEFFECT_FLIP = 2,
        D3DSWAPEFFECT_COPY = 3,
    }
}

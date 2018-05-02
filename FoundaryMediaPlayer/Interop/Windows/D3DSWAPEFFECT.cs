using System;
using System.Diagnostics.CodeAnalysis;

namespace FoundaryMediaPlayer.Interop.Windows
{
    [Flags]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum D3DSWAPEFFECT 
    {
        D3DSWAPEFFECT_DISCARD = 1,
        D3DSWAPEFFECT_FLIP = 2,
        D3DSWAPEFFECT_COPY = 3,
    }
}

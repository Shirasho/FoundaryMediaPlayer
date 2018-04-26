using System;
using System.Diagnostics.CodeAnalysis;

namespace FoundaryMediaPlayer.Platforms.Windows.DirectX
{
    [Flags]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal enum D3DPRESENTFLAG 
    {
        D3DPRESENTFLAG_LOCKABLE_BACKBUFFER = 0x00000001,
        D3DPRESENTFLAG_DISCARD_DEPTHSTENCIL = 0x00000002,
        D3DPRESENTFLAG_DEVICECLIP = 0x00000004,
        D3DPRESENTFLAG_VIDEO = 0x00000010      
    }
}

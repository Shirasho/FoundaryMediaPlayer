﻿using System.Diagnostics.CodeAnalysis;

namespace FoundaryMediaPlayer.Platforms.Windows.DirectX
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal enum D3DRESOURCETYPE
    {
        D3DRTYPE_SURFACE = 1,
        D3DRTYPE_VOLUME = 2,
        D3DRTYPE_TEXTURE = 3,
        D3DRTYPE_VOLUMETEXTURE = 4,
        D3DRTYPE_CUBETEXTURE = 5,
        D3DRTYPE_VERTEXBUFFER = 6,
        D3DRTYPE_INDEXBUFFER = 7,

        D3DRTYPE_FORCE_DWORD = 0x7fffffff
    }
}
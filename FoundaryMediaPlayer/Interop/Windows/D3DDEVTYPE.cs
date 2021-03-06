﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace FoundaryMediaPlayer.Interop.Windows
{
    [Flags]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum D3DDEVTYPE
    {
        D3DDEVTYPE_HAL = 1,
        D3DDEVTYPE_REF = 2,
        D3DDEVTYPE_SW = 3,
        D3DDEVTYPE_NULLREF = 4,
    }
}

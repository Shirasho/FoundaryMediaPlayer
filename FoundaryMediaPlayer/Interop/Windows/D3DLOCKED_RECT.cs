using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace FoundaryMediaPlayer.Interop.Windows
{
    [StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public struct D3DLOCKED_RECT
    {
        public int Pitch;
        public IntPtr pBits;
    }
}

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace FoundaryMediaPlayer.Platforms.Windows.DirectX
{
    [StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal struct D3DDISPLAYMODEEX
    {
        public uint Size;
        public uint Width;
        public uint Height;
        public uint RefreshRate;
        public D3DFORMAT Format;
        public D3DSCANLINEORDERING ScanLineOrdering;
    }
}

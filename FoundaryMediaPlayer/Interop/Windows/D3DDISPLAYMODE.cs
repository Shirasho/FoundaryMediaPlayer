using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace FoundaryMediaPlayer.Interop.Windows
{
    [StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public struct D3DDISPLAYMODE
    {
        public uint Width;
        public uint Height;
        public uint RefreshRate;
        public D3DFORMAT Format;
    } 
}

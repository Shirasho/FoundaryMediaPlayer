using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace FoundaryMediaPlayer.Platforms.Windows.DirectX
{
    /// <summary>
    /// Dummy size of 1 needed for compilation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal struct D3DSURFACE_DESC
    {
        public D3DFORMAT Format;
        public D3DRESOURCETYPE Type;
        public int Usage;
        public D3DPOOL Pool;
        public D3DMULTISAMPLE_TYPE MultiSampleType;
        public int MultiSampleQuality;
        public uint Width;
        public uint Height;
    }
}

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace FoundaryMediaPlayer.Interop.Windows
{
    [StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public struct D3DVSHADERCAPS2_0
    {
        public int Caps;
        public int DynamicFlowControlDepth;
        public int NumTemps;
        public int StaticFlowControlDepth;
    } 
}

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace FoundaryMediaPlayer.Platforms.Windows.DirectX
{
    [StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal struct D3DPRESENT_PARAMETERS
    {
        public uint BackBufferWidth;
        public uint BackBufferHeight;
        public D3DFORMAT BackBufferFormat;
        public uint BackBufferCount;
        public D3DMULTISAMPLE_TYPE MultiSampleType;
        public int MultiSampleQuality;
        public D3DSWAPEFFECT SwapEffect;
        public IntPtr hDeviceWindow;
        public int Windowed;
        public int EnableAutoDepthStencil;
        public D3DFORMAT AutoDepthStencilFormat;
        public int Flags;
        /* FullScreen_RefreshRateInHz must be zero for Windowed mode */
        public uint FullScreen_RefreshRateInHz;
        public uint PresentationInterval;
    }
}

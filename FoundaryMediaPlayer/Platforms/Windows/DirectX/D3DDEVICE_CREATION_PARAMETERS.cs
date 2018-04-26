using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace FoundaryMediaPlayer.Platforms.Windows.DirectX
{
    [StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal struct D3DDEVICE_CREATION_PARAMETERS
    {
        public uint AdapterOrdinal;
        public D3DDEVTYPE DeviceType;
        public IntPtr hFocusWindow;
        public int BehaviorFlags;
    }
}

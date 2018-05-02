using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace FoundaryMediaPlayer.Interop.Windows
{
    [StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public struct D3DDEVICE_CREATION_PARAMETERS
    {
        public uint AdapterOrdinal;
        public D3DDEVTYPE DeviceType;
        public IntPtr hFocusWindow;
        public int BehaviorFlags;
    }
}

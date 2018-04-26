#pragma warning disable 1591

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using FoundaryMediaPlayer.Engine;

namespace FoundaryMediaPlayer.Platforms.Windows
{
    [SuppressUnmanagedCodeSecurity]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("B110CDE5-6331-4118-8AAF-A870D6F7E2E4")]
    [ComImport]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal interface IGraphEngine
    {
        [MethodImpl(MethodImplOptions.PreserveSig)]
        EEngineType GetEngine();
    }
}

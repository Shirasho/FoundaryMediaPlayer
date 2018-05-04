using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using FoundaryMediaPlayer.Engine;

namespace FoundaryMediaPlayer.Interfaces
{
    [ComImport, SuppressUnmanagedCodeSecurity]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("B110CDE5-6331-4118-8AAF-A870D6F7E2E4")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public interface IGraphEngine
    {
        [MethodImpl(MethodImplOptions.PreserveSig)]
        EEngineType GetEngine();
    }
}

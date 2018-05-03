using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using DirectShowLib;
using FoundaryMediaPlayer.Engine;

namespace FoundaryMediaPlayer.Interfaces
{
    [SuppressUnmanagedCodeSecurity]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("43CDA93D-6A4E-4A07-BD3E-49D161073EE7")]
    [ComImport]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public interface IGraphBuilderDeadEnd
    {
        [MethodImpl(MethodImplOptions.PreserveSig)]
        uint GetCount();

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int GetDeadEnd([In] int iIndex, [In] IList<string> path, [In] IList<AMMediaType> mts);
    }
}

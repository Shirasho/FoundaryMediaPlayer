﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using DirectShowLib;

namespace FoundaryMediaPlayer.Interfaces
{
    [ComImport, SuppressUnmanagedCodeSecurity]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("56a868bf-0ad4-11ce-b03a-0020af0ba770")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public interface IStreamBuilder
    {
        [MethodImpl(MethodImplOptions.PreserveSig)]
        int Render([In] IPin ppinOut, [In] IGraphBuilder pGraph);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int Backout([In] IPin ppinOut, [In] IGraphBuilder pGraph);
    }
}

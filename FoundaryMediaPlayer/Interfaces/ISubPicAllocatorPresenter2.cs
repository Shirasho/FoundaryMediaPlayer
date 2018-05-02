#pragma warning disable 1591

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;

namespace FoundaryMediaPlayer.Interfaces
{
    [SuppressUnmanagedCodeSecurity]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("767AEBA8-A084-488a-89C8-F6B74E53A90F")]
    [ComImport]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public interface ISubPicAllocatorPresenter2 : ISubPicAllocatorPresenter
    {
        //STDMETHOD(SetPixelShader2)(LPCSTR pSrcData, LPCSTR pTarget, bool bScreenSpace) PURE;
        //STDMETHOD_(SIZE, GetVisibleVideoSize)() const PURE;

        //STDMETHOD_(bool, IsRendering)() PURE;
        //STDMETHOD(SetIsRendering)(bool bIsRendering) PURE;

        //STDMETHOD(SetDefaultVideoAngle)(Vector v) PURE;
    }
}

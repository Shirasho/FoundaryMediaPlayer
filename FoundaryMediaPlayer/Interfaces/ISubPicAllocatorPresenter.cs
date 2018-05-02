#pragma warning disable 1591

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;

namespace FoundaryMediaPlayer.Interfaces
{
    [SuppressUnmanagedCodeSecurity]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("CF75B1F0-535C-4074-8869-B15F177F944E")]
    [ComImport]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public interface ISubPicAllocatorPresenter
    {
        //STDMETHOD(CreateRenderer)(IUnknown** ppRenderer) PURE;

        //STDMETHOD_(SIZE, GetVideoSize)(bool bCorrectAR = true) const PURE;
        //STDMETHOD_(void, SetPosition)(RECT w, RECT v) PURE;
        //STDMETHOD_(bool, Paint)(bool bAll) PURE;

        //STDMETHOD_(void, SetTime)(REFERENCE_TIME rtNow) PURE;
        //STDMETHOD_(void, SetSubtitleDelay)(int delayMs) PURE;
        //STDMETHOD_(int, GetSubtitleDelay)() const PURE;
        //STDMETHOD_(double, GetFPS)() const PURE;

        //STDMETHOD_(void, SetSubPicProvider)(ISubPicProvider * pSubPicProvider) PURE;
        //STDMETHOD_(void, Invalidate)(REFERENCE_TIME rtInvalidate = -1) PURE;

        //STDMETHOD(GetDIB)(BYTE * lpDib, DWORD * size) PURE;

        //STDMETHOD(SetVideoAngle)(Vector v) PURE;
        //STDMETHOD(SetPixelShader)(LPCSTR pSrcData, LPCSTR pTarget) PURE;

        //STDMETHOD_(bool, ResetDevice)() PURE;
        //STDMETHOD_(bool, DisplayChange)() PURE;
    }
}

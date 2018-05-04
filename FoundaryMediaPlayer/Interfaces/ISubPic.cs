using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace FoundaryMediaPlayer.Interfaces
{
    [ComImport, SuppressUnmanagedCodeSecurity]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("DA3A5B51-958C-4C28-BF66-68D7947577A2")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public interface ISubPic
    {
        //TODO: Implement
        //STDMETHOD_(void*, GetObject)() PURE;

        //STDMETHOD_(REFERENCE_TIME, GetStart)() const PURE;
        //STDMETHOD_(REFERENCE_TIME, GetStop)() const PURE;
        //STDMETHOD_(void, SetStart)(REFERENCE_TIME rtStart) PURE;
        //STDMETHOD_(void, SetStop)(REFERENCE_TIME rtStop) PURE;

        //STDMETHOD(GetDesc)(SubPicDesc& spd /*[out]*/) PURE;
        //STDMETHOD(CopyTo)(ISubPic* pSubPic /*[in]*/) PURE;

        //STDMETHOD(ClearDirtyRect)(DWORD color /*[in]*/) PURE;
        //STDMETHOD(GetDirtyRect)(RECT* pDirtyRect /*[out]*/) const PURE;
        //STDMETHOD(SetDirtyRect)(const RECT* pDirtyRect /*[in]*/) PURE;

        //STDMETHOD(GetMaxSize)(SIZE* pMaxSize /*[out]*/) const PURE;
        //STDMETHOD(SetSize)(SIZE pSize /*[in]*/, RECT vidrect /*[in]*/) PURE;

        //STDMETHOD(Lock)(SubPicDesc& spd /*[out]*/) PURE;
        //STDMETHOD(Unlock)(RECT* pDirtyRect /*[in]*/) PURE;

        //STDMETHOD(AlphaBlt)(RECT * pSrc, RECT * pDst, SubPicDesc* pTarget = nullptr /*[in]*/) PURE;
        //STDMETHOD(GetSourceAndDest)(RECT rcWindow /*[in]*/, RECT rcVideo /*[in]*/,
        //RECT* pRcSource /*[out]*/,  RECT* pRcDest /*[out]*/,
        //const double videoStretchFactor = 1.0 /*[in]*/,
        //int xOffsetInPixels = 1 /*[in]*/) const PURE;
        //STDMETHOD(SetVirtualTextureSize)(const SIZE pSize, const POINT pTopLeft) PURE;
        //STDMETHOD(GetRelativeTo)(RelativeTo* pRelativeTo /*[out]*/) const PURE;
        //STDMETHOD(SetRelativeTo)(RelativeTo relativeTo /*[in]*/) PURE;

        //STDMETHOD_(REFERENCE_TIME, GetSegmentStart)() const PURE;
        //STDMETHOD_(REFERENCE_TIME, GetSegmentStop)() const PURE;
        //STDMETHOD_(void, SetSegmentStart)(REFERENCE_TIME rtStart) PURE;
        //STDMETHOD_(void, SetSegmentStop)(REFERENCE_TIME rtStop) PURE;

        //STDMETHOD_(bool, GetInverseAlpha)() const PURE;
        //STDMETHOD_(void, SetInverseAlpha)(bool bInverted) PURE;
    }
}

#pragma warning disable 1591

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;

namespace FoundaryMediaPlayer.Interfaces
{
    [SuppressUnmanagedCodeSecurity]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("C8334466-CD1E-4ad1-9D2D-8EE8519BD180")]
    [ComImport]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public interface ISubPicQueue
    {
        //STDMETHOD(SetSubPicProvider)(ISubPicProvider* pSubPicProvider /*[in]*/) PURE;
        //STDMETHOD(GetSubPicProvider)(ISubPicProvider** pSubPicProvider /*[out]*/) PURE;

        //STDMETHOD(SetFPS)(double fps /*[in]*/) PURE;
        //STDMETHOD(SetTime)(REFERENCE_TIME rtNow /*[in]*/) PURE;

        //STDMETHOD(Invalidate)(REFERENCE_TIME rtInvalidate = -1) PURE;
        //STDMETHOD_(bool, LookupSubPic)(REFERENCE_TIME rtNow /*[in]*/, CComPtr<ISubPic>& pSubPic /*[out]*/) PURE;

        //STDMETHOD(GetStats)(int& nSubPics, REFERENCE_TIME & rtNow, REFERENCE_TIME & rtStart, REFERENCE_TIME& rtStop /*[out]*/) PURE;
        //STDMETHOD(GetStats)(int nSubPic /*[in]*/, REFERENCE_TIME & rtStart, REFERENCE_TIME& rtStop /*[out]*/) PURE;

        //STDMETHOD_(bool, LookupSubPic)(REFERENCE_TIME rtNow /*[in]*/, bool bAdviseBlocking, CComPtr<ISubPic>& pSubPic /*[out]*/) PURE;
    }
}

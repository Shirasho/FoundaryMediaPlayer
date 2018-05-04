using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace FoundaryMediaPlayer.Interfaces
{
    [ComImport, SuppressUnmanagedCodeSecurity]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("D62B9A1A-879A-42db-AB04-88AA8F243CFD")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public interface ISubPicProvider
    {
        //TODO: Implement
        //STDMETHOD(Lock)() PURE;
        //STDMETHOD(Unlock)() PURE;

        //STDMETHOD_(POSITION, GetStartPosition)(REFERENCE_TIME rt, double fps) PURE;
        //STDMETHOD_(POSITION, GetNext)(POSITION pos) PURE;

        //STDMETHOD_(REFERENCE_TIME, GetStart)(POSITION pos, double fps) PURE;
        //STDMETHOD_(REFERENCE_TIME, GetStop)(POSITION pos, double fps) PURE;

        //STDMETHOD_(bool, IsAnimated)(POSITION pos) PURE;

        //STDMETHOD(Render)(SubPicDesc & spd, REFERENCE_TIME rt, double fps, RECT & bbox) PURE;
        //STDMETHOD(GetTextureSize)(POSITION pos, SIZE & MaxTextureSize, SIZE & VirtualSize, POINT & VirtualTopLeft) PURE;
        //STDMETHOD(GetRelativeTo)(POSITION pos, RelativeTo & relativeTo) PURE;
    }
}

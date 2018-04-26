using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Shapes;

namespace FoundaryMediaPlayer.Platforms.Windows.DirectX
{
    [ComImport]
    [SuppressUnmanagedCodeSecurity]
    [Guid("0CFBAF3A-9FF6-429a-99B3-A2796AF8B89B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [SuppressUnmanagedCodeSecurity]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal interface IDirect3DSurface9
    {
        [PreserveSig, SuppressUnmanagedCodeSecurity]
        void GetDevice(out IDirect3DDevice9 ppDevice);
        [PreserveSig, SuppressUnmanagedCodeSecurity]
        void SetPrivateData(Guid refguid, IntPtr pData, int SizeOfData, int Flags);
        [PreserveSig, SuppressUnmanagedCodeSecurity]
        void GetPrivateData(Guid refguid, IntPtr pData, out int pSizeOfData);
        [PreserveSig, SuppressUnmanagedCodeSecurity]
        void FreePrivateData(Guid refguid);
        [PreserveSig, SuppressUnmanagedCodeSecurity]
        int SetPriority(int PriorityNew);
        [PreserveSig, SuppressUnmanagedCodeSecurity]
        int GetPriority();
        void PreLoad();
        [PreserveSig, SuppressUnmanagedCodeSecurity]
        D3DRESOURCETYPE GetType();
        void GetContainer(Guid riid, out object ppContainer);
        void GetDesc(out D3DSURFACE_DESC pDesc);
        void LockRect(D3DLOCKED_RECT pLockedRect, Rectangle pRect, int Flags);
        void UnlockRect();
        [PreserveSig, SuppressUnmanagedCodeSecurity]
        int GetDC(out IntPtr phdc);
        [PreserveSig, SuppressUnmanagedCodeSecurity]
        int ReleaseDC(IntPtr hdc);
    }
}

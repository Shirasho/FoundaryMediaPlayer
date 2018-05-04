using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;
using FoundaryMediaPlayer.Interop.Windows;
using PInvoke;

namespace FoundaryMediaPlayer.Interfaces
{
    [ComImport, SuppressUnmanagedCodeSecurity]
    [Guid("85C31227-3DE5-4f00-9B3A-F11AC38C18B5")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public interface IDirect3DTexture9
    {
        [PreserveSig, SuppressUnmanagedCodeSecurity]
        void GetDevice();

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        void SetPrivateData(Guid refguid, IntPtr pData, int SizeOfData, int Flags);

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        void GetPrivateData(Guid refguid, IntPtr pData, IntPtr pSizeOfData);

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        void FreePrivateData(Guid refguid);

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        void SetPriority(int PriorityNew);

        void GetPriority();

        void PreLoad();

        void GetType();

        void SetLOD(int LODNew);

        void GetLOD();

        void GetLevelCount();

        void SetAutoGenFilterType(int FilterType);

        int GetAutoGenFilterType();

        void GenerateMipSubLevels();

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        void GetLevelDesc(int Level, IntPtr pDesc);

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        int GetSurfaceLevel(int Level, out IDirect3DSurface9 ppSurfaceLevel);

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        void LockRect(int Level, ref D3DLOCKED_RECT pLockedRect, RECT pRect, int Flags);

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        void UnlockRect(int Level);

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        void AddDirtyRect(RECT pDirtyRect);
    }
}

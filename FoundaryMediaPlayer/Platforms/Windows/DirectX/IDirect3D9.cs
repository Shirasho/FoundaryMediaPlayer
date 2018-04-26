using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;

namespace FoundaryMediaPlayer.Platforms.Windows.DirectX
{
    [ComImport]
    [SuppressUnmanagedCodeSecurity]
    [Guid("81BDCBCA-64D4-426d-AE8D-AD0147F4275C")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [SuppressUnmanagedCodeSecurity]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal interface IDirect3D9
    {
        [PreserveSig, SuppressUnmanagedCodeSecurity]
        int RegisterSoftwareDevice([In, Out]IntPtr pInitializeFunction);

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        int GetAdapterCount();

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        int GetAdapterIdentifier(uint Adapter, uint Flags, uint pIdentifier);

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        uint GetAdapterModeCount(uint Adapter, D3DFORMAT Format);

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        int EnumAdapterModes(uint Adapter, D3DFORMAT Format, uint Mode, [Out] out D3DDISPLAYMODE pMode);

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        int GetAdapterDisplayMode(ushort Adapter, [Out]out D3DFORMAT Format);

        #region Method Placeholders
        [PreserveSig]
        int CheckDeviceType();

        [PreserveSig]
        int CheckDeviceFormat();

        [PreserveSig]
        int CheckDeviceMultiSampleType();

        [PreserveSig]
        int CheckDepthStencilMatch();

        [PreserveSig]
        int CheckDeviceFormatConversion();

        [PreserveSig]
        int GetDeviceCaps();
        #endregion

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        IntPtr GetAdapterMonitor(uint Adapter);

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        int CreateDevice(int Adapter,
            D3DDEVTYPE DeviceType,
            IntPtr hFocusWindow,
            CreateFlags BehaviorFlags,
            [In, Out]
            ref D3DPRESENT_PARAMETERS pPresentationParameters,
            [Out]out IntPtr ppReturnedDeviceInterface);
    }
}

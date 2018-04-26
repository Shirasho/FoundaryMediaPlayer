using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;

namespace FoundaryMediaPlayer.Platforms.Windows.DirectX
{
    [ComImport]
    [SuppressUnmanagedCodeSecurity]
    [Guid("02177241-69FC-400C-8FF1-93A44DF6861D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [SuppressUnmanagedCodeSecurity]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal interface IDirect3D9Ex : IDirect3D9
    {
        [PreserveSig, SuppressUnmanagedCodeSecurity]
        new int RegisterSoftwareDevice([In, Out]IntPtr pInitializeFunction);

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        new int GetAdapterCount();

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        new int GetAdapterIdentifier(uint Adapter, uint Flags, uint pIdentifier);

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        new uint GetAdapterModeCount(uint Adapter, D3DFORMAT Format);

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        new int EnumAdapterModes(uint Adapter, D3DFORMAT Format, uint Mode, [Out] out D3DDISPLAYMODE pMode);

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        new int GetAdapterDisplayMode(ushort Adapter, [Out]out D3DFORMAT Format);

        #region Method Placeholders
        [PreserveSig]
        new int CheckDeviceType();

        [PreserveSig]
        new int CheckDeviceFormat();

        [PreserveSig]
        new int CheckDeviceMultiSampleType();

        [PreserveSig]
        new int CheckDepthStencilMatch();

        [PreserveSig]
        new int CheckDeviceFormatConversion();

        [PreserveSig]
        new int GetDeviceCaps();
        #endregion

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        new IntPtr GetAdapterMonitor(uint Adapter);

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        new int CreateDevice(int Adapter,
                          D3DDEVTYPE DeviceType,
                          IntPtr hFocusWindow,
                          CreateFlags BehaviorFlags,
                          [In, Out]
                          ref D3DPRESENT_PARAMETERS pPresentationParameters,
                          [Out]out IntPtr ppReturnedDeviceInterface);

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        uint GetAdapterModeCountEx();

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        int EnumAdapterModesEx();

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        int GetAdapterDisplayModeEx();

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        int CreateDeviceEx(int Adapter,
                          D3DDEVTYPE DeviceType,
                          IntPtr hFocusWindow,
                          CreateFlags BehaviorFlags,
                          [In, Out]
                          ref D3DPRESENT_PARAMETERS pPresentationParameters,
                          [In, Out]
                          IntPtr pFullscreenDisplayMode,
                          [Out]out IntPtr ppReturnedDeviceInterface);

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        int GetAdapterLUID();
    }

}

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;
using DirectShowLib;

namespace FoundaryMediaPlayer.Platforms.Windows.DirectX
{
    [ComImport]
    [Guid("B18B10CE-2649-405a-870F-95F777D4313A")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [SuppressUnmanagedCodeSecurity]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal interface IDirect3DDevice9Ex : IDirect3DDevice9
    {
        [PreserveSig, SuppressUnmanagedCodeSecurity]
        new int TestCooperativeLevel();
        [PreserveSig, SuppressUnmanagedCodeSecurity]
        new int GetAvailableTextureMem();
        [PreserveSig, SuppressUnmanagedCodeSecurity]
        new int EvictManagedResources();
        [PreserveSig, SuppressUnmanagedCodeSecurity]
        new int GetDirect3D([Out] out IDirect3D9 ppD3D9);
        [PreserveSig, SuppressUnmanagedCodeSecurity]
        new int GetDeviceCaps([In, Out] ref D3DCAPS9 pCaps);
        [PreserveSig, SuppressUnmanagedCodeSecurity]
        new int GetDisplayMode(uint iSwapChain, D3DDISPLAYMODE pMode);
        [PreserveSig, SuppressUnmanagedCodeSecurity]
        new int GetCreationParameters([In, Out] ref D3DDEVICE_CREATION_PARAMETERS pParameters);
        new int SetCursorProperties();
        new int SetCursorPosition();
        new int ShowCursor(bool bShow);
        new int CreateAdditionalSwapChain();
        new int GetSwapChain();
        new int GetNumberOfSwapChains();
        [PreserveSig, SuppressUnmanagedCodeSecurity]
        new int Reset([In, Out] ref D3DPRESENT_PARAMETERS pPresentationParameters);
        new int Present();
        new int GetBackBuffer();
        new int GetRasterStatus();
        new int SetDialogBoxMode();
        new int SetGammaRamp();
        new int GetGammaRamp();
        [PreserveSig, SuppressUnmanagedCodeSecurity]
        new int CreateTexture(int Width, int Height, int Levels, int Usage, D3DFORMAT Format, int Pool,
                          out IDirect3DTexture9 ppTexture, IntPtr pSharedHandle);
        new int CreateVolumeTexture();
        new int CreateCubeTexture();
        new int CreateVertexBuffer();
        new int CreateIndexBuffer();
        [PreserveSig, SuppressUnmanagedCodeSecurity]
        new int CreateRenderTarget(int width, int height, D3DFORMAT Format, D3DMULTISAMPLE_TYPE MultiSample,
                                 uint MultisampleQuality, [MarshalAs(UnmanagedType.Bool)] bool Lockable, [Out]out IntPtr pSurface,
                                 IntPtr pSharedSurface);
        new int CreateDepthStencilSurface();
        new int UpdateSurface();
        new int UpdateTexture();
        new int GetRenderTargetData();
        new int GetFrontBufferData();
        new int StretchRect(IntPtr pSourceSurface, DsRect pSourceRect, IDirect3DSurface9 pDestSurface, DsRect pDestRect, int Filter);
        new int ColorFill();
        new int CreateOffscreenPlainSurface();
        new int SetRenderTarget();
        [PreserveSig, SuppressUnmanagedCodeSecurity]
        new int GetRenderTarget([Out]out IntPtr pSurface);
        new int SetDepthStencilSurface();
        new int GetDepthStencilSurface();
        new int BeginScene();
        new int EndScene();
        new int Clear();
        new int SetTransform();
        new int GetTransform();
        new int MultiplyTransform();
        new int SetViewport();
        new int GetViewport();
        new int SetMaterial();
        new int GetMaterial();
        new int SetLight();
        new int GetLight();
        new int LightEnable();
        new int GetLightEnable();
        new int SetClipPlane();
        new int GetClipPlane();
        new int SetRenderState();
        new int GetRenderState();
        new int CreateStateBlock();
        new int BeginStateBlock();
        new int EndStateBlock();
        new int SetClipStatus();
        new int GetClipStatus();
        new int GetTexture();
        new int SetTexture();
        new int GetTextureStageState();
        new int SetTextureStageState();
        new int GetSamplerState();
        new int SetSamplerState();
        new int ValidateDevice();
        new int SetPaletteEntries();
        new int GetPaletteEntries();
        new int SetCurrentTexturePalette();
        new int GetCurrentTexturePalette();
        new int SetScissorRect();
        new int GetScissorRect();
        new int SetSoftwareVertexProcessing(bool bSoftware);
        new bool GetSoftwareVertexProcessing();
        new int SetNPatchMode(float nSegments);
        new float GetNPatchMode();
        new int DrawPrimitive();
        new int DrawIndexedPrimitive();
        new int DrawPrimitiveUP();
        new int DrawIndexedPrimitiveUP();
        new int ProcessVertices();
        new int CreateVertexDeclaration();
        new int SetVertexDeclaration();
        new int GetVertexDeclaration();
        new int SetFVF();
        new int GetFVF();
        new int CreateVertexShader();
        new int SetVertexShader();
        new int GetVertexShader();
        new int SetVertexShaderConstantF();
        new int GetVertexShaderConstantF();
        new int SetVertexShaderConstantI();
        new int GetVertexShaderConstantI();
        new int SetVertexShaderConstantB();
        new int GetVertexShaderConstantB();
        new int SetStreamSource();
        new int GetStreamSource();
        new int SetStreamSourceFreq();
        new int GetStreamSourceFreq();
        new int SetIndices();
        new int GetIndices();
        new int CreatePixelShader();
        new int SetPixelShader();
        new int GetPixelShader();
        new int SetPixelShaderConstantF();
        new int GetPixelShaderConstantF();
        new int SetPixelShaderConstantI();
        new int GetPixelShaderConstantI();
        new int SetPixelShaderConstantB();
        new int GetPixelShaderConstantB();
        new int DrawRectPatch();
        new int DrawTriPatch();
        new int DeletePatch(uint Handle);
        new int CreateQuery();
        [PreserveSig, SuppressUnmanagedCodeSecurity]
        int SetConvolutionMonoKernel(int width, int height, IntPtr rows, IntPtr columns);
        int ComposeRects();
        int PresentEx();
        int GetGPUThreadPriority();
        int SetGPUThreadPriority();
        int WaitForVBlank();
        int CheckResourceResidency();
        int SetMaximumFrameLatency();
        int GetMaximumFrameLatency();
        [PreserveSig, SuppressUnmanagedCodeSecurity]
        int CheckDeviceState(IntPtr hWnd);
        [PreserveSig, SuppressUnmanagedCodeSecurity]
        int CreateRenderTargetEx(int width, int height, D3DFORMAT Format, D3DMULTISAMPLE_TYPE MultiSample,
                                 uint MultisampleQuality, [MarshalAs(UnmanagedType.Bool)] bool Lockable, [Out]out IntPtr pSurface,
                                 [In, Out]ref IntPtr pSharedSurface, uint Usage);
        /*
         STDMETHOD(SetConvolutionMonoKernel)(THIS_ UINT width,UINT height,float* rows,float* columns) PURE;
    STDMETHOD(ComposeRects)(THIS_ IDirect3DSurface9* pSrc,IDirect3DSurface9* pDst,IDirect3DVertexBuffer9* pSrcRectDescs,UINT NumRects,IDirect3DVertexBuffer9* pDstRectDescs,D3DCOMPOSERECTSOP Operation,int Xoffset,int Yoffset) PURE;
    STDMETHOD(PresentEx)(THIS_ CONST RECT* pSourceRect,CONST RECT* pDestRect,HWND hDestWindowOverride,CONST RGNDATA* pDirtyRegion,DWORD dwFlags) PURE;
    STDMETHOD(GetGPUThreadPriority)(THIS_ INT* pPriority) PURE;
    STDMETHOD(SetGPUThreadPriority)(THIS_ INT Priority) PURE;
    STDMETHOD(WaitForVBlank)(THIS_ UINT iSwapChain) PURE;
    STDMETHOD(CheckResourceResidency)(THIS_ IDirect3DResource9** pResourceArray,UINT32 NumResources) PURE;
    STDMETHOD(SetMaximumFrameLatency)(THIS_ UINT MaxLatency) PURE;
    STDMETHOD(GetMaximumFrameLatency)(THIS_ UINT* pMaxLatency) PURE;
    STDMETHOD(CheckDeviceState)(THIS_ HWND hDestinationWindow) PURE;
    STDMETHOD(CreateRenderTargetEx)(THIS_ UINT Width,UINT Height,D3DFORMAT Format,D3DMULTISAMPLE_TYPE MultiSample,DWORD MultisampleQuality,BOOL Lockable,IDirect3DSurface9** ppSurface,HANDLE* pSharedHandle,DWORD Usage) PURE;
    STDMETHOD(CreateOffscreenPlainSurfaceEx)(THIS_ UINT Width,UINT Height,D3DFORMAT Format,D3DPOOL Pool,IDirect3DSurface9** ppSurface,HANDLE* pSharedHandle,DWORD Usage) PURE;
    STDMETHOD(CreateDepthStencilSurfaceEx)(THIS_ UINT Width,UINT Height,D3DFORMAT Format,D3DMULTISAMPLE_TYPE MultiSample,DWORD MultisampleQuality,BOOL Discard,IDirect3DSurface9** ppSurface,HANDLE* pSharedHandle,DWORD Usage) PURE;
    STDMETHOD(ResetEx)(THIS_ D3DPRESENT_PARAMETERS* pPresentationParameters,D3DDISPLAYMODEEX *pFullscreenDisplayMode) PURE;
    STDMETHOD(GetDisplayModeEx)(THIS_ UINT iSwapChain,D3DDISPLAYMODEEX* pMode,D3DDISPLAYROTATION* pRotation) PURE; 
         */
    }
}

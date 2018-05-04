using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;
using DirectShowLib;
using FoundaryMediaPlayer.Interop.Windows;

namespace FoundaryMediaPlayer.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    [ComImport, SuppressUnmanagedCodeSecurity]
    [Guid("D0223B96-BF7A-43fd-92BD-A43B0D82B9EB")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public interface IDirect3DDevice9
    {
        [PreserveSig, SuppressUnmanagedCodeSecurity]
        int TestCooperativeLevel();

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        uint GetAvailableTextureMem();

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        int EvictManagedResources();

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        int GetDirect3D([Out] out IDirect3D9 ppD3D9);

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        int GetDeviceCaps([In, Out] ref D3DCAPS9 pCaps);

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        int GetDisplayMode(uint iSwapChain, D3DDISPLAYMODE pMode);

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        int GetCreationParameters([In, Out] ref D3DDEVICE_CREATION_PARAMETERS pParameters);

        int SetCursorProperties();

        int SetCursorPosition();

        int ShowCursor(bool bShow);

        int CreateAdditionalSwapChain();

        int GetSwapChain();

        uint GetNumberOfSwapChains();

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        int Reset([In, Out] ref D3DPRESENT_PARAMETERS pPresentationParameters);

        int Present();

        int GetBackBuffer();

        int GetRasterStatus();

        int SetDialogBoxMode();

        int SetGammaRamp();

        int GetGammaRamp();

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        int CreateTexture(int Width, int Height, int Levels, int Usage, D3DFORMAT Format, int Pool,
                          out IDirect3DTexture9 ppTexture, IntPtr pSharedHandle);

        int CreateVolumeTexture();

        int CreateCubeTexture();

        int CreateVertexBuffer();

        int CreateIndexBuffer();

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        int CreateRenderTarget(int width, int height, D3DFORMAT Format, D3DMULTISAMPLE_TYPE MultiSample,
                                 uint MultisampleQuality, [MarshalAs(UnmanagedType.Bool)] bool Lockable, [Out]out IntPtr pSurface,
                                 IntPtr pSharedSurface);

        int CreateDepthStencilSurface();

        int UpdateSurface();

        int UpdateTexture();

        int GetRenderTargetData();

        int GetFrontBufferData();

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        int StretchRect(IntPtr pSourceSurface, DsRect pSourceRect, IDirect3DSurface9 pDestSurface, DsRect pDestRect, int Filter);

        int ColorFill();

        int CreateOffscreenPlainSurface();

        int SetRenderTarget();

        [PreserveSig, SuppressUnmanagedCodeSecurity]
        int GetRenderTarget([Out]out IntPtr pSurface);

        int SetDepthStencilSurface();

        int GetDepthStencilSurface();

        int BeginScene();

        int EndScene();

        int Clear();

        int SetTransform();

        int GetTransform();

        int MultiplyTransform();

        int SetViewport();

        int GetViewport();

        int SetMaterial();

        int GetMaterial();

        int SetLight();

        int GetLight();

        int LightEnable();

        int GetLightEnable();

        int SetClipPlane();

        int GetClipPlane();

        int SetRenderState();

        int GetRenderState();

        int CreateStateBlock();

        int BeginStateBlock();

        int EndStateBlock();

        int SetClipStatus();

        int GetClipStatus();

        int GetTexture();

        int SetTexture();

        int GetTextureStageState();

        int SetTextureStageState();

        int GetSamplerState();

        int SetSamplerState();

        int ValidateDevice();

        int SetPaletteEntries();

        int GetPaletteEntries();

        int SetCurrentTexturePalette();

        int GetCurrentTexturePalette();

        int SetScissorRect();

        int GetScissorRect();

        int SetSoftwareVertexProcessing(bool bSoftware);

        bool GetSoftwareVertexProcessing();

        int SetNPatchMode(float nSegments);

        float GetNPatchMode();

        int DrawPrimitive();

        int DrawIndexedPrimitive();

        int DrawPrimitiveUP();

        int DrawIndexedPrimitiveUP();

        int ProcessVertices();

        int CreateVertexDeclaration();

        int SetVertexDeclaration();

        int GetVertexDeclaration();

        int SetFVF();

        int GetFVF();

        int CreateVertexShader();

        int SetVertexShader();

        int GetVertexShader();

        int SetVertexShaderConstantF();

        int GetVertexShaderConstantF();

        int SetVertexShaderConstantI();

        int GetVertexShaderConstantI();

        int SetVertexShaderConstantB();

        int GetVertexShaderConstantB();

        int SetStreamSource();

        int GetStreamSource();

        int SetStreamSourceFreq();

        int GetStreamSourceFreq();

        int SetIndices();

        int GetIndices();

        int CreatePixelShader();

        int SetPixelShader();

        int GetPixelShader();

        int SetPixelShaderConstantF();

        int GetPixelShaderConstantF();

        int SetPixelShaderConstantI();

        int GetPixelShaderConstantI();

        int SetPixelShaderConstantB();

        int GetPixelShaderConstantB();

        int DrawRectPatch();

        int DrawTriPatch();

        int DeletePatch(uint Handle);

        int CreateQuery();
    }
}

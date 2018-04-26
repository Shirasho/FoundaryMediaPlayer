using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace FoundaryMediaPlayer.Platforms.Windows.DirectX
{
    [StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal struct D3DCAPS9
    {
        /* Device Info */
        public D3DDEVTYPE DeviceType;
        public uint AdapterOrdinal;

        /* Caps from DX7 Draw */
        public int Caps;
        public int Caps2;
        public int Caps3;
        public int PresentationIntervals;

        /* Cursor Caps */
        public int CursorCaps;

        /* 3D Device Caps */
        public int DevCaps;

        public int PrimitiveMiscCaps;
        public int RasterCaps;
        public int ZCmpCaps;
        public int SrcBlendCaps;
        public int DestBlendCaps;
        public int AlphaCmpCaps;
        public int ShadeCaps;
        public int TextureCaps;
        public int TextureFilterCaps;          // D3DPTFILTERCAPS for IDirect3DTexture9's
        public int CubeTextureFilterCaps;      // D3DPTFILTERCAPS for IDirect3DCubeTexture9's
        public int VolumeTextureFilterCaps;    // D3DPTFILTERCAPS for IDirect3DVolumeTexture9's
        public int TextureAddressCaps;         // D3DPTADDRESSCAPS for IDirect3DTexture9's
        public int VolumeTextureAddressCaps;   // D3DPTADDRESSCAPS for IDirect3DVolumeTexture9's

        public int LineCaps;                   // D3DLINECAPS

        public int MaxTextureWidth, MaxTextureHeight;
        public int MaxVolumeExtent;

        public int MaxTextureRepeat;
        public int MaxTextureAspectRatio;
        public int MaxAnisotropy;
        public float MaxVertexW;

        public float GuardBandLeft;
        public float GuardBandTop;
        public float GuardBandRight;
        public float GuardBandBottom;

        public float ExtentsAdjust;
        public int StencilCaps;

        public int FVFCaps;
        public int TextureOpCaps;
        public int MaxTextureBlendStages;
        public int MaxSimultaneousTextures;

        public int VertexProcessingCaps;
        public int MaxActiveLights;
        public int MaxUserClipPlanes;
        public int MaxVertexBlendMatrices;
        public int MaxVertexBlendMatrixIndex;

        public float MaxPointSize;

        public int MaxPrimitiveCount;          // max number of primitives per DrawPrimitive call
        public int MaxVertexIndex;
        public int MaxStreams;
        public int MaxStreamStride;            // max stride for SetStreamSource

        public int VertexShaderVersion;
        public int MaxVertexShaderConst;       // number of vertex shader constant registers

        public int PixelShaderVersion;
        public float PixelShader1xMaxValue;      // max value storable in registers of ps.1.x shaders

        // Here are the DX9 specific ones
        public int DevCaps2;

        public float MaxNpatchTessellationLevel;
        public int Reserved5;

        public uint MasterAdapterOrdinal;       // ordinal of master adaptor for adapter group
        public uint AdapterOrdinalInGroup;      // ordinal inside the adapter group
        public uint NumberOfAdaptersInGroup;    // number of adapters in this adapter group (only if master)
        public int DeclTypes;                  // Data types, supported in vertex declarations
        public int NumSimultaneousRTs;         // Will be at least 1
        public int StretchRectFilterCaps;      // Filter caps supported by StretchRect
        public D3DVSHADERCAPS2_0 VS20Caps;
        public D3DPSHADERCAPS2_0 PS20Caps;
        public int VertexTextureFilterCaps;    // D3DPTFILTERCAPS for IDirect3DTexture9's for texture, used in vertex shaders
        public int MaxVShaderInstructionsExecuted; // maximum number of vertex shader instructions that can be executed
        public int MaxPShaderInstructionsExecuted; // maximum number of pixel shader instructions that can be executed
        public int MaxVertexShader30InstructionSlots;
        public int MaxPixelShader30InstructionSlots;
    }
}

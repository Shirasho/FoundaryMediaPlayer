using System.Diagnostics.CodeAnalysis;

namespace FoundaryMediaPlayer.Interfaces
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum ELAVHardwareAcceleration
    {
        None,
        CUDA,
        QuickSync,
        DXVA2,
        DXVA2CopyBack = DXVA2,
        DXVA2Native
    }
}

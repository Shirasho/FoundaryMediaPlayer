using System.Diagnostics.CodeAnalysis;

namespace FoundaryMediaPlayer.Platforms.Windows.DirectX
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal enum D3DPOOL
    {
        D3DPOOL_DEFAULT      = 0,
        D3DPOOL_MANAGED      = 1,
        D3DPOOL_SYSTEMMEM    = 2,
        D3DPOOL_SCRATCH      = 3,
        D3DPOOL_FORCE_DWORD  = 0x7fffffff
    }
}

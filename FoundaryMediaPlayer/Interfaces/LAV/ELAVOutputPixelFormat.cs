using System.Diagnostics.CodeAnalysis;

namespace FoundaryMediaPlayer.Interfaces
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum ELAVOutputPixelFormat
    {
        None = -1,
        YV12,            // 4:2:0, 8bit, planar
        NV12,            // 4:2:0, 8bit, Y planar, U/V packed
        YUY2,            // 4:2:2, 8bit, packed
        UYVY,            // 4:2:2, 8bit, packed
        AYUV,            // 4:4:4, 8bit, packed
        P010,            // 4:2:0, 10bit, Y planar, U/V packed
        P210,            // 4:2:2, 10bit, Y planar, U/V packed
        Y410,            // 4:4:4, 10bit, packed
        P016,            // 4:2:0, 16bit, Y planar, U/V packed
        P216,            // 4:2:2, 16bit, Y planar, U/V packed
        Y416,            // 4:4:4, 16bit, packed
        RGB32,           // 32-bit RGB (BGRA)
        RGB24,           // 24-bit RGB (BGR)
        v210,            // 4:2:2, 10bit, packed
        v410,            // 4:4:4, 10bit, packed

        YV16,            // 4:2:2, 8-bit, planar
        YV24,

        COUNT = YV24 + 1 // Number of formats
    }
}

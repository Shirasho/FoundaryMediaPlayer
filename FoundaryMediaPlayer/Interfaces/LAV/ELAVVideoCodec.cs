using System.Diagnostics.CodeAnalysis;

namespace FoundaryMediaPlayer.Interfaces
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum ELAVVideoCodec
    {
        H264 = 1,
        VC1,
        MPEG1,
        MPEG2,
        MPEG4,
        MSMPEG4,
        VP8,
        WMV3,
        WMV12,
        MJPEG,
        Theora,
        FLV1,
        VP6,
        SVQ,
        H261,
        H263,
        Indeo,
        TSCC,
        Fraps,
        HuffYUV,
        QTRle,
        DV,
        Bink,
        Smacker,
        RV12,
        RV34,
        Lagarith,
        Cinepak,
        Camstudio,
        QPEG,
        ZLIB,
        QTRpza,
        PNG,
        MSRLE,
        ProRes,
        UtVideo,
        Dirac,
        DNxHD,
        MSVideo1,
        EightBPS,
        LOCO,
        ZMBV,
        VCR1,
        Snow,
        FFV1,
        v210,

        COUNT = v210
    }
}

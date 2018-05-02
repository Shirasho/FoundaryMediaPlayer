using System.Diagnostics.CodeAnalysis;

namespace FoundaryMediaPlayer.Interfaces
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum ELAVVideoHardwareCodec
    {
        H264 = ELAVVideoCodec.H264,
        VC1 = ELAVVideoCodec.VC1,
        MPEG2DVD,
        MPEG2 = ELAVVideoCodec.MPEG2,
        MPEG4 = ELAVVideoCodec.MPEG4,

        COUNT = MPEG4
    }
}

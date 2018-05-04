using System;
using System.Diagnostics.CodeAnalysis;

namespace FoundaryMediaPlayer.Interop.Windows
{
    /// <summary>
    /// IIDs.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class IID
    {
        public static Guid None { get; } = Guid.Empty;

        public const string CLSID_Proxy = "17CCA71B-ECD7-11D0-B908-00A0C9223196";
        public static Guid Proxy { get; } = new Guid(CLSID_Proxy);
        
        public const string CLSID_NetShowSource = "6B6D0800-9ADA-11d0-A520-00A0D10129C0";
        public static Guid NetShowSource { get; } = new Guid(CLSID_NetShowSource);

        public const string CLSID_Haali = "55DA30FC-F16B-49FC-BAA5-AE59FC65F82D";
        public static Guid Haali { get; } = new Guid(CLSID_Haali);

        public const string CLSID_LAVFilter = "171252A0-8820-4AFE-9DF8-5C92B2D66B04";
        public static Guid LAVFilter { get; } = new Guid(CLSID_LAVFilter);

        public const string CLSID_LAVSplitterSource = "B98D13E7-55DB-4385-A33D-09FD1BA26338";
        public static Guid LAVSplitterSource { get; } = new Guid(CLSID_LAVSplitterSource);

        public const string CLSID_LAVVideo = "EE30215D-164F-4A92-A4EB-9D4C13390F9F";
        public static Guid LAVVideo { get; } = new Guid(CLSID_LAVVideo);

        public const string CLSID_LAVAudio = "E8E73B6B-4CB3-44A4-BE99-4F7BCB96E491";
        public static Guid LAVAudio { get; } = new Guid(CLSID_LAVAudio);

        public const string CLSID_FFDShowVideoDecoder = "04FE9017-F873-410E-871E-AB91661A4EF7";
        public static Guid FFDShowVideoDecoder { get; } = new Guid(CLSID_FFDShowVideoDecoder);

        public const string CLSID_MadVR = "E1A8B82A-32CE-4B0D-BE0D-AA68C772E423";
        public static Guid MadVR { get; } = new Guid(CLSID_MadVR);

        public const string CLSID_MadVRAllocatorPresenter = "C7ED3100-9002-4595-9DCA-B30B30413429";
        public static Guid MadVRAllocatorPresenter { get; } = new Guid(CLSID_MadVRAllocatorPresenter);

        public const string CLSID_Sync = "FA10746C-9B63-4B6C-BC49-FC300EA5F256";
        public static Guid Sync { get; } = new Guid(CLSID_Sync);

        public const string CLSID_ASSFilter = "8A3704F3-BE3B-4944-9FF3-EE8757FDBDA5";
        public static Guid ASSFilter { get; } = new Guid(CLSID_ASSFilter);

        public const string CLSID_VSFilter = "9852A670-F845-491B-9BE6-EBD841B8A613";
        public static Guid VSFilter { get; } = new Guid(CLSID_VSFilter);

        public const string CLSID_XYVSFilter = "2DFCB782-EC20-4A7C-B530-4577ADB33F21";
        public static Guid XYVSFilter { get; } = new Guid(CLSID_XYVSFilter);

        public const string CLSID_XYSubFilter = "2DFCB782-EC20-4A7C-B530-4577ADB33F21";
        public static Guid XYSubFilter { get; } = new Guid(CLSID_XYSubFilter);

        public const string CLSID_AudioRecord = "E30629D2-27E5-11CE-875D-00608CB78066";
        public static Guid AudioRecord { get; } = new Guid(CLSID_AudioRecord);

        public const string CLSID_CMPEG2VideoDecoderDS = "212690FB-83E5-4526-8FD7-74478B7939CD";
        public static Guid CMPEG2VideoDecoderDS { get; } = new Guid(CLSID_CMPEG2VideoDecoderDS);

        public const string CLSID_CMPEG2DecoderFilter = "39F498AF-1A09-4275-B193-673B0BA3D478";
        public static Guid CMPEG2DecoderFilter { get; } = new Guid(CLSID_CMPEG2DecoderFilter);

        public const string CLSID_NvidiaVideoDecoder = "71E4616A-DB5E-452B-8CA5-71D9CC7805E9";
        public static Guid NvidiaVideoDecoder { get; } = new Guid(CLSID_NvidiaVideoDecoder);

        public const string CLSID_SonicCinemasterVideoDecoder = "D7D50E8D-DD72-43C2-8587-A0C197D837D2";
        public static Guid SonicCinemasterVideoDecoder { get; } = new Guid(CLSID_SonicCinemasterVideoDecoder);


        public const string IID_AsyncReader = "E436EBB5-524F-11CE-9F53-0020AF0BA770";
        public static Guid AsyncReader { get; } = new Guid(IID_AsyncReader);

        
        public const string IID_TIMEFORMAT_MediaTime = "7B785574-8C82-11CF-BC0C-00AA00AC74F6";
        public static Guid TIMEFORMAT_MediaTime { get; } = new Guid(IID_TIMEFORMAT_MediaTime);


        public const string IID_MEDIATYPE_Stream = "E436EB83-524F-11CE-9F53-0020AF0BA770";
        public static Guid MEDIATYPE_Stream { get; } = new Guid(IID_MEDIATYPE_Stream);


        public const string IID_SERVICE_MRVideoAcceleration = "EFEF5175-5C7D-4CE2-BBBD-34FF8BCA6554";
        public static Guid SERVICE_MRVideoAcceleration { get; } = new Guid(IID_SERVICE_MRVideoAcceleration);

        public const string IID_SERVICE_MRVideoMixer = "073CD2FC-6CF4-40B7-8859-E89552C841F8";
        public static Guid SERVICE_MRVideoMixer { get; } = new Guid(IID_SERVICE_MRVideoMixer);

        private const string CLSID_VMR9 = "51B4ABF3-748F-4E3B-A276-C828330E926A";
    }
}

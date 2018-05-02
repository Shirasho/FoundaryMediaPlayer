using System;
using System.Diagnostics.CodeAnalysis;

namespace FoundaryMediaPlayer.Interop.Windows
{
    /// <summary>
    /// CLSIDs.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class IID
    {
        public const string CLSID_IUnknown = "00000000-0000-0000-C000-000000000046";
        public static Guid IUnknown { get; } = new Guid(CLSID_IUnknown);

        public const string CLSID_IMixerPinConfig = "593CDDE1-0759-11d1-9E69-00C04FD7C15B";
        public static Guid IMixerPinConfig { get; } = new Guid(CLSID_IMixerPinConfig);

        public const string CLSID_IMixerPinConfig2 = "EBF47182-8764-11d1-9E69-00C04FD7C15B";
        public static Guid IMixerPinConfig2 { get; } = new Guid(CLSID_IMixerPinConfig);

        public const string CLSID_Proxy = "17CCA71B-ECD7-11D0-B908-00A0C9223196";
        public static Guid Proxy { get; } = new Guid(CLSID_Proxy);

        public const string CLSID_FilterGraph = "E436EBB3-524F-11CE-9F53-0020AF0BA770";
        public static Guid FilterGraph { get; } = new Guid(CLSID_FilterGraph);

        public const string CLSID_FilterMapper2 = "CDA42200-BD88-11d0-BD4E-00A0C911CE86";
        public static Guid FilterMapper2 { get; } = new Guid(CLSID_FilterMapper2);

        public const string CLSID_EnhancedVideoRenderer = "FA10746C-9B63-4B6C-BC49-FC300EA5F256";
        public static Guid EnhancedVideoRenderer { get; } = new Guid(CLSID_EnhancedVideoRenderer);
        public static Guid EnhancedVideoRendererCustom { get; } = EnhancedVideoRenderer;

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

        
        public const string IID_TIME_FORMAT_MEDIA_TIME = "7B785574-8C82-11CF-BC0C-00AA00AC74F6";
        public static Guid TIME_FORMAT_MEDIA_TIME { get; } = new Guid(IID_TIME_FORMAT_MEDIA_TIME);

        private const string CLSID_VMR9 = "51B4ABF3-748F-4E3B-A276-C828330E926A";
    }
}

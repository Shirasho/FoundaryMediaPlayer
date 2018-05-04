using System;
using System.Diagnostics.CodeAnalysis;

namespace FoundaryMediaPlayer.Interop.Windows
{
    /// <summary>
    /// CLSIDs.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class CLSID
    {
        public static Guid None { get; } = Guid.Empty;

        public static Guid IUnknown { get; } = new Guid(Strings.IUnknown);

        public static Guid IMixerPinConfig { get; } = new Guid(Strings.IMixerPinConfig);
        
        public static Guid IMixerPinConfig2 { get; } = new Guid(Strings.IMixerPinConfig2);
        
        public static Guid FilterGraph { get; } = new Guid(Strings.FilterGraph);
        
        public static Guid FilterMapper2 { get; } = new Guid(Strings.FilterMapper2);
        
        public static Guid EnhancedVideoRenderer { get; } = new Guid(Strings.EnhancedVideoRenderer);

        public static Guid EnhancedVideoRendererCustom { get; } = new Guid(Strings.EnhancedVideoRenderer);
        
        public static Guid SystemDeviceEnum { get; } = new Guid(0x62BE5D10, 0x60EB, 0x11d0, 0xBD, 0x3B, 0x00, 0xA0, 0xC9, 0x11, 0xCE, 0x86);
        
        public static Guid CaptureGraphBuilder2 { get; } = new Guid(0xBF87B6E1, 0x8C27, 0x11d0, 0xB3, 0xF0, 0x0, 0xAA, 0x00, 0x37, 0x61, 0xC5);
        
        public static Guid SampleGrabber { get; } = new Guid(0xC1F400A0, 0x3F08, 0x11D3, 0x9F, 0x0B, 0x00, 0x60, 0x08, 0x03, 0x9E, 0x37);
        
        public static Guid DvdGraphBuilder { get; } = new Guid(0xFCC152B7, 0xF372, 0x11d0, 0x8E, 0x00, 0x00, 0xC0, 0x4F, 0xD7, 0xC0, 0x8B);

        [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
        public static class Strings
        {
            public const string IUnknown = "00000000-0000-0000-C000-000000000046";

            public const string IMixerPinConfig = "593CDDE1-0759-11d1-9E69-00C04FD7C15B";

            public const string IMixerPinConfig2 = "EBF47182-8764-11d1-9E69-00C04FD7C15B";
            
            public const string FilterGraph = "E436EBB3-524F-11CE-9F53-0020AF0BA770";

            public const string FilterMapper2 = "CDA42200-BD88-11d0-BD4E-00A0C911CE86";
            
            public const string EnhancedVideoRenderer = "FA10746C-9B63-4B6C-BC49-FC300EA5F256";
        }
    }
}

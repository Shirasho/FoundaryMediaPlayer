using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using FoundaryMediaPlayer.Engine;

namespace FoundaryMediaPlayer.Platforms.Windows
{
    /// <summary>
    /// CLSIDs.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class CLSID
    {
        public const string CLSID_IUnknown = "00000000-0000-0000-C000-000000000046";
        public static Guid IUnknown { get; } = new Guid(CLSID_IUnknown);

        public const string CLSID_Proxy = "17CCA71B-ECD7-11D0-B908-00A0C9223196";
        public static Guid Proxy { get; } = new Guid(CLSID_Proxy);

        public const string CLSID_FilterGraph = "E436EBB3-524F-11CE-9F53-0020AF0BA770";
        public static Guid FilterGraph { get; } = new Guid(CLSID_FilterGraph);

        public const string CLSID_FilterMapper2 = "CDA42200-BD88-11d0-BD4E-00A0C911CE86";
        public static Guid FilterMapper2 = new Guid(CLSID_FilterMapper2);

        public const string CLSID_EnhancedVideoRenderer = "FA10746C-9B63-4B6C-BC49-FC300EA5F256";
        public static Guid EnhancedVideoRenderer { get; } = new Guid(CLSID_EnhancedVideoRenderer);

        public const string CLSID_Haali = "55DA30FC-F16B-49FC-BAA5-AE59FC65F82D";
        public static Guid Haali { get; } = new Guid(CLSID_Haali);

        public const string CLSID_LAVFilter = "171252A0-8820-4AFE-9DF8-5C92B2D66B04";
        public static Guid LAVFilter { get; } = new Guid(CLSID_LAVFilter);

        public const string CLSID_MadVR = "E1A8B82A-32CE-4B0D-BE0D-AA68C772E423";
        public static Guid MadVR { get; } = new Guid(CLSID_MadVR);

        public const string CLSID_Sync = "FA10746C-9B63-4B6C-BC49-FC300EA5F256";
        public static Guid Sync { get; } = new Guid(CLSID_Sync);


        /// <summary>
        /// ASSFilter
        /// </summary>
        public static Guid SFILTER_Ass { get; } = Guid.Parse("8A3704F3-BE3B-4944-9FF3-EE8757FDBDA5");

        /// <summary>
        /// VS Filter
        /// </summary>
        public static Guid SFILTER_VS { get; } = Guid.Parse("9852A670-F845-491B-9BE6-EBD841B8A613");

        /// <summary>
        /// XYSubFilter
        /// </summary>
        public static Guid SFILTER_XYSub { get; } = Guid.Parse("2DFCB782-EC20-4A7C-B530-4577ADB33F21");

        /// <summary>
        /// LAV Filter Source
        /// </summary>
        public static Guid X_LAVFilterSource { get; } = Guid.Parse("B98D13E7-55DB-4385-A33D-09FD1BA26338");

        /// <summary>
        /// MPC Matroska
        /// </summary>
        public static Guid MPCMatroska { get; } = Guid.Parse("149D2E01-C32E-4939-80F6-C07B81015A7A");

        /// <summary>
        /// MPC Matroska Source
        /// </summary>
        public static Guid MPCMatroskaSource { get; } = Guid.Parse("0A68C3B5-9164-4A54-AFAF-995B2FF0E0D4");

        /// <summary>
        /// VMR9 Renderless
        /// </summary>
        public static Guid VFILTER_VMR9Renderless { get; } = Guid.Parse("51B4ABF3-748F-4E3B-A276-C828330E926A");

        /// <summary>
        /// VMR9 Windowed
        /// </summary>
        public static Guid VFILTER_VMR9Windowed { get; } = Guid.Parse("51B4ABF3-748F-4E3B-A276-C828330E926A");

        /// <summary>
        /// Null Renderless
        /// </summary>
        // Is this a Windows null renderer or an application null renderer?
        public static Guid VFILTER_Null { get; } = Guid.Parse("C1F400A4-3F08-11D3-9F0B-006008039E37");

        /// <summary>
        /// EVRCustom filter
        /// </summary>
        public static Guid VFILTER_EVRCustom { get; } = Guid.Parse("FA10746C-9B63-4B6C-BC49-FC300EA5F256");



        /// <summary>
        /// 
        /// </summary>
        public static Guid TIME_FORMAT_MEDIA_TIME { get; } = Guid.Parse("7B785574-8C82-11CF-BC0C-00AA00AC74F6");

        private CLSID() { }

        /// <summary>
        /// Gets the CLSID of the specified class.
        /// </summary>
        /// <typeparam name="TClass"></typeparam>
        /// <returns></returns>
        public static Guid GetCLSIDOf<TClass>()
        {
            var type = typeof(TClass);
            var attribute = typeof(TClass).GetCustomAttribute<GuidAttribute>();
            return attribute != null ? Guid.Parse(attribute.Value) : type.GUID;
        }

        /// <summary>
        /// Gets the CLSID for the specified subtitle renderer.
        /// </summary>
        /// <param name="renderer"></param>
        /// <returns></returns>
        public static Guid GetCLSID(ESubtitleRenderer renderer)
        {
            switch (renderer)
            {
                case ESubtitleRenderer.ASSFilter: return SFILTER_Ass;
                case ESubtitleRenderer.VSFilter: return SFILTER_VS;
                //6B237877-902B-4C6C-92F6-E63169A5166C XYSubFilterAutoLoader
                case ESubtitleRenderer.XYSubFilter: return SFILTER_XYSub;
                case ESubtitleRenderer.Internal: return Guid.Empty;
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Gets the CLSID for the specified video renderer.
        /// </summary>
        /// <param name="renderer"></param>
        /// <returns></returns>
        public static Guid GetCLSID(EVideoRenderer renderer)
        {
            switch (renderer)
            {
                case EVideoRenderer.EVR: return EnhancedVideoRenderer;
                case EVideoRenderer.MadVR: return MadVR;
                case EVideoRenderer.VMR9Renderless: return VFILTER_VMR9Renderless;
                case EVideoRenderer.VMR9Windowed: return VFILTER_VMR9Windowed;
                case EVideoRenderer.Null: return VFILTER_Null;
                case EVideoRenderer.EVRCustom: return VFILTER_EVRCustom;
                case EVideoRenderer.Sync: return Sync;
            }

            return Guid.Empty;
        }
    }
}

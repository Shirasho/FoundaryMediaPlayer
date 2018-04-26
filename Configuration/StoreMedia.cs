using System;
using FoundaryMediaPlayer.Engine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Prism.Mvvm;

namespace FoundaryMediaPlayer.Configuration
{
    /// <summary>
    /// Information regarding media metadata.
    /// </summary>
    [Serializable]
    public sealed class StoreMedia : BindableBase
    {
        private ESubtitleRenderer _SubtitleRenderer = ESubtitleRenderer.Internal;
        private EVideoRenderer _VideoRenderer = EVideoRenderer.Default;
        private EEngineType _RtspEngine = EEngineType.RealMedia;
        private bool _RtspFileExtensionFirst;
        private ECaptureDevice _DefaultCaptureDevice;

        /// <summary>
        /// The RTSP engine.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public EEngineType RtspEngine
        {
            get => _RtspEngine;
            set => SetProperty(ref _RtspEngine, value);
        }

        /// <summary>
        /// The subtitle renderer to attempt to use. This is not necessarily
        /// the renderer that will be used.
        /// </summary>
        public ESubtitleRenderer SubtitleRenderer
        {
            get => _SubtitleRenderer;
            set => SetProperty(ref _SubtitleRenderer, value);
        }

        /// <summary>
        /// The video renderer to attempt to use. This is not necessarily
        /// the renderer that will be used.
        /// </summary>
        public EVideoRenderer VideoRenderer
        {
            get => _VideoRenderer;
            set => SetProperty(ref _VideoRenderer, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool RtspFileExtensionFirst
        {
            get => _RtspFileExtensionFirst;
            set => SetProperty(ref _RtspFileExtensionFirst, value);
        }

        /// <summary>
        /// The default capture device.
        /// </summary>
        public ECaptureDevice DefaultCaptureDevice
        {
            get => _DefaultCaptureDevice;
            set => SetProperty(ref _DefaultCaptureDevice, value);
        }
    }
}

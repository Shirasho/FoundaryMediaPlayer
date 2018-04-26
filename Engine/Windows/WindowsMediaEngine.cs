using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using DirectShowLib;
using FluentAssertions;
using Foundary;
using FoundaryMediaPlayer.Configuration;
using FoundaryMediaPlayer.Platforms.Windows;
using Prism.Events;

using HResult = PInvoke.HResult.Code;

namespace FoundaryMediaPlayer.Engine.Windows
{
    /// <summary>
    /// A Windows <see cref="IMediaEngine"/>.
    /// </summary>
    public sealed class WindowsMediaEngine : MediaEngineBase
    {
        /// <summary>
        /// The DXVA version.
        /// </summary>
        private int _DXVAVersion { get; set; }

        /// <summary>
        /// The DXVA decoder <see cref="Guid"/>.
        /// </summary>
        private Guid _DXVADecoderGuid { get; set; }

        /// <summary>
        /// Whether to start in D3D fullscreen.
        /// </summary>
        private bool _bStartInD3DFullscreen { get; set; }

        private IGraphBuilder2 _GraphBuilder { get; set; }
        private IMediaControl _MediaControl { get; set; }
        private IDSMChapterBag _ChapterBag { get; set; }
        private IVideoFrameStep _VideoFrameStep { get; set; }
        private IBasicAudio _BasicAudio { get; set; }
        private IBasicVideo _BasicVideo { get; set; }
        private IVideoWindow _VideoWindow { get; set; }
        private IMediaSeeking _MediaSeeking { get; set; }
        private IMediaEventEx _MediaEvent { get; set; }

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public WindowsMediaEngine(
            Store store,
            CLOptions options,
            MediaFormatCollection mediaFormats,
            IEventAggregator eventAggregator)
            : base(store, options, mediaFormats, eventAggregator)
        {

        }

        /// <inheritdoc />
        public override ESubtitleRenderer GetSubtitleRenderer()
        {
            var renderer = Store.Media.VideoRenderer;

            if (IsSubtitleRendererSupported(ESubtitleRenderer.Internal, renderer) ||
                IsSubtitleRendererSupported(ESubtitleRenderer.XYSubFilter, renderer) ||
                IsSubtitleRendererSupported(ESubtitleRenderer.ASSFilter, renderer))
            {
                return Store.Media.SubtitleRenderer;
            }

            return ESubtitleRenderer.VSFilter;
        }

        /// <inheritdoc />
        public override EVideoRenderer GetVideoRenderer()
        {
            return Store.Media.VideoRenderer;
        }

        /// <inheritdoc />
        public override bool IsSubtitleRendererRegistered(ESubtitleRenderer renderer)
        {
            switch (renderer)
            {
                case ESubtitleRenderer.Internal:
                    return true;
                case ESubtitleRenderer.VSFilter:
                case ESubtitleRenderer.XYSubFilter:
                case ESubtitleRenderer.ASSFilter:
                    return Native.IsActiveMovieCLSIDRegistered(CLSID.GetCLSID(renderer));
                default:
                    return false;
            }
        }

        /// <inheritdoc />
        public override bool IsVideoRendererRegistered(EVideoRenderer renderer)
        {
            switch (renderer)
            {
                case EVideoRenderer.Default:
                    return true;
                case EVideoRenderer.EVR:
                case EVideoRenderer.EVRCustom:
                case EVideoRenderer.MadVR:
                case EVideoRenderer.Null:
                case EVideoRenderer.Sync:
                case EVideoRenderer.VMR9Renderless:
                case EVideoRenderer.VMR9Windowed:
                    return Native.IsActiveMovieCLSIDRegistered(CLSID.GetCLSID(renderer));
                default:
                    return false;
            }
        }

        /// <inheritdoc />
        public override bool IsSubtitleRendererSupported(ESubtitleRenderer subtitleRenderer, EVideoRenderer videoRenderer)
        {
            switch (subtitleRenderer)
            {
                case ESubtitleRenderer.Internal:
                    switch (videoRenderer)
                    {
                        case EVideoRenderer.VMR9Renderless:
                        case EVideoRenderer.EVRCustom:
                        case EVideoRenderer.Sync:
                        case EVideoRenderer.MadVR:
                            return true;
                    }

                    break;
                case ESubtitleRenderer.VSFilter:
                    return true;
                case ESubtitleRenderer.XYSubFilter:
                case ESubtitleRenderer.ASSFilter:
                    switch (videoRenderer)
                    {
                        case EVideoRenderer.VMR9Renderless:
                        case EVideoRenderer.EVRCustom:
                        case EVideoRenderer.Sync:
                        case EVideoRenderer.MadVR:
                            return true;
                    }

                    break;
            }

            return false;
        }

        /// <summary>
        /// Whether the video renderer is D3D fullscreen.
        /// </summary>
        /// <returns></returns>
        public bool IsD3DFullscreen()
        {
            switch (Store.Media.VideoRenderer)
            {
                case EVideoRenderer.VMR9Renderless:
                case EVideoRenderer.EVRCustom:
                case EVideoRenderer.Sync:
                    return Store.Player.bFullscreen || (Options.Fullscreen ?? false);
                default:
                    return false;
            }
        }

        /// <inheritdoc />
        protected override void OnSetVolume(float newVolume)
        {
            //TODO: Set volume.
        }

        /// <inheritdoc />
        protected override MediaData GetMediaDataFromPlaylistItem(PlaylistItem item, TimeSpan startTime)
        {
            if (item == null)
            {
                return null;
            }

            MediaStream = item.File.OpenRead();

            // DVDs will contain "video_ts.ifo" somewhere in the filename.
            if (item.File.FullName.ToLowerInvariant().Contains("video_ts.ifo"))
            {
                return new WindowsMediaDvdData(item.File, null);
            }

            if (item.Type == EPlaylistItemType.Device)
            {
                return new WindowsMediaDeviceData
                // ReSharper disable once RedundantEmptyObjectOrCollectionInitializer
                {
                    //TODO: Figure out how to port this.
                    //VideoInput = item.VideoInput,
                    //VideoChannel = item.VideoChannel,
                    //AudioInput = item.AudioInput
                };
            }

            return new MediaFileData(item.File, item.Subs)
            {
                StartTime = startTime
            };
        }

        /// <inheritdoc />
        protected override void OnAbortGraph()
        {
            var abortTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            Task.Run(() => _GraphBuilder?.Abort(), abortTokenSource.Token).Wait(abortTokenSource.Token);
            if (abortTokenSource.IsCancellationRequested)
            {
                //TODO: Destroy the graph builder and make a new one.
                //_GraphBuilder = new IGraphBuilder2();
            }
        }

        /// <inheritdoc />
        protected override void OnStopMedia()
        {
            // TODO: 6997
        }

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "UsePatternMatching")]
        protected override void OnOpenMedia(MediaData data)
        {
            CastData(data, out var fileData, out var dvdData, out var deviceData);

            if (deviceData != null)
            {
                //TODO: deviceData 14593
                return;
            }

            // Recheck that the file is still valid.
            if (fileData != null)
            {
                //TODO: We want to display a window instead of flat asserting.
                fileData.FirstFile.Should().NotBeNull();
                fileData.FirstFile.Exists.Should().BeTrue();
            }

            // Set the window to fullscreen if we are requesting it.
            if (IsD3DFullscreen() && _bStartInD3DFullscreen)
            {
                ToggleFullscreen(true);
                _bStartInD3DFullscreen = false;
            }
            else
            {
                ToggleFullscreen(false);
            }


            //_bDelayOutputRect = true;

            ClearDXVAState();

            try
            {
                OpenMediaTokenSource.Check();

                // 11801
                OpenCreateGraphObject(data);
            }
        }

        // ReSharper disable RedundantOverriddenMember
        /// <inheritdoc />
        protected override void OnPreCloseMedia()
        {
            base.OnPreCloseMedia();

            //TODO: 14788
            //SubtitlesProvider.Abort();
        }
        // ReSharper restore RedundantOverriddenMember

        /// <inheritdoc />
        protected override void OnCloseMedia()
        {
            //return Task.Run(() =>
            {
                _MediaControl?.Stop();

                _GraphBuilder?.RemoveFromROT();
                Native.CoRelease(_GraphBuilder); _GraphBuilder = null;
            }//);
        }

        private void OpenCreateGraphObject(MediaData data)
        {
            Engine = EEngineType.None;

            CastData(data, out var fileData, out var dvdData, out var deviceData);

            if (fileData != null)
            {
                Engine = MediaFormats.GetEngine(fileData.FirstFile);
#if OVERRIDE_RM_QT_ENGINE
                if (engine == EEngineType.RealMedia || engine == EEngineType.QuickTime)
                {
                    engine = EEngineType.Custom;
                }
#endif
                if (Engine == EEngineType.RealMedia)
                {
                    _GraphBuilder = new RealMediaGraph();
                }
                else if (Engine == EEngineType.Shockwave)
                {
                    _GraphBuilder = new ShockwaveGraph();
                }
                else if (Engine == EEngineType.QuickTime)
                {
                    _GraphBuilder = new QuickTimeGraph();
                }
                else
                {
                    _GraphBuilder = new DirectShowGraph();
                }
            }
            else if (dvdData != null)
            {
                //_GraphBuilder = new DvdGraph();
            }
            else if (deviceData != null)
            {
                if (Store.Media.DefaultCaptureDevice == ECaptureDevice.Digital)
                {
                    //_GraphBuilder = new BDAGraph();
                }
                else
                {
                    //_GraphBuilder = new CaptureGraph();
                }
            }

            _GraphBuilder.Should().NotBeNull();

            _GraphBuilder.AddToROT();

            _MediaControl = _GraphBuilder as IMediaControl;
            _MediaEvent = _GraphBuilder as IMediaEventEx;
            _MediaSeeking = _GraphBuilder as IMediaSeeking;
            _VideoWindow = _GraphBuilder as IVideoWindow;
            _BasicVideo = _GraphBuilder as IBasicVideo;
            _BasicAudio = _GraphBuilder as IBasicAudio;
            _VideoFrameStep = _GraphBuilder as IVideoFrameStep;

            if (!(_MediaControl != null && _MediaEvent != null && _MediaSeeking != null) ||
                !(_VideoWindow != null && _BasicVideo != null) ||
                _BasicAudio == null)
            {
                throw new RuntimeException("Graph interfaces were not implemented correctly.");
            }

            // The number here seems arbitrary.
            if (_MediaEvent?.SetNotifyWindow(hWnd, 32771, IntPtr.Zero) < HResult.S_OK)
            {
                throw new RuntimeException("Error setting notify window for media event.");
            }

            // 10435
            //_Prov = new KeyProvider();
            if (_GraphBuilder is IObjectWithSite site)
            {
                //site.SetSite(_Prov);
            }

            _ChapterBag = new DSMChapterBag();
        }

        private void ClearDXVAState()
        {
            _DXVADecoderGuid = Guid.Empty;
            _DXVAVersion = 0;
        }

        /// <summary>
        /// <see cref="MediaData"/> initialization/casting utility.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="file"></param>
        /// <param name="dvd"></param>
        /// <param name="device"></param>
        private void CastData(MediaData data, out MediaFileData file, out WindowsMediaDvdData dvd, out WindowsMediaDeviceData device)
        {
            file = data as MediaFileData;
            dvd = data as WindowsMediaDvdData;
            device = data as WindowsMediaDeviceData;
        }
    }
}

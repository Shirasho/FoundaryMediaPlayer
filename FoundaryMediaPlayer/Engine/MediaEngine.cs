using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using DirectShowLib;
using FluentAssertions;
using FoundaryMediaPlayer.Application;
using FoundaryMediaPlayer.Application.Components;
using FoundaryMediaPlayer.Events;
using FoundaryMediaPlayer.Interfaces;
using FoundaryMediaPlayer.Interop.Windows;
using FoundaryMediaPlayer.Windows;
using log4net;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace FoundaryMediaPlayer.Engine
{
    public class FMediaEngine : BindableBase, IMediaEngine
    {
        /// <summary>
        /// The media engine logger.
        /// </summary>
        protected static ILog Logger { get; } = LogManager.GetLogger(typeof(FMediaEngine));

        private EMediaLoadState _MediaLoadState = EMediaLoadState.Closed;
        private EEngineType _Engine = EEngineType.None;
        private EPlaybackMode _PlaybackMode = EPlaybackMode.None;
        private ICommand _SetVolumeCommand;

        /// <inheritdoc />
        public IReadOnlyPlaylist Playlist => PlaylistInternal;

        /// <inheritdoc />
        public IReadOnlyMediaFormatCollection MediaFormats { get; }

        /// <inheritdoc />
        public EMediaLoadState MediaLoadState
        {
            get => _MediaLoadState;
            protected set => SetProperty(ref _MediaLoadState, value);
        }

        /// <inheritdoc />
        public EEngineType Engine
        {
            get => _Engine;
            protected set => SetProperty(ref _Engine, value);
        }

        /// <inheritdoc />
        public EPlaybackMode PlaybackMode
        {
            get => _PlaybackMode;
            protected set => SetProperty(ref _PlaybackMode, value);
        }

        /// <inheritdoc />
        public ICommand SetVolumeCommand
        {
            get => _SetVolumeCommand ?? (SetVolumeCommand = new DelegateCommand<SliderDragDeltaEventArgs>(
                       e =>DispatchEvent(new FVolumeChangeRequestEvent((int)e.HorizontalChange)
                       {
                           NumberType = EPercentNumberType.NonNormalized,
                           ValueType = EValueType.Offset
                       })));
            set => SetProperty(ref _SetVolumeCommand, value);
        }

        /// <summary>
        /// The application store.
        /// </summary>
        protected FApplicationStore Store { get; }

        /// <summary>
        /// The command line options.
        /// </summary>
        protected FStartupCommandLineOptions CommandLineOptions { get; }

        /// <summary>
        /// The cancellation token to use for aborting opening a file.
        /// </summary>
        protected CancellationTokenSource OpenMediaTokenSource { get; set; } = new CancellationTokenSource();

        /// <summary>
        /// The owner window (the Shell).
        /// </summary>
        protected AMediaWindowBase OwnerWindow { get; set; }

        /// <summary>
        /// The current output window.
        /// </summary>
        protected AMediaWindowBase OutputWindow { get; set; }

        protected FPlaylist PlaylistInternal { get; }

        /// <summary>
        /// The application event aggregator.
        /// </summary>
        private IEventAggregator _EventAggregator { get; }

        private IGraphBuilder2 _GraphBuilder { get; set; }
        private IMediaControl _MediaControl { get; set; }
        private IDSMChapterBag _ChapterBag { get; set; }
        private IVideoFrameStep _VideoFrameStep { get; set; }
        private IBasicAudio _BasicAudio { get; set; }
        private IBasicVideo _BasicVideo { get; set; }
        private IVideoWindow _VideoWindow { get; set; }
        private IMediaSeeking _MediaSeeking { get; set; }
        private IMediaEventEx _MediaEvent { get; set; }

        /// <summary>
        /// Whether to start in D3D fullscreen.
        /// </summary>
        private bool _bStartInD3DFullscreen { get; set; }

        /// <summary>
        /// The DXVA version.
        /// </summary>
        private int _DXVAVersion { get; set; }

        /// <summary>
        /// The DXVA decoder <see cref="Guid"/>.
        /// </summary>
        private Guid _DXVADecoderGuid { get; set; }

        public FMediaEngine(
            IEventAggregator eventAggregator,
            IReadOnlyMediaFormatCollection formats,
            FApplicationStore store,
            FStartupCommandLineOptions commandLineOptions
        )
        {
            eventAggregator.Should().NotBeNull();
            formats.Should().NotBeNullOrEmpty();
            store.Should().NotBeNull();
            commandLineOptions.Should().NotBeNull();

            _EventAggregator = eventAggregator;
            MediaFormats = formats;
            Store = store;
            CommandLineOptions = commandLineOptions;

            PlaylistInternal = new FPlaylist(MediaFormats);

            SubscribeEvent<FClearPlaylistRequestEvent>(ClearPlaylistRequestEvent);
            SubscribeEvent<FAddFilesToPlaylistRequestEvent>(AddFilesToPlaylistRequestEvent);
            SubscribeEvent<FOpenMediaRequestEvent>(OpenMediaRequestEvent);
            SubscribeEvent<FCloseMediaRequestEvent>(CloseMediaRequestEvent);

            SubscribeEvent<FVolumeChangeRequestEvent>(VolumeChangedRequestEvent);
        }

        public void Dispose()
        {
            CloseMedia();

            OnDispose();

            OutputWindow.DisposeMedia();
            if (OutputWindow is FullscreenWindow)
            {
                OutputWindow.Close();
                OutputWindow = null;
            }
        }

        /// <summary>
        /// Override to dispose of custom resources.
        /// </summary>
        protected virtual void OnDispose()
        {

        }

        /// <summary>
        /// Dispatches the payload to an event.
        /// </summary>
        /// <typeparam name="TEventType">The event type.</typeparam>
        /// <param name="payload">The payload.</param>
        protected void DispatchEvent<TEventType>(TEventType payload)
            where TEventType : PubSubEvent<TEventType>, new()
        {
            _EventAggregator?.GetEvent<TEventType>().Publish(payload);
        }

        /// <summary>
        /// Subscribes to an event.
        /// </summary>
        protected void SubscribeEvent<TEventType>(Action<TEventType> callback)
            where TEventType : PubSubEvent<TEventType>, new()
        {
            _EventAggregator?.GetEvent<TEventType>().Subscribe(callback);
        }

        /// <summary>
        /// Unsubscribes from an event.
        /// </summary>
        protected void UnsubscribeEvent<TEventType>(Action<TEventType> callback)
            where TEventType : PubSubEvent<TEventType>, new()
        {
            _EventAggregator?.GetEvent<TEventType>().Unsubscribe(callback);
        }

        /// <inheritdoc />
        public void ToggleFullscreen(bool? bFullscreen = null)
        {
            //TODO: Make window monitor transfer to the fullscreen window.
            if (!bFullscreen.HasValue)
            {
                OutputWindow.DisposeMedia();
                OutputWindow.Hide();

                if (ReferenceEquals(OutputWindow, OwnerWindow))
                {
                    OutputWindow = new FullscreenWindow(_EventAggregator, Store);
                }
                else
                {
                    // It is the fullscreen window. Fully destroy it.
                    OutputWindow.Close();
                    OutputWindow = OwnerWindow;
                }
            }
            else
            {
                if (bFullscreen.Value)
                {
                    if (!ReferenceEquals(OutputWindow, OwnerWindow))
                    {
                        // We are already in fullscreen.
                        return;
                    }

                    ToggleFullscreen();
                }
                else
                {
                    if (ReferenceEquals(OutputWindow, OwnerWindow))
                    {
                        // We are already in windowed mode.
                        return;
                    }

                    ToggleFullscreen();
                }
            }
        }

        /// <inheritdoc />
        public void SetOwner(ShellWindow owner)
        {
            owner.Should().NotBeNull();

            OwnerWindow = owner;
        }

        /// <inheritdoc />
        public void SetVolume(float volume)
        {
            volume.Should().BeInRange(0, 1);

            Exception ex;
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                ex = e;
            }

            DispatchEvent(new FVolumeChangedEvent(volume)
            {
                NumberType = EPercentNumberType.Normalized,
                ValueType = EValueType.Absolute,
                Exception = ex
            });
        }

        /// <inheritdoc />
        public void AddFilesToPlaylist(IEnumerable<string> files)
        {
            if (files == null)
            {
                return;
            }

            AddFilesToPlaylist(files.Select(file => new FileInfo(file)));
        }

        /// <inheritdoc />
        public void AddFilesToPlaylist(IEnumerable<FileInfo> files)
        {
            if (files == null)
            {
                return;
            }

            var items = files as FileInfo[] ?? files.ToArray();
            if (items.Length == 0)
            {
                return;
            }

            try
            {
                PlaylistInternal.AddRange(items.Select(file => new FPlaylistItem(MediaFormats, file, EPlaylistItemType.File)));
            }
            catch (Exception e)
            {
                Logger.Error(e);

                throw new MediaEngineException(e.Message, e);
            }
        }

        /// <inheritdoc />
        public void ClearPlaylist(bool bStopMedia = false)
        {
            if (bStopMedia)
            {
                CloseMedia();
            }

            PlaylistInternal.Clear();
        }

        public void OpenMedia(bool bCloseExistingMedia)
        {
            //MainFrm:3719

            try
            {
                // If we are already loading something, bail out.
                if (MediaLoadState == EMediaLoadState.Loading)
                {
                    return;
                }

                // e.Data represents whether we want to force close existing media
                // in order to proceed opening and playing the next media event.
                // If we call this event with that value equal to false and no media
                // is loaded we can also proceed since we aren't interrupting anything.
                if (bCloseExistingMedia || MediaLoadState == EMediaLoadState.Closed)
                {
                    // Close existing media.
                    CloseMedia();
                    MediaLoadState.Should().Be(EMediaLoadState.Closed);

                    // Load the next playlist item.
                    var data = GetNextMediaData();

                    // If the OpenMediaData is valid, open the MediaData.
                    if (data != null)
                    {
                        MediaLoadState = EMediaLoadState.Loading;

                        OnPreOpenMedia(data);
                        OnOpenMedia(data);
                        OnPostOpenMedia(data);
                    }
                }
            }
            catch (Exception openException)
            {
                // There was an error opening the media. Attempt to close whatever
                // has been opened. If an exception occurs again there, it should be
                // up to the developer to safely release any potentially corrupt
                // resources. In that case, simply set the MediaLoadState to closed
                // and rethrow the original exception.
                try
                {
                    CloseMedia();
                    MediaLoadState.Should().Be(EMediaLoadState.Closed);
                }
                finally
                {
                    MediaLoadState = EMediaLoadState.Closed;
                    // TODO: The OpenMedia token source might want to be reset here as well.
                }

                Logger.Error(openException.Message, openException);

                throw new MediaEngineException(openException.Message, openException);
            }
        }

        /// <inheritdoc />
        public void CloseMedia()
        {
            try
            {
                // No need to close if the media is already closed or in the process of closing.
                if (MediaLoadState == EMediaLoadState.Closed || MediaLoadState == EMediaLoadState.Closing)
                {
                    return;
                }

                DispatchEvent(new FMediaUnloadingEvent());

                // If the media state is loading, abort the load.
                if (MediaLoadState == EMediaLoadState.Loading)
                {
                    using (new FWaitCursor())
                    {
                        OpenMediaTokenSource?.Cancel(true);
                        OnAbortGraph();
                    }
                }

                MediaLoadState = EMediaLoadState.Closing;

                OnStopMedia();

                MediaLoadState.Should().Be(EMediaLoadState.Closing);
                OnPreCloseMedia();
                MediaLoadState.Should().Be(EMediaLoadState.Closing);
                OnCloseMedia();
                MediaLoadState.Should().Be(EMediaLoadState.Closing);
                OnPostCloseMedia();
                MediaLoadState.Should().Be(EMediaLoadState.Closing);

                Engine = EEngineType.None;
                PlaybackMode = EPlaybackMode.None;
                MediaLoadState = EMediaLoadState.Closed;

                // This would also be the place to destroy any system messages asking to open media,
                // but our event dispatcher is synchronous. It is not possible to queue up two different
                // events at the same time.
                
                OpenMediaTokenSource?.Dispose();
                OpenMediaTokenSource = new CancellationTokenSource();

                DispatchEvent(new FMediaUnloadedEvent());
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);

                throw new MediaEngineException(e.Message, e);
            }
        }

        /// <inheritdoc />
        public virtual bool IsSubtitleRendererRegistered()
        {
            var renderer = Store.SubtitleRenderer;

            return IsSubtitleRendererRegistered(renderer);
        }

        /// <inheritdoc />
        public virtual ESubtitleRenderer GetSubtitleRenderer()
        {
            var renderer = Store.VideoRenderer;

            if (IsSubtitleRendererSupported(ESubtitleRenderer.Internal, renderer) ||
                IsSubtitleRendererSupported(ESubtitleRenderer.XYSubFilter, renderer) ||
                IsSubtitleRendererSupported(ESubtitleRenderer.ASSFilter, renderer))
            {
                return Store.SubtitleRenderer;
            }

            return ESubtitleRenderer.VSFilter;
        }

        /// <inheritdoc />
        public virtual EVideoRenderer GetVideoRenderer()
        {
            return Store.VideoRenderer;
        }

        /// <inheritdoc />
        public bool IsSubtitleRendererRegistered(ESubtitleRenderer renderer)
        {
            switch (renderer)
            {
                case ESubtitleRenderer.Internal:
                    return true;
                case ESubtitleRenderer.VSFilter:
                case ESubtitleRenderer.XYSubFilter:
                case ESubtitleRenderer.ASSFilter:
                    return WindowsInterop.IsActiveMovieCLSIDRegistered(WindowsInterop.GetCLSID(renderer));
                default:
                    return false;
            }
        }

        /// <inheritdoc />
        public bool IsVideoRendererRegistered()
        {
            var renderer = Store.VideoRenderer;

            return IsVideoRendererRegistered(renderer);
        }

        /// <inheritdoc />
        public bool IsVideoRendererRegistered(EVideoRenderer renderer)
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
                    return WindowsInterop.IsActiveMovieCLSIDRegistered(WindowsInterop.GetCLSID(renderer));
                default:
                    return false;
            }
        }

        /// <inheritdoc />
        public bool IsSubtitleRendererSupported(ESubtitleRenderer subtitleRenderer, EVideoRenderer videoRenderer)
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
        protected bool IsD3DFullscreen()
        {
            switch (Store.VideoRenderer)
            {
                case EVideoRenderer.VMR9Renderless:
                case EVideoRenderer.EVRCustom:
                case EVideoRenderer.Sync:
                    return Store.bFullscreen || CommandLineOptions.bFullscreen;
                default:
                    return false;
            }
        }

        protected virtual AMediaData GetNextMediaData()
        {
            // Get the currently active PlaylistItem. If no active item is playing, get the next playlist item.
            var currentPlaylistItem = PlaylistInternal.Current ?? PlaylistInternal.Next();

            // If there are no playlist items, bail out.
            if (currentPlaylistItem == null)
            {
                return null;
            }

            DispatchEvent(new FMediaLoadingEvent(currentPlaylistItem));

            // Get the current OpenMediaData from the playlist with a start time of 0.
            return GetMediaDataFromPlaylistItem(currentPlaylistItem, TimeSpan.Zero);
        }

        protected virtual AMediaData GetMediaDataFromPlaylistItem(FPlaylistItem item, TimeSpan startTime)
        {
            if (item == null)
            {
                return null;
            }

            // DVDs will contain "video_ts.ifo" somewhere in the filename.
            if (item.File.FullName.ToLowerInvariant().Contains("video_ts.ifo"))
            {
                return new FMediaDvdData(item.File);
            }

            if (item.Type == EPlaylistItemType.Device)
            {
                return new FMediaDeviceData(item.File)
                    {
                        //TODO: Figure out how to port this.
                        //VideoInput = item.VideoInput,
                        //VideoChannel = item.VideoChannel,
                        //AudioInput = item.AudioInput
                    };
            }

            return new FMediaFileData(item.File, startTime, item.Subs);
        }

        /// <summary>
        /// Called before the media is opened.
        /// </summary>
        /// <param name="data"></param>
        protected virtual void OnPreOpenMedia(AMediaData data)
        {
            CastData(data, out var fileData, out var dvdData, out var deviceData);

            if (fileData != null)
            {
                //TODO: Set window status type to file extension.
            }
            else if (dvdData != null)
            {
                //TODO: Set window status type to .ifo
            }
            else
            {
                //TODO: Set window status type to .unknown
            }

        }

        /// <summary>
        /// Opens the specified <see cref="AMediaData"/> object for playback.
        /// </summary>
        /// <param name="data">The data information to open.</param>
        protected virtual void OnOpenMedia(AMediaData data)
        {

        }

        /// <summary>
        /// Called after the media is opened.
        /// </summary>
        /// <param name="data"></param>
        protected virtual void OnPostOpenMedia(AMediaData data)
        {

        }

        /// <summary>
        /// Called before the media is closed.
        /// </summary>
        protected virtual void OnPreCloseMedia()
        {
            //TODO: Clear OSD messages.
        }

        /// <summary>
        /// Close the current media graph.
        /// </summary>
        protected virtual void OnCloseMedia()
        {
            //return Task.Run(() =>
            {
                _MediaControl?.Stop();

                _GraphBuilder?.RemoveFromROT();
                WindowsInterop.CoRelease(_GraphBuilder); 
                _GraphBuilder = null;
            }//);
        }

        /// <summary>
        /// Called after the media is closed.
        /// </summary>
        protected virtual void OnPostCloseMedia()
        {
            //TODO: 3424
        }

        /// <summary>
        /// Abort the graph building process.
        /// </summary>
        protected virtual void OnAbortGraph()
        {
            var abortTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            Task.Run(() => _GraphBuilder?.Abort(), abortTokenSource.Token).Wait(abortTokenSource.Token);
            if (abortTokenSource.IsCancellationRequested)
            {
                //TODO: Destroy the graph builder and make a new one.
                //_GraphBuilder = new IGraphBuilder2();
            }
        }

        /// <summary>
        /// Stop playing the currently active media.
        /// </summary>
        protected virtual void OnStopMedia()
        {
            // TODO: 6997
        }

        /// <summary>
        /// <see cref="AMediaData"/> casting utility method for this engine version.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="file"></param>
        /// <param name="dvd"></param>
        /// <param name="device"></param>
        /// <returns>Whether one of the casts was successful.</returns>
        protected bool CastData(AMediaData data, out FMediaFileData file, out FMediaDvdData dvd, out FMediaDeviceData device)
        {
            file = data as FMediaFileData;
            dvd = data as FMediaDvdData;
            device = data as FMediaDeviceData;

            return file != null || dvd != null || device != null;
        }

        private void VolumeChangedRequestEvent(FVolumeChangeRequestEvent e)
        {
            e.Should().NotBeNull();
            e.NumberType.Should().NotBe(EPercentNumberType.NaN, "Unable to process volume event without knowing how to handle the value.");

            float normalizedValue = e.NumberType == EPercentNumberType.Normalized ? e.Data : e.Data / 100;
            if (e.ValueType == EValueType.Absolute)
            {
                SetVolume(normalizedValue);
            }
            else
            {
                SetVolume(Store.Volume + normalizedValue);
            }
        }

        private void ClearPlaylistRequestEvent(FClearPlaylistRequestEvent e)
        {
            PlaylistInternal.Clear();
        }

        private void AddFilesToPlaylistRequestEvent(FAddFilesToPlaylistRequestEvent e)
        {
            AddFilesToPlaylist(e.Data);
        }

        private void OpenMediaRequestEvent(FOpenMediaRequestEvent e)
        {
            OpenMedia(e.Data);
        }

        private void CloseMediaRequestEvent(FCloseMediaRequestEvent e)
        {
            CloseMedia();
        }
    }
}

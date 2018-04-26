using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using FluentAssertions;
using FoundaryMediaPlayer.Configuration;
using FoundaryMediaPlayer.Engine.Components;
using FoundaryMediaPlayer.Events;
using FoundaryMediaPlayer.Windows;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace FoundaryMediaPlayer.Engine
{
    /// <summary>
    /// The base class for media engines.
    /// </summary>
    public abstract class MediaEngineBase : BindableBase, IMediaEngine
    {
        private EMediaLoadState _MediaLoadState = EMediaLoadState.Closed;
        private EEngineType _Engine = EEngineType.None;
        private EPlaybackMode _PlaybackMode = EPlaybackMode.None;
        private ICommand _SetVolumeCommand;


        /// <inheritdoc />
        public MediaFormatCollection MediaFormats { get; }


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
                e => _EventAggregator.GetEvent<VolumeChangeRequestEvent>().Publish(new VolumeChangeRequestEvent
                {
                    NumberType = EPercentNumberType.NonNormalized,
                    ValueType = EValueType.Offset,
                    Data = (int)e.HorizontalChange
                })));
            set => SetProperty(ref _SetVolumeCommand, value);
        }

        /// <summary>
        /// The application store.
        /// </summary>
        protected Store Store { get; }

        /// <summary>
        /// The command line options.
        /// </summary>
        protected CLOptions Options { get; }

        /// <summary>
        /// The playlist.
        /// </summary>
        protected Playlist Playlist { get; }

        /// <summary>
        /// The currently loaded media stream.
        /// </summary>
        protected FileStream MediaStream { get; set; }

        /// <summary>
        /// The cancellation token to use for aborting opening a file.
        /// </summary>
        protected CancellationTokenSource OpenMediaTokenSource { get; set; }

        /// <summary>
        /// The owner window (the Shell).
        /// </summary>
        protected MediaOutputWindowBase OwnerWindow { get; set; }

        /// <summary>
        /// The current output window.
        /// </summary>
        protected MediaOutputWindowBase OutputWindow { get; set; }

        /// <summary>
        /// The application event aggregator.
        /// </summary>
        private IEventAggregator _EventAggregator { get; }

        /// <summary>
        /// 
        /// </summary>
        protected MediaEngineBase(
            Store store,
            CLOptions options,
            MediaFormatCollection mediaFormats,
            IEventAggregator eventAggregator)
        {
            store.Should().NotBeNull();
            options.Should().NotBeNull();
            mediaFormats.Should().NotBeNullOrEmpty();
            eventAggregator.Should().NotBeNull();

            Store = store;
            Options = options;
            MediaFormats = mediaFormats;
            Playlist = new Playlist(MediaFormats);

            _EventAggregator = eventAggregator;


            SubscribeEvent<ClearPlaylistRequestEvent>(ClearPlaylistRequestEvent);
            SubscribeEvent<AddFilesToPlaylistRequestEvent>(AddFilesToPlaylistRequestEvent);
            SubscribeEvent<OpenMediaRequestEvent>(OpenMediaRequestEvent);
            SubscribeEvent<CloseMediaRequestEvent>(CloseMediaRequestEvent);

            SubscribeEvent<VolumeChangeRequestEvent>(SetVolumeEvent);
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            CloseMedia();

            // We only 
            OutputWindow.DisposeMedia();
        }
        
        /// <inheritdoc />
        public void SetOwner(MediaOutputWindowBase owner)
        {
            OwnerWindow = owner;
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

        /// <inheritdoc />
        public void SetVolume(float value)
        {
            value.Should().BeInRange(0, 1);

            Exception ex = null;
            try
            {
                OnSetVolume(value);
            }
            catch (Exception e)
            {
                ex = e;
            }

            DispatchEvent(new VolumeChangedEvent(value)
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

            Playlist.AddRange(items.Select(file => new PlaylistItem(MediaFormats, file, EPlaylistItemType.File)));
        }

        /// <inheritdoc />
        public void ClearPlaylist(bool bStopMedia = false)
        {
            if (bStopMedia)
            {
                CloseMedia();
            }

            Playlist.Clear();
        }

        /// <inheritdoc />
        public void CloseMedia()
        {
            // No need to close if the media is already closed or in the process of closing.
            if (MediaLoadState == EMediaLoadState.Closed || MediaLoadState == EMediaLoadState.Closing)
            {
                return;
            }

            DispatchEvent(new MediaUnloadingEvent());

            // If the media state is loading, abort the load.
            if (MediaLoadState == EMediaLoadState.Loading)
            {
                using (new WaitCursor())
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

            MediaStream?.Dispose();
            MediaStream = null;

            OpenMediaTokenSource?.Dispose();
            OpenMediaTokenSource = new CancellationTokenSource();

            DispatchEvent(new MediaUnloadedEvent());
        }

        private void SetVolumeEvent(VolumeChangeRequestEvent e)
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
                SetVolume(Store.Player.Volume + normalizedValue);
            }
        }

        private void ClearPlaylistRequestEvent(ClearPlaylistRequestEvent e)
        {
            Playlist.Clear();
        }

        private void AddFilesToPlaylistRequestEvent(AddFilesToPlaylistRequestEvent e)
        {
            AddFilesToPlaylist(e?.Data);
        }

        private void OpenMediaRequestEvent(OpenMediaRequestEvent e)
        {
            //MainFrm:3719
            
            // If we are already loading something, bail out.
            if (MediaLoadState == EMediaLoadState.Loading)
            {
                return;
            }

            // e.Data represents whether we want to force close existing media
            // in order to proceed opening and playing the next media event.
            // If we call this event with that value equal to false and no media
            // is loaded we can also proceed since we aren't interrupting anything.
            if (e.Data || MediaLoadState == EMediaLoadState.Closed)
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

        private void CloseMediaRequestEvent(CloseMediaRequestEvent e)
        {
            CloseMedia();
        }

        private MediaData GetNextMediaData()
        {
            // Get the currently active PlaylistItem. If no active item is playing, get the next playlist item.
            var currentPlaylistItem = Playlist.Current ?? Playlist.Next();

            // If there are no playlist items, bail out.
            if (currentPlaylistItem == null)
            {
                return null;
            }

            DispatchEvent(new MediaLoadingEvent(currentPlaylistItem));

            // Get the current OpenMediaData from the playlist with a start time of 0.
            return GetMediaDataFromPlaylistItem(currentPlaylistItem, TimeSpan.Zero);
        }

        private void ClearMenuItems()
        {
            //TODO: 14782
        }

        /// <inheritdoc />
        public bool IsSubtitleRendererRegistered()
        {
            var renderer = Store.Media.SubtitleRenderer;

            return IsSubtitleRendererRegistered(renderer);
        }

        /// <inheritdoc />
        public bool IsVideoRendererRegistered()
        {
            var renderer = Store.Media.VideoRenderer;

            return IsVideoRendererRegistered(renderer);
        }

        /// <inheritdoc />
        public abstract ESubtitleRenderer GetSubtitleRenderer();

        /// <inheritdoc />
        public abstract EVideoRenderer GetVideoRenderer();

        /// <inheritdoc />
        public abstract bool IsSubtitleRendererRegistered(ESubtitleRenderer renderer);

        /// <inheritdoc />
        public abstract bool IsVideoRendererRegistered(EVideoRenderer renderer);

        /// <inheritdoc />
        public abstract bool IsSubtitleRendererSupported(ESubtitleRenderer subtitleRenderer, EVideoRenderer videoRenderer);

        /// <summary>
        /// Loads the playlist item and its associated media data.
        /// </summary>
        /// <param name="item">The playlist item to load.</param>
        /// <param name="startTime">The time to start playing.</param>
        /// <returns>The loaded media data.</returns>
        protected abstract MediaData GetMediaDataFromPlaylistItem(PlaylistItem item, TimeSpan startTime);

        /// <summary>
        /// Sets the new volume.
        /// </summary>
        /// <param name="newVolume">The new volume between 0 and 1.</param>
        /// <returns><see langword="true" /> if the volume was successfully changed, <see langword="false" /> otherwise.</returns>
        protected abstract void OnSetVolume(float newVolume);

        /// <summary>
        /// Called before the media is opened.
        /// </summary>
        /// <param name="data"></param>
        protected virtual void OnPreOpenMedia(MediaData data)
        {
            var fileData = data as MediaFileData;
            var dvdData = data as MediaDvdData;

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
        /// Opens the specified <see cref="MediaData"/> object for playback.
        /// </summary>
        /// <param name="data">The data information to open.</param>
        protected abstract void OnOpenMedia(MediaData data);

        /// <summary>
        /// Called after the media is opened.
        /// </summary>
        /// <param name="data"></param>
        protected virtual void OnPostOpenMedia(MediaData data)
        {

        }

        /// <summary>
        /// Called before the media is closed.
        /// </summary>
        protected virtual void OnPreCloseMedia()
        {
            //TODO: Clear OSD messages.

            ClearMenuItems();
        }

        /// <summary>
        /// Close the current media graph.
        /// </summary>
        protected abstract void OnCloseMedia();

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
        protected abstract void OnAbortGraph();

        /// <summary>
        /// Stop playing the currently active media.
        /// </summary>
        protected abstract void OnStopMedia();
    }
}

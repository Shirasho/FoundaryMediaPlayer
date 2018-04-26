using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using FoundaryMediaPlayer.Configuration;
using FoundaryMediaPlayer.Windows;

namespace FoundaryMediaPlayer.Engine
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMediaEngine : IDisposable
    {
        /// <summary>
        /// The media formats of the application.
        /// </summary>
        MediaFormatCollection MediaFormats { get; }

        /// <summary>
        /// Sets the output volume. Required by the view.
        /// </summary>
        ICommand SetVolumeCommand { get; }

        /// <summary>
        /// The media load state.
        /// </summary>
        EMediaLoadState MediaLoadState { get; }

        /// <summary>
        /// The current engine being used to process the media.
        /// </summary>
        EEngineType Engine { get; }

        /// <summary>
        /// The playback mode.
        /// </summary>
        EPlaybackMode PlaybackMode { get; }

        /// <summary>
        /// Toggles fullscreen.
        /// </summary>
        void ToggleFullscreen(bool? bFullscreen = null);

        /// <summary>
        /// Sets the owner window of this engine.
        /// </summary>
        /// <param name="owner"></param>
        void SetOwner(MediaOutputWindowBase owner);

        /// <summary>
        /// Sets the output volume.
        /// </summary>
        /// <param name="volume">The new volume between 0 and 1.</param>
        void SetVolume(float volume);

        /// <summary>
        /// Adds the specified files to the playlist.
        /// </summary>
        /// <param name="files">The files to add.</param>
        void AddFilesToPlaylist(IEnumerable<string> files);

        /// <summary>
        /// Adds the specified files to the playlist.
        /// </summary>
        /// <param name="files">The files to add.</param>
        void AddFilesToPlaylist(IEnumerable<FileInfo> files);

        /// <summary>
        /// Clears the playlist.
        /// </summary>
        /// <param name="bStopMedia">Whether to stop the currently playing media.</param>
        void ClearPlaylist(bool bStopMedia = false);

        /// <summary>
        /// Closes the current media and stops playback.
        /// </summary>
        void CloseMedia();

        /// <summary>
        /// Whether the subtitle renderer specified in the <see cref="Store"/> is registered.
        /// </summary>
        /// <returns></returns>
        bool IsSubtitleRendererRegistered();

        /// <summary>
        /// Obtains the subtitle renderer from the <see cref="Store"/>. If it is not supported,
        /// a different renderer will be returned.
        /// </summary>
        /// <returns></returns>
        ESubtitleRenderer GetSubtitleRenderer();

        /// <summary>
        /// Gets the video renderer from the <see cref="Store"/>. If the specified renderer is not
        /// supported, another video renderer will be returned.
        /// </summary>
        /// <returns></returns>
        EVideoRenderer GetVideoRenderer();

        /// <summary>
        /// Returns whether the specified subtitle renderer is registered.
        /// </summary>
        /// <param name="renderer">The subtitle renderer.</param>
        /// <returns></returns>
        bool IsSubtitleRendererRegistered(ESubtitleRenderer renderer);

        /// <summary>
        /// Returns whether the video renderer in the <see cref="Store"/> is registered.
        /// </summary>
        /// <returns></returns>
        bool IsVideoRendererRegistered();

        /// <summary>
        /// Returns whether the specified video renderer is registered.
        /// </summary>
        /// <param name="renderer">The video renderer.</param>
        /// <returns></returns>
        bool IsVideoRendererRegistered(EVideoRenderer renderer);

        /// <summary>
        /// Returns whether the subtitle renderer is supported for the specified video renderer.
        /// </summary>
        /// <param name="subtitleRenderer">The subtitle renderer.</param>
        /// <param name="videoRenderer">The video renderer.</param>
        /// <returns></returns>
        bool IsSubtitleRendererSupported(ESubtitleRenderer subtitleRenderer, EVideoRenderer videoRenderer);
    }
}

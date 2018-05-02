using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using FluentAssertions;
using Foundary;
using Foundary.Extensions;
using FoundaryMediaPlayer.Application;
using FoundaryMediaPlayer.Properties;

namespace FoundaryMediaPlayer.Engine
{
    public interface IReadOnlyMediaFormatCollection : IReadOnlyCollection<FMediaFormat>
    {
        /// <summary>
        /// The RTSP engine.
        /// </summary>
        EEngineType RtspEngine { get; }

        /// <summary>
        /// Returns whether this <see cref="IReadOnlyMediaFormatCollection"/> instance has a <see cref="FMediaFormat"/>
        /// registered with the specified extension.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <param name="bAudioOnly">Whether to only look for audio-only formats.</param>
        /// <param name="bAssociableOnly">Whether to look for associable-only formats.</param>
        /// <returns><see langword="true"/> if this <see cref="IReadOnlyMediaFormatCollection"/> instance has a <see cref="FMediaFormat"/> instance registered with
        /// the specified extension, <see langword="false"/> otherwise.</returns>
        bool HasFormat(string extension, bool bAudioOnly = false, bool bAssociableOnly = false);

        /// <summary>
        /// Returns the <see cref="FMediaFormat"/> registered in this <see cref="FMediaFormatCollection"/> instance
        /// that has the specified extension.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <param name="bAudioOnly">Whether to only look for audio-only formats.</param>
        /// <param name="bAssociableOnly">Whether to look for associable-only formats.</param>
        /// <returns>The <see cref="FMediaFormat"/> instance that contains the specified extension if found, <see langword="null"/> otherwise.</returns>
        FMediaFormat FindFormatByExtension(string extension, bool bAudioOnly = false, bool bAssociableOnly = false);

        /// <summary>
        /// Sets the RTSP engine to use.
        /// </summary>
        /// <param name="rtspEngine">The RTSP engine to use.</param>
        /// <param name="bNewFileExtensionFirst"></param>
        void SetEngine(EEngineType rtspEngine, bool bNewFileExtensionFirst);

        /// <summary>
        /// Whether the specified path is using the provided engine type.
        /// </summary>
        /// <param name="file">The path.</param>
        /// <param name="engineType">The engine type.</param>
        /// <returns><see langword="true"/> if the path is using the specified engine, <see langword="false"/> otherwise.</returns>
        bool IsUsingEngine(FileInfo file, EEngineType engineType);

        /// <summary>
        /// Gets all format filters.
        /// </summary>
        /// <param name="filter">The resulting filter will be stored in this variable.</param>
        /// <remarks>Used for file system operations.</remarks>
        void GetFilter(out string filter);

        /// <summary>
        /// Gets all format filters.
        /// </summary>
        /// <param name="filter">The resulting filter will be stored in this variable.</param>
        /// <param name="filters">The filters will be added to this list for convenience.</param>
        /// <param name="masks">The filter masks will be added to this list for convenience.</param>
        /// <remarks>Used for file system operations.</remarks>
        void GetFilter(out string filter, out IList<string> filters, out IList<string> masks);

        /// <summary>
        /// Gets the audio format filters.
        /// </summary>
        /// <param name="filter">The resulting filter will be stored in this variable.</param>
        void GetAudioFilter(out string filter);

        /// <summary>
        /// Gets the audio format filters.
        /// </summary>
        /// <param name="filter">The resulting filter will be stored in this variable.</param>
        /// <param name="descriptions">The filters descriptions be added to this list for convenience.</param>
        /// <param name="filters">The filter masks will be added to this list for convenience.</param>
        /// <remarks>Used for file system operations.</remarks>
        void GetAudioFilter(out string filter, out IList<string> descriptions, out IList<string> filters);

        /// <summary>
        /// Gets the video format filters.
        /// </summary>
        /// <param name="filter">The resulting filter will be stored in this variable.</param>
        /// <param name="descriptions">The filter descriptions will be added to this list for convenience.</param>
        /// <param name="filters">The filter masks will be added to this list for convenience.</param>
        /// <remarks>Used for file system operations.</remarks>
        void GetVideoFilter(out string filter, out IList<string> descriptions, out IList<string> filters);

        /// <summary>
        /// Repopulates the list of formats. Each format will then update itself depending on
        /// variable settings in the <see cref="FApplicationStore"/>.
        /// </summary>
        void Update();

        /// <summary>
        /// Saves any necessary information about this <see cref="IReadOnlyMediaFormatCollection"/> instance to the <see cref="FApplicationStore"/>.
        /// </summary>
        void Save();

        /// <summary>
        /// Gets the engine of the specified path.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        EEngineType GetEngine(FileInfo file);
    }

    /// <summary>
    /// A list of supported media formats.
    /// </summary>
    public sealed class FMediaFormatCollection : ObservableCollection<FMediaFormat>, IReadOnlyMediaFormatCollection
    {
        /// <summary>
        /// The RTSP protocol.
        /// </summary>
        public const string RtspProtocol = "rtsp://";

        /// <summary>
        /// 
        /// </summary>
        public bool bFileExtensionFirst { get; private set; } = true;

        /// <inheritdoc />
        public EEngineType RtspEngine { get; private set; }

        /// <summary>
        /// The <see cref="FApplicationStore"/> instance associated with this instance.
        /// </summary>
        private FApplicationStore _Store { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="store"></param>
        public FMediaFormatCollection(FApplicationStore store)
        {
            _Store = store;
            Update();
        }

        /// <inheritdoc />
        public bool HasFormat(string extension, bool bAudioOnly = false, bool bAssociableOnly = false)
        {
            return FindFormatByExtension(extension, bAudioOnly, bAssociableOnly) != null;
        }

        /// <inheritdoc />
        public FMediaFormat FindFormatByExtension(string extension, bool bAudioOnly = false, bool bAssociableOnly = false)
        {
            if (!string.IsNullOrWhiteSpace(extension))
            {
                foreach (var format in this)
                {
                    if ((!bAudioOnly || format.bAudioOnly) && (!bAssociableOnly || format.bAssociable) && format.HasExtension(extension))
                    {
                        return format;
                    }
                }
            }

            return null;
        }
        
        /// <inheritdoc />
        public void SetEngine(EEngineType rtspEngine, bool bNewFileExtensionFirst)
        {
            RtspEngine = rtspEngine;
            bFileExtensionFirst = bNewFileExtensionFirst;
        }

        /// <inheritdoc />
        public bool IsUsingEngine(FileInfo file, EEngineType engineType)
        {
            return GetEngine(file) == engineType;
        }
        
        /// <inheritdoc />
        public void GetFilter(out string filter)
        {
            GetFilter(out filter, out _, out _);
        }
        
        /// <inheritdoc />
        public void GetFilter(out string filter, out IList<string> filters, out IList<string> masks)
        {
            GetVideoFilter(out _, out IList<string> videoFilters, out IList<string> videoMasks);
            GetAudioFilter(out _, out IList<string> audioFilters, out IList<string> audioMasks);

            // Video and Audio filter lists always start with ALL filter. We only need one.
            filters = new List<string> { videoFilters[0], videoFilters[1], audioFilters[1] };
            masks = new List<string> { videoMasks[0], videoMasks[1], audioMasks[1] };

            // Highly inefficient, but whatever.
            videoFilters.RemoveAt(0);
            videoFilters.RemoveAt(0);
            videoMasks.RemoveAt(0);
            videoMasks.RemoveAt(0);
            audioFilters.RemoveAt(0);
            audioFilters.RemoveAt(0);
            audioMasks.RemoveAt(0);
            audioMasks.RemoveAt(0);

            filters.AddRange(videoFilters);
            filters.AddRange(audioFilters);

            masks.AddRange(videoMasks);
            masks.AddRange(audioMasks);

            var filterBuilder = new StringBuilder();
            BuildFilter(filterBuilder, filters, masks);
            filter = filterBuilder.ToString();
        }
        
        /// <inheritdoc />
        public void GetAudioFilter(out string filter)
        {
            GetAudioFilter(out filter, out _, out _);
        }
        
        /// <inheritdoc />
        public void GetAudioFilter(out string filter, out IList<string> descriptions, out IList<string> filters)
        {
            if (Count == 0)
            {
                filter = string.Empty;
                descriptions = new List<string>();
                filters = new List<string>();
                return;
            }

            var filterBuilder = new StringBuilder();
            descriptions = new List<string>
            {
                $"{Resources.LABEL_ALLFILES} (*.*)",
                $"{(AreExtensionsHidden() ? Resources.LABEL_AUDIOFILES : $"{Resources.LABEL_AUDIOFILES} (*.mp3;*.flac;*.wav;...)")}"
            };
            filters = new List<string> { "*.*" };

            GetFilterInternal(filterBuilder, descriptions, filters, f => f.bAudioOnly && f.EngineType == EEngineType.Custom);
            filter = filterBuilder.ToString();
        }
        
        /// <inheritdoc />
        public void GetVideoFilter(out string filter, out IList<string> descriptions, out IList<string> filters)
        {
            if (Count == 0)
            {
                filter = string.Empty;
                descriptions = new List<string>();
                filters = new List<string>();
                return;
            }

            var filterBuilder = new StringBuilder();
            descriptions = new List<string>
            {
                $"{Resources.LABEL_ALLFILES} (*.*)",
                $"{(AreExtensionsHidden() ? Resources.LABEL_MEDIAFILES : $"{Resources.LABEL_MEDIAFILES} (*.avi;*.mp4;*.mkv;...)")}"
            };
            filters = new List<string> { "*.*" };

            GetFilterInternal(filterBuilder, descriptions, filters, f => !f.bAudioOnly);
            filter = filterBuilder.ToString();
        }

        private void GetFilterInternal(StringBuilder filterBuilder, IList<string> descriptions, IList<string> filters, Predicate<FMediaFormat> formatDetector)
        {
            var subDescriptions = new List<string>();
            var subFilters = new List<string>();

            foreach (var format in this)
            {
                if (formatDetector(format))
                {
                    subDescriptions.Add(format.Description);
                    subFilters.Add(format.GetFilter());
                }
            }

            // Masks was passed in without the aggregation filter. We need to add that.
            filters.Add(subFilters.Implode(";"));

            descriptions.AddRange(subDescriptions);
            filters.AddRange(subFilters);

            BuildFilter(filterBuilder, descriptions, filters);
        }

        private void BuildFilter(StringBuilder filterBuilder, IList<string> descriptions, IList<string> filters)
        {
            descriptions.Should().NotBeNull();
            filters.Should().NotBeNull();
            descriptions.Count.Should().Be(filters.Count);

            var count = descriptions.Count;

            for (int i = 0; i < count; ++i)
            {
                filterBuilder.Append($"{descriptions[i]}|{filters[i]}{(i < count - 1 ? "|" : "")}");
            }
        }
        
        /// <inheritdoc />
        public void Update()
        {
            Clear();

            // Video
            Add(new FMediaFormat("avi", "AVI", "avi"));
            Add(new FMediaFormat("mpeg", "MPEG", "mpg mpeg mpe m1v m2v mpv2 mp2v pva evo mp2"));
            Add(new FMediaFormat("dvdvideo", "DVD-Video", "vob ifo"));
            Add(new FMediaFormat("mkv", "Matroska", "mkv mk3d"));
            Add(new FMediaFormat("webm", "WebM", "webm"));
            Add(new FMediaFormat("mp4", "MP4", "mp4 m4v mp4v mpv4 hdmov"));
            Add(new FMediaFormat("mov", "QuickTime Movie", "mov"));
            Add(new FMediaFormat("flv", "Flash Video", "flv f4v"));
            Add(new FMediaFormat("ogm", "Ogg Media", "ogm ogv"));
            Add(new FMediaFormat("wmv", "Windows Media Video", "wmv wmp wm asf"));
            Add(new FMediaFormat("swf", "Shockwave Flash", "swf", engineType: EEngineType.Shockwave));
            Add(new FMediaFormat("divx", "DivX", "divx"));

            // Audio
            Add(new FMediaFormat("flac", "FLAC", "flac", true));
            Add(new FMediaFormat("m4a", "MPEG-4 Audio", "m4a m4b m4r aac", true));
            Add(new FMediaFormat("midi", "MIDI", "mid midi rmi", true));
            Add(new FMediaFormat("mp3", "MP3", "mp3", true));
            Add(new FMediaFormat("ogg", "Ogg Vorbis", "ogg", true));
            Add(new FMediaFormat("wav", "WAV", "wav", true));
            Add(new FMediaFormat("wma", "Windows Media Audio", "wma", true));

            // Other
            Add(new FMediaFormat("rar", "RAR Archive", "rar", false, bAssociable: false));

            RtspEngine = _Store.RtspEngine;
            bFileExtensionFirst = _Store.RtspFileExtensionFirst;

            foreach (var format in this)
            {
                format.Update(_Store);
            }
        }
        
        /// <inheritdoc />
        public void Save()
        {
            _Store.RtspEngine = RtspEngine;
            _Store.RtspFileExtensionFirst = bFileExtensionFirst;
        }
        
        /// <inheritdoc />
        public EEngineType GetEngine(FileInfo file)
        {
            file.Should().NotBeNull();

            var enginePath = file.FullName.ToLowerInvariant();
            if (!bFileExtensionFirst && enginePath.StartsWith(RtspProtocol))
            {
                return RtspEngine;
            }

            var extension = file.Extension.ToLowerInvariant();
            if (!string.IsNullOrWhiteSpace(extension))
            {
                if (file.FullName.StartsWith(RtspProtocol))
                {
                    if (extension.Equals(".ram") || extension.Equals(".rm") || extension.Equals(".ra"))
                    {
                        return EEngineType.RealMedia;
                    }

                    if (extension.Equals(".qt") || extension.Equals(".mov"))
                    {
                        return EEngineType.QuickTime;
                    }
                }

                foreach (var format in this)
                {
                    if (format.HasExtension(extension))
                    {
                        return format.EngineType;
                    }
                }
            }

            if (bFileExtensionFirst && enginePath.StartsWith(RtspProtocol))
            {
                return RtspEngine;
            }

            return EEngineType.Custom;
        }

        private bool AreExtensionsHidden()
        {
            int? bAreExtensionsHidden;
            using (var key = new RegistryKeyReference("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced").Open())
            {
                bAreExtensionsHidden = key.GetInt("HideFileExt");
            }

            return bAreExtensionsHidden.HasValue && bAreExtensionsHidden.Value != 0;
        }
    }
}

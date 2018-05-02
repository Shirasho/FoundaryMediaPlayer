using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Foundary;
using Foundary.Collections;
using Foundary.Extensions;
using FoundaryMediaPlayer.Engine;
using FoundaryMediaPlayer.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Prism.Mvvm;

namespace FoundaryMediaPlayer.Application
{
    [Serializable]
    public class FApplicationStore : BindableBase
    {
        #region Store Helper Data
        private bool _bCorrupt;

        /// <summary>
        /// Whether this <see cref="FApplicationStore"/> is corrupt and should not be saved.
        /// </summary>
        [JsonIgnore]
        public bool bCorrupt
        {
            get => _bCorrupt;
            private set => SetProperty(ref _bCorrupt, value);
        }

        /// <summary>
        /// Sets whether the <see cref="FApplicationStore"/> instance is corrupt. Corrupt stores
        /// will not be serialized when the application exits.
        /// </summary>
        /// <param name="bNewCorrupt"></param>
        public void SetCorrupt(bool bNewCorrupt)
        {
            bCorrupt = bNewCorrupt;
        }

        /// <summary>
        /// Returns a <see cref="FApplicationStore"/> instance parsed from a JSON string.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="bReturnNewStoreOnFailure"></param>
        /// <returns></returns>
        public static FApplicationStore FromJson(string json, bool bReturnNewStoreOnFailure = true)
        {
            return FromJson(json, out _, bReturnNewStoreOnFailure);
        }

        /// <summary>
        /// Returns a <see cref="FApplicationStore"/> instance parsed from a JSON string.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="e"></param>
        /// <param name="bReturnNewStoreOnFailure"></param>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "LocalizableElement")]
        public static FApplicationStore FromJson(string json, out Exception e, bool bReturnNewStoreOnFailure = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(json))
                {
                    e = null;
                    return new FApplicationStore();
                }

                var result = JsonConvert.DeserializeObject<FApplicationStore>(json);
                if (result == null)
                {
                    throw new JsonReaderException($"Unable to parse {nameof(json)} into a valid {nameof(FApplicationStore)} object.");
                }

                e = null;
                return result;
            }
            catch (Exception ex)
            {
                e = ex;
                return bReturnNewStoreOnFailure ? new FApplicationStore { bCorrupt = true } : null;
            }
        }
        #endregion

        public FApplicationStore()
        {
            RecentFiles = new FixedCapacityList<string>(MaxRecentFiles);
            RecentFiles.OnCollectionChanged += RecentFiles_OnCollectionChanged;
        }

        #region Store Contents

        private byte _MaxRecentFiles = 10;
        private string _LastOpenedDirectory;
        private FixedCapacityList<string> _RecentFiles;
        private string _LastOpenedFile;
        private TimeSpan? _LastOpenedFilePosition;
        private EMetroAccent _Accent = EMetroAccent.Blue;
        private EMetroTheme _Theme = EMetroTheme.BaseDark;
        private Dictionary<EKeybindableEvent, List<FMergedInputGesture>> _Bindings = new Dictionary<EKeybindableEvent, List<FMergedInputGesture>>();
        private ESubtitleRenderer _SubtitleRenderer = ESubtitleRenderer.Internal;
        private EVideoRenderer _VideoRenderer = EVideoRenderer.Default;
        private EEngineType _RtspEngine = EEngineType.RealMedia;
        private bool _RtspFileExtensionFirst;
        private ECaptureDevice _DefaultCaptureDevice;
        private bool _bShowOSD;
        private bool _bFullScreen;
        private int _VolumeTickFrequency = 2;
        private long _SeekTickFrequency = (long)TimeSpan.FromSeconds(1).TotalMilliseconds;
        private bool _bAutoPlay = true;
        private bool _bRememberPosition;
        private int _Volume = 40;

        /// <summary>
        /// The maximum number of files that the history should keep track of.
        /// </summary>
        public byte MaxRecentFiles
        {
            get { return _MaxRecentFiles; }
            set
            {
                if (SetProperty(ref _MaxRecentFiles, value))
                {
                    RecentFiles.Resize(_MaxRecentFiles, EOrderDirection.FromFront);
                }
            }
        }

        /// <summary>
        /// The last opened directory when opening a file.
        /// </summary>
        public string LastOpenedDirectory
        {
            get => _LastOpenedDirectory;
            set => SetProperty(ref _LastOpenedDirectory, value);
        }
        
        /// <summary>
        /// A list of recently opened files.
        /// </summary>
        public FixedCapacityList<string> RecentFiles
        {
            get
            {
                return _RecentFiles;
            }
            set
            {
                if (_RecentFiles != null)
                {
                    _RecentFiles.OnCollectionChanged -= RecentFiles_OnCollectionChanged;
                }

                SetProperty(ref _RecentFiles, value);

                if (_RecentFiles != null)
                {
                    _RecentFiles.OnCollectionChanged += RecentFiles_OnCollectionChanged;
                }
            }
        }

        /// <summary>
        /// The path to the last opened file.
        /// </summary>
        public string LastOpenedFile
        {
            get => _LastOpenedFile;
            set => SetProperty(ref _LastOpenedFile, value);
        }

        /// <summary>
        /// The position in the last opened file.
        /// </summary>
        public TimeSpan? LastOpenedFilePosition
        {
            get => _LastOpenedFilePosition;
            set => SetProperty(ref _LastOpenedFilePosition, value);
        }

        /// <summary>
        /// The theme accent.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public EMetroAccent Accent
        {
            get => _Accent;
            set => SetProperty(ref _Accent, value);
        }

        /// <summary>
        /// The theme.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public EMetroTheme Theme
        {
            get => _Theme;
            set => SetProperty(ref _Theme, value);
        }

        /// <summary>
        /// The application key bindings.
        /// </summary>
        public Dictionary<EKeybindableEvent, List<FMergedInputGesture>> Bindings
        {
            get => _Bindings;
            set => SetProperty(ref _Bindings, value);
        }

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

        /// <summary>
        /// Whether to display the OSD.
        /// </summary>
        public bool bShowOSD
        {
            get => _bShowOSD;
            set => SetProperty(ref _bShowOSD, value);
        }

        /// <summary>
        /// Whether to autoplay files when they are loaded.
        /// </summary>
        public bool bAutoPlay
        {
            get => _bAutoPlay;
            set => SetProperty(ref _bAutoPlay, value);
        }

        /// <summary>
        /// Whether to start videos in fullscreen.
        /// </summary>
        public bool bFullscreen
        {
            get => _bFullScreen;
            set => SetProperty(ref _bFullScreen, value);
        }

        /// <summary>
        /// The volume of the audio player between 0 and 100 inclusive.
        /// </summary>
        public int Volume
        {
            get => _Volume;
            set => SetProperty(ref _Volume, value.Clamp(0, 100));
        }

        /// <summary>
        /// The amount to adjust the volume by when using the mousewheel to change the volume.
        /// </summary>
        public int VolumeTickFrequency
        {
            get => _VolumeTickFrequency;
            set => SetProperty(ref _VolumeTickFrequency, value.Clamp(1, 10));
        }

        /// <summary>
        /// The amount to seek by when using the mousewheel.
        /// </summary>
        public long SeekTickFrequency
        {
            get => _SeekTickFrequency;
            set => SetProperty(ref _SeekTickFrequency, value);
        }

        /// <summary>
        /// Whether to remember the playback position of the last open file.
        /// </summary>
        public bool bRememberPosition
        {
            get => _bRememberPosition;
            set => SetProperty(ref _bRememberPosition, value);
        }


        private void RecentFiles_OnCollectionChanged(object sender, CollectionChangedEventArgs e)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(RecentFiles)));
        }

        #endregion
    }
}

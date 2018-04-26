using Foundary.Extensions;
using Prism.Mvvm;

namespace FoundaryMediaPlayer.Configuration
{
    /// <summary>
    /// Media player settings.
    /// </summary>
    public class StorePlayer : BindableBase
    {
        private bool _bShowOSD;
        private bool _bFullScreen;
        private int _VolumeTickFrequency = 2;
        private bool _AutoPlay = true;
        private bool _RememberPosition;
        private int _Volume = 40;

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
            get => _AutoPlay;
            set => SetProperty(ref _AutoPlay, value);
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
        /// Whether to remember the playback position of the last open file.
        /// </summary>
        public bool RememberPosition
        {
            get => _RememberPosition;
            set => SetProperty(ref _RememberPosition, value);
        }
    }
}

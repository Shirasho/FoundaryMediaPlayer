using System;
using System.Collections.Generic;
using FoundaryMediaPlayer.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Prism.Mvvm;

namespace FoundaryMediaPlayer.Configuration
{
    /// <summary>
    /// Application interface settings.
    /// </summary>
    [Serializable]
    public class StoreInterface : BindableBase
    {
        private EMetroAccent _Accent = EMetroAccent.Blue;
        private EMetroTheme _Theme = EMetroTheme.BaseDark;
        private Dictionary<EKeybindableEvent, List<MergedInputGesture>> _Bindings = new Dictionary<EKeybindableEvent, List<MergedInputGesture>>();

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
        public Dictionary<EKeybindableEvent, List<MergedInputGesture>> Bindings
        {
            get => _Bindings;
            set => SetProperty(ref _Bindings, value);
        }
    }
}

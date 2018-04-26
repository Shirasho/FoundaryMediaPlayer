using System;
using System.Diagnostics.CodeAnalysis;
using CommandLine;

namespace FoundaryMediaPlayer.Configuration
{
    /// <summary>
    /// Command line options.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class CLOptions
    {
        /// <summary>
        /// The file to load.
        /// </summary>
        [Option('f', "file")]
        public string File { get; set; }

        ///// <summary>
        ///// The log verbosity. Overrides whatever is in the configuration.
        ///// </summary>
        //[Option('v', "verbosity", Default = null)]
        //public EVerbosity? Verbosity { get; set; }

        /// <summary>
        /// Whether to start in fullscreen mode. Overrides whatever is in the configuration.
        /// </summary>
        [Option(Default = null)]
        public bool? Fullscreen { get; set; }

        /// <summary>
        /// Whether to show the OSD. Overrides whatever is in the configuration.
        /// </summary>
        [Option(Default = null)]
        public bool? OSD {get; set;}

        /// <summary>
        /// The accent to use. Overrides whatever is in the configuration.
        /// </summary>
        [Option("accent")]
        public string AccentRaw { get; set; }

        /// <summary>
        /// The theme to use. Overrides whatever is in the configuration.
        /// </summary>
        [Option("theme")]
        public string ThemeRaw { get; set; }

        /// <summary>
        /// Whether to display a console window.
        /// </summary>
#if DEBUG
        [Option(Default = true)]
#else
        [Option(Default = false)]
#endif
        public bool Console { get; set; }

        /// <summary>
        /// The accent.
        /// </summary>
        public EMetroAccent? Accent => !string.IsNullOrWhiteSpace(AccentRaw) ? Enum.Parse(typeof(EMetroAccent), AccentRaw) as EMetroAccent? : null;

        /// <summary>
        /// The theme.
        /// </summary>
        public EMetroTheme? Theme => !string.IsNullOrWhiteSpace(ThemeRaw) ? Enum.Parse(typeof(EMetroTheme), ThemeRaw) as EMetroTheme? : null;
    }
}

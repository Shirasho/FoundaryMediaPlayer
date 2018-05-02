using CommandLine;

namespace FoundaryMediaPlayer.Application
{
    /// <summary>
    /// Command line options that are supported at startup.
    /// </summary>
    public class FStartupCommandLineOptions
    {
        /// <summary>
        /// The file to load.
        /// </summary>
        [Option('f', "file", Default = null, HelpText = "The media file to load.")]
        public string StartupFile { get; set; }

        /// <summary>
        /// Whether to start in fullscreen mode. Overrides whatever is in the configuration.
        /// </summary>
        [Option("fullscreen", Default = false, HelpText = "Whether to start in fullscreen mode. This option does not apply to all renderers.")]
        public bool bFullscreen { get; set; }

        /// <summary>
        /// Whether to show the OSD. Overrides whatever is in the configuration.
        /// </summary>
        [Option("osd", Default = null, HelpText = "Whether to show the OSD.")]
        public bool bOSD {get; set;}

        /// <summary>
        /// Whether to display a console window.
        /// </summary>
        [Option("console", HelpText = "Whether to show the interactive console/logging window.",
#if DEBUG
        Default = true
#else
        Default = false
#endif
        )]
        public bool bConsole { get; set; }
    }
}

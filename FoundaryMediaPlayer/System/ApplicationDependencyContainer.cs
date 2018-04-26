using FluentAssertions;
using FoundaryMediaPlayer.Platforms;
using Prism.Events;

namespace FoundaryMediaPlayer
{
    /// <summary>
    /// A container that collects multiple dependency injections related to the application.
    /// </summary>
    public class ApplicationDependencyContainer
    {
        /// <summary>
        /// The current platform.
        /// </summary>
        public IPlatform Platform { get; }

        /// <summary>
        /// The current application settings.
        /// </summary>
        public IApplicationSettings ApplicationSettings { get; }

        /// <summary>
        /// The current application paths.
        /// </summary>
        public IApplicationPaths ApplicationPaths { get; }

        /// <summary>
        /// The current application event aggregator.
        /// </summary>
        public IEventAggregator EventAggregator { get; }

        /// <summary>
        /// The application logger.
        /// </summary>
        public ApplicationLogger Logger { get; }

        /// <summary></summary>
        public ApplicationDependencyContainer(
            IPlatform platform,
            IApplicationSettings applicationSettings,
            IApplicationPaths applicationPaths,
            IEventAggregator eventAggregator,
            ApplicationLogger logger)
        {
            platform.Should().NotBeNull();
            applicationSettings.Should().NotBeNull();
            applicationPaths.Should().NotBeNull();
            eventAggregator.Should().NotBeNull();
            logger.Should().NotBeNull();

            Platform = platform;
            ApplicationSettings = applicationSettings;
            ApplicationPaths = applicationPaths;
            EventAggregator = eventAggregator;
            Logger = logger;
        }
    }
}

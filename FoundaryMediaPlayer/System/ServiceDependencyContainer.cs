using FluentAssertions;
using Foundary;
using FoundaryMediaPlayer.Windows;

namespace FoundaryMediaPlayer
{
    /// <summary>
    /// A container that collects multiple dependency injections related to services.
    /// </summary>
    public class ServiceDependencyContainer
    {
        /// <summary>
        /// The application service.
        /// </summary>
        public IApplicationService ApplicationService { get; }

        /// <summary>
        /// The window service.
        /// </summary>
        public IWindowService WindowService { get; }

        /// <summary></summary>
        public ServiceDependencyContainer(
            IApplicationService applicationService,
            IWindowService windowService)
        {
            applicationService.Should().NotBeNull();
            windowService.Should().NotBeNull();

            ApplicationService = applicationService;
            WindowService = windowService;
        }
    }
}

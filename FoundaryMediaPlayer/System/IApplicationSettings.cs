using Foundary;

namespace FoundaryMediaPlayer
{
    /// <summary>
    /// Application settings.
    /// </summary>
    public interface IApplicationSettings
    {
        /// <summary>
        /// Whether the application can check for updates.
        /// </summary>
        bool bCanCheckForUpdates { get; }

        /// <summary>
        /// Whether the application is terminating.
        /// </summary>
        bool bIsApplicationTerminating { get; set; }

        /// <summary>
        /// The company name.
        /// </summary>
        string CompanyName { get; }

        /// <summary>
        /// The company name without spaces.
        /// </summary>
        string CompanyNameNoSpaces { get; }

        /// <summary>
        /// The product name.
        /// </summary>
        string ProductName { get; }

        /// <summary>
        /// The product name without spaces.
        /// </summary>
        string ProductNameNoSpaces { get; }

        /// <summary>
        /// The human-friendly application name.
        /// </summary>
        string ApplicationName { get; }

        /// <summary>
        /// The application version.
        /// </summary>
        SimpleVersion Version { get; }
    }
}

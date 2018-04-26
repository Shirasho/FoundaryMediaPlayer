using Foundary.Extensions;
using FoundaryMediaPlayer.Configuration;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;

namespace FoundaryMediaPlayer.Platforms
{
    /// <summary>
    /// Operating system specific methods. <see cref="IPlatform"/> is the root.
    /// </summary>
    public interface IPlatform
    {
        /// <summary>
        /// Information about the operating system.
        /// </summary>
        OperatingSystemInfo OperatingSystem { get; }

        /// <summary>
        /// Initializes an external console window and redirects console output to point to
        /// this console.
        /// </summary>
        /// <param name="bForceInitializeConsole">Whether to force the initialization of a new console window.</param>
        /// <param name="options">The command line values.</param>
        object InitializeConsole(bool bForceInitializeConsole, CLOptions options);

        /// <summary>
        /// Disposes the console created by InitializeConsole.
        /// </summary>
        /// <returns>Whether the console was successfully disposed.</returns>
        bool DisposeConsole(object console);

        /// <summary>
        /// Configures the console appender for log4net.
        /// </summary>
        /// <param name="console">The console object created by <see cref="InitializeConsole(bool, CLOptions)"/>.</param>
        void ConfigureConsoleAppender(object console);
    }

    /// <summary>
    /// Platform-specific implementations.
    /// </summary>
    public abstract class Platform : IPlatform
    {
        private static IPlatform _Current;
        private static object _CurrentLock = new object();

        /// <summary>
        /// The current platform.
        /// </summary>
        public static IPlatform Current
        {
            get => _Current ?? (_Current = ObjectHelper.ThreadSafeDefault(ref _Current, ref _CurrentLock, () =>
#if WIN
                       new WindowsPlatform()
#endif

                   ));
        }

        /// <inheritdoc />
        public abstract OperatingSystemInfo OperatingSystem { get; }

        /// <inheritdoc />
        public abstract object InitializeConsole(bool bForceInitializeConsole, CLOptions options);

        /// <inheritdoc />
        public abstract bool DisposeConsole(object console);

        /// <inheritdoc />
        public abstract void ConfigureConsoleAppender(object console);

        /// <summary>
        /// Adds an appender to the root logger.
        /// </summary>
        /// <param name="appender">The appender to add.</param>
        public void AddAppender(AppenderSkeleton appender)
        {
            appender.ActivateOptions();
            ((Hierarchy)LogManager.GetRepository()).Root.AddAppender(appender);
        }
    }
}

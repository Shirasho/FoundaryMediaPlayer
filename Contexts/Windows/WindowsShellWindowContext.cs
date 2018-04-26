using FoundaryMediaPlayer.Configuration;
using FoundaryMediaPlayer.Engine;

namespace FoundaryMediaPlayer.Contexts.Windows
{
    /// <summary>
    /// A <see cref="ShellWindowContext"/> for the Windows operating system.
    /// </summary>
    public class WindowsShellWindowContext : ShellWindowContext
    {
        /// <inheritdoc />
        public WindowsShellWindowContext(
            ApplicationDependencyContainer adc,
            ServiceDependencyContainer sdc,
            ApplicationUpdater applicationUpdater,
            Store store,
            IMediaEngine mediaEngine) 
            : base(adc, sdc, applicationUpdater, store, mediaEngine)
        {
            
        }
    }
}

using System.Threading.Tasks;
using NConsole;

namespace FoundaryMediaPlayer.Application.ConsoleCommands
{
    /// <summary>
    /// Requests that the application exit.
    /// </summary>
    [Command(Name = "exit")]
    public class FExitCommand : IConsoleCommand
    {
        /// <inheritdoc />
        public Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            FApplication.Current.Shutdown();

            return Task.FromResult<object>(null);
        }
    }
}

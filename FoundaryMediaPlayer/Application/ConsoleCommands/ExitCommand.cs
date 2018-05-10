using System.Threading.Tasks;
using Foundary.CommandParser;
using JetBrains.Annotations;

namespace FoundaryMediaPlayer.Application.ConsoleCommands
{
    /// <summary>
    /// Requests that the application exit.
    /// </summary>
    [Command(Name = "exit")]
    public class FExitCommand : IConsoleCommand
    {
        /// <inheritdoc />
        [ContractAnnotation("=> halt")]
        public Task<object> RunAsync(ICommandParser parser, object input = null)
        {
            FApplication.Current.Shutdown();

            return Task.FromResult<object>(null);
        }
    }
}

using System.Threading.Tasks;
using NConsole;

namespace FoundaryMediaPlayer.Application.ConsoleCommands
{
    /// <summary>
    /// Requests that the application exit.
    /// </summary>
    [Command(Name = "test-console")]
    public class FTestCommand : IConsoleCommand
    {
        [Argument(Name = "value")]
        public string TestValue { get; set; }

        /// <inheritdoc />
        public Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            host.WriteMessage($"Test value: {TestValue}");

            return Task.FromResult<object>(null);
        }
    }
}

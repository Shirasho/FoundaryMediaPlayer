using System.Threading.Tasks;
using Foundary.CommandParser;

namespace FoundaryMediaPlayer.Application.ConsoleCommands
{
    /// <summary>
    /// Requests that the application exit.
    /// </summary>
    [Command(Name = "test-console")]
    public class FTestCommand : IConsoleCommand
    {
        [Argument(LongName = "value")]
        public string TestValue { get; set; }

        /// <inheritdoc />
        public Task<object> RunAsync(ICommandParser parser, object input = null)
        {
            //TODO: input is DI.
            //host.WriteMessage($"Test value: {TestValue}");

            return Task.FromResult<object>(null);
        }
    }
}

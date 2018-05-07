using log4net;
using NConsole;

namespace FoundaryMediaPlayer.Application
{
    public sealed class FApplicationConsoleHost : IConsoleHost
    {
        private static ILog Logger { get; } = LogManager.GetLogger(typeof(FApplicationConsoleHost));

        /// <inheritdoc />
        public void WriteMessage(string message)
        {
            Logger.Info(message);
        }

        /// <inheritdoc />
        public void WriteError(string message)
        {
            Logger.Error(message);
        }

        /// <inheritdoc />
        public string ReadValue(string message)
        {
            return null;
        }
    }
}

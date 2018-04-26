using log4net.Core;

namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// Called when a request to clear the playlist has been made.
    /// </summary>
    public sealed class ClearPlaylistRequestEvent : EventBase<ClearPlaylistRequestEvent>
    {
        /// <inheritdoc />
        protected override Level LoggingLevel { get; } = Level.Info;

        /// <inheritdoc />
        protected override string GetLoggerMessage(ClearPlaylistRequestEvent payload)
        {
            return "Request made to clear the playlist.";
        }
    }
}

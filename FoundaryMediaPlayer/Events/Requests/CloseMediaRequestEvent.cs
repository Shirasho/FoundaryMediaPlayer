using log4net.Core;

namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// The event sent when a request to close the current playlist media item has been created.
    /// </summary>
    public sealed class CloseMediaRequestEvent : EventBase<CloseMediaRequestEvent>
    {
        /// <inheritdoc />
        protected override Level LoggingLevel { get; } = Level.Info;

        /// <inheritdoc />
        protected override string GetLoggerMessage(CloseMediaRequestEvent payload)
        {
            return "Request made to close current media.";
        }
    }
}

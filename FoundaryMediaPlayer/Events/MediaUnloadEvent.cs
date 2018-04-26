using log4net.Core;

namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// The event sent when the previously loaded media is about to be unloaded.
    /// </summary>
    public sealed class MediaUnloadingEvent : EventBase<MediaUnloadingEvent>
    {
        /// <inheritdoc />
        protected override Level LoggingLevel { get; } = Level.Info;

        /// <inheritdoc />
        protected override string GetLoggerMessage(MediaUnloadingEvent payload)
        {
            return "Unloading previous media.";
        }
    }

    /// <summary>
    /// The event sent when the previously loaded media is finished unloading.
    /// </summary>
    public sealed class MediaUnloadedEvent : EventBase<MediaUnloadedEvent>
    {
        /// <inheritdoc />
        protected override Level LoggingLevel { get; } = Level.Info;

        /// <inheritdoc />
        protected override string GetLoggerMessage(MediaUnloadedEvent payload)
        {
            return "Previous media unloaded.";
        }
    }
}

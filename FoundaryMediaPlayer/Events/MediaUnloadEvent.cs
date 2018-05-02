using log4net.Core;

namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// The event sent when the previously loaded media is about to be unloaded.
    /// </summary>
    public sealed class FMediaUnloadingEvent : AEventBase<FMediaUnloadingEvent>
    {
        /// <inheritdoc />
        protected override Level LoggingLevel { get; } = Level.Info;

        /// <inheritdoc />
        protected override string GetLoggerMessage(FMediaUnloadingEvent payload)
        {
            return "Unloading previous media.";
        }
    }

    /// <summary>
    /// The event sent when the previously loaded media is finished unloading.
    /// </summary>
    public sealed class FMediaUnloadedEvent : AEventBase<FMediaUnloadedEvent>
    {
        /// <inheritdoc />
        protected override Level LoggingLevel { get; } = Level.Info;

        /// <inheritdoc />
        protected override string GetLoggerMessage(FMediaUnloadedEvent payload)
        {
            return "Previous media unloaded.";
        }
    }
}

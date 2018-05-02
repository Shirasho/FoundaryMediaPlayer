using log4net.Core;

namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// The event sent when a request to seek has been created.
    /// </summary>
    public sealed class FMediaSeekRequestEvent : ANumericEventBase<float, FMediaSeekRequestEvent>
    {
        /// <inheritdoc />
        protected override Level LoggingLevel { get; } = Level.Info;

        /// <inheritdoc />
        public FMediaSeekRequestEvent()
            : this(0)
        {

        }

        /// <inheritdoc />
        public FMediaSeekRequestEvent(long data) 
            : base(data)
        {
        }

        /// <inheritdoc />
        protected override string GetLoggerMessage(FMediaSeekRequestEvent payload)
        {
            return "Request made to " +
                   $"{(payload.ValueType == EValueType.Absolute ? "set" : "adjust")} " +
                   "media playback position " +
                   $"{(payload.ValueType == EValueType.Absolute ? "to" : "by")} " +
                   $"{payload.Data} milliseconds.";
        }
    }
}

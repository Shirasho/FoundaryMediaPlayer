using log4net.Core;

namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// The event sent when a request to adjust the volume has been created.
    /// </summary>
    public sealed class FVolumeChangeRequestEvent : ANumericEventBase<float, FVolumeChangeRequestEvent>
    {
        /// <inheritdoc />
        protected override Level LoggingLevel { get; } = Level.Info;

        /// <inheritdoc />
        public FVolumeChangeRequestEvent()
            : this(0)
        {

        }

        /// <inheritdoc />
        public FVolumeChangeRequestEvent(float data) 
            : base(data)
        {
        }

        /// <inheritdoc />
        protected override string GetLoggerMessage(FVolumeChangeRequestEvent payload)
        {
            return "Request made to " +
                   $"{(payload.ValueType == EValueType.Absolute ? "set" : "adjust")} " +
                   "volume " +
                   $"{(payload.ValueType == EValueType.Absolute ? "to" : "by")} " +
                   $"{payload.Data}.";
        }
    }
}

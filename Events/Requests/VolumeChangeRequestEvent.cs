using log4net.Core;

namespace FoundaryMediaPlayer.Events
{
    

    /// <summary>
    /// The event sent when a request to adjust the volume has been created.
    /// </summary>
    public sealed class VolumeChangeRequestEvent : NumericEventBase<float, VolumeChangeRequestEvent>
    {
        /// <inheritdoc />
        protected override Level LoggingLevel { get; } = Level.Info;

        /// <inheritdoc />
        public VolumeChangeRequestEvent()
            : this(0)
        {

        }

        /// <inheritdoc />
        public VolumeChangeRequestEvent(float data) 
            : base(data)
        {
        }

        /// <inheritdoc />
        protected override string GetLoggerMessage(VolumeChangeRequestEvent payload)
        {
            return "Request made to " +
                   $"{(payload.ValueType == EValueType.Absolute ? "set" : "adjust")} " +
                   "volume " +
                   $"{(payload.ValueType == EValueType.Absolute ? "to" : "by")} " +
                   $"{payload.Data}.";
        }
    }
}

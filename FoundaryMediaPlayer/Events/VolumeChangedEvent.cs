using log4net.Core;

namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// The event sent when the volume has been changed.
    /// </summary>
    public sealed class FVolumeChangedEvent : ANumericEventBase<float, FVolumeChangedEvent>
    {
        /// <inheritdoc />
        protected override Level LoggingLevel => Exception == null ? Level.Verbose : Level.Error;

        /// <inheritdoc />
        public FVolumeChangedEvent()
            : this(0)
        {

        }

        /// <inheritdoc />
        public FVolumeChangedEvent(float data)
            : base(data)
        {

        }

        /// <inheritdoc />
        protected override string GetLoggerMessage(FVolumeChangedEvent payload)
        {
            return $"Volume set to {payload.Data}";
        }
    }
}

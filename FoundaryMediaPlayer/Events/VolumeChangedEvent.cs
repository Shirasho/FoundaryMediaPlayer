using log4net.Core;

namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// The event sent when the volume has been changed.
    /// </summary>
    public sealed class VolumeChangedEvent : NumericEventBase<float, VolumeChangedEvent>
    {
        /// <inheritdoc />
        protected override Level LoggingLevel => Exception == null ? Level.Verbose : Level.Error;

        /// <inheritdoc />
        public VolumeChangedEvent()
            : this(0)
        {

        }

        /// <inheritdoc />
        public VolumeChangedEvent(float data)
            : base(data)
        {

        }

        /// <inheritdoc />
        protected override string GetLoggerMessage(VolumeChangedEvent payload)
        {
            return $"Volume set to {payload.Data}";
        }
    }
}

using FluentAssertions;
using FoundaryMediaPlayer.Engine;
using log4net.Core;

namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// The event sent when the media engine starts loading a playlist item for playback.
    /// </summary>
    public sealed class MediaLoadingEvent : EventBase<PlaylistItem, MediaLoadingEvent>
    {
        /// <inheritdoc />
        protected override Level LoggingLevel { get; } = Level.Info;

        /// <inheritdoc />
        public MediaLoadingEvent()
            : this(null)
        {

        }

        /// <inheritdoc />
        public MediaLoadingEvent(PlaylistItem value)
            : base(value) {}

        /// <inheritdoc />
        protected override string GetLoggerMessage(MediaLoadingEvent payload)
        {
            payload.Data.Should().NotBeNull();
            return $"Loading media {payload.Data.Type} at {payload.Data.File.Name}.";
        }
    }

    /// <summary>
    /// The event sent when the media engine has completed loading a playlist item for playback.
    /// </summary>
    public sealed class MediaLoadedEvent : EventBase<PlaylistItem, MediaLoadedEvent>
    {
        /// <inheritdoc />
        protected override Level LoggingLevel { get; } = Level.Info;

        /// <inheritdoc />
        public MediaLoadedEvent()
            : this(null)
        {

        }

        /// <inheritdoc />
        public MediaLoadedEvent(PlaylistItem value)
            : base(value) {}


        /// <inheritdoc />
        protected override string GetLoggerMessage(MediaLoadedEvent payload)
        {
            payload.Data.Should().NotBeNull();
            return $"Media {payload.Data.Type} at {payload.Data.File.Name} loaded.";
        }
    }
}

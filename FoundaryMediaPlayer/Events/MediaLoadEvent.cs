using FluentAssertions;
using FoundaryMediaPlayer.Engine;
using log4net.Core;

namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// The event sent when the media engine starts loading a playlist item for playback.
    /// </summary>
    public sealed class FMediaLoadingEvent : AEventBase<FPlaylistItem, FMediaLoadingEvent>
    {
        /// <inheritdoc />
        protected override Level LoggingLevel { get; } = Level.Info;

        /// <inheritdoc />
        public FMediaLoadingEvent()
            : this(null)
        {

        }

        /// <inheritdoc />
        public FMediaLoadingEvent(FPlaylistItem value)
            : base(value) {}

        /// <inheritdoc />
        protected override string GetLoggerMessage(FMediaLoadingEvent payload)
        {
            payload.Data.Should().NotBeNull();
            return $"Loading media {payload.Data.Type.ToString().ToLowerInvariant()} at {payload.Data.File.FullName}.";
        }
    }

    /// <summary>
    /// The event sent when the media engine has completed loading a playlist item for playback.
    /// </summary>
    public sealed class FMediaLoadedEvent : AEventBase<FPlaylistItem, FMediaLoadedEvent>
    {
        /// <inheritdoc />
        protected override Level LoggingLevel { get; } = Level.Info;

        /// <inheritdoc />
        public FMediaLoadedEvent()
            : this(null)
        {

        }

        /// <inheritdoc />
        public FMediaLoadedEvent(FPlaylistItem value)
            : base(value) {}


        /// <inheritdoc />
        protected override string GetLoggerMessage(FMediaLoadedEvent payload)
        {
            payload.Data.Should().NotBeNull();
            return $"Media {payload.Data.Type} at {payload.Data.File.Name} loaded.";
        }
    }
}

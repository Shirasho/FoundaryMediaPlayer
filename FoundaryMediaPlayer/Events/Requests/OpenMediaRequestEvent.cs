using FoundaryMediaPlayer.Engine;

namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// The event sent when a request to open the current playlist media item has been created.
    /// </summary>
    public sealed class FOpenMediaRequestEvent : ARequestEventBase<bool, FOpenMediaRequestEvent>
    {
        /// <inheritdoc />
        public FOpenMediaRequestEvent()
            : this(true)
        {

        }

        /// <summary>
        /// Whether this event should force the <see cref="IMediaEngine"/>
        /// to close existing media if media is currently playing. This would be set
        /// to <see langword="false" />, for example, on drag-drop operations where
        /// we only want to start playing the media if no other media is playing.
        /// </summary>
        /// <param name="bDisruptPlayingMedia"></param>
        public FOpenMediaRequestEvent(bool bDisruptPlayingMedia)
            : base(bDisruptPlayingMedia)
        {
            
        }

        /// <inheritdoc />
        protected override string GetLoggerMessage(FOpenMediaRequestEvent payload)
        {
            return "Request made to open media.";
        }
    }
}

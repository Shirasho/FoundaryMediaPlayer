namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// Called when a request to clear the playlist has been made.
    /// </summary>
    public sealed class FClearPlaylistRequestEvent : ARequestEventBase<FClearPlaylistRequestEvent>
    {
        /// <inheritdoc />
        protected override string GetLoggerMessage(FClearPlaylistRequestEvent payload)
        {
            return "Request made to clear the playlist.";
        }
    }
}

namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// The event sent when a request to close the current playlist media item has been created.
    /// </summary>
    public sealed class FCloseMediaRequestEvent : ARequestEventBase<FCloseMediaRequestEvent>
    {
        /// <inheritdoc />
        protected override string GetLoggerMessage(FCloseMediaRequestEvent payload)
        {
            return "Request made to close current media.";
        }
    }
}

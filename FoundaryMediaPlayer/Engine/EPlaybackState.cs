namespace FoundaryMediaPlayer.Engine
{
    /// <summary>
    /// The possible playback states of the media player.
    /// </summary>
    public enum EPlaybackState
    {
        /// <summary>
        /// The media has been unloaded or there is no media loaded.
        /// </summary>
        Stopped,

        /// <summary>
        /// The media is playing.
        /// </summary>
        Playing,

        /// <summary>
        /// The media is paused.
        /// </summary>
        Paused
    }
}

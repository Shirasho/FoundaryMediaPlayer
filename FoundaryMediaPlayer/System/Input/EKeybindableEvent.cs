namespace FoundaryMediaPlayer.Input
{
    /// <summary>
    /// Commands/events that can be keybound.
    /// </summary>
    public enum EKeybindableEvent
    {
        /// <summary>
        /// Toggle fullscreen.
        /// </summary>
        ToggleFullscreen = 1,

        /// <summary>
        /// Increase the volume.
        /// </summary>
        IncreaseVolume,

        /// <summary>
        /// Decrease the volume.
        /// </summary>
        DecreaseVolume,

        /// <summary>
        /// Toggle whether the volume is muted.
        /// </summary>
        ToggleVolume,

        /// <summary>
        /// A test event.
        /// </summary>
        DummyEvent = 9999
    }
}

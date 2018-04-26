namespace FoundaryMediaPlayer
{
    /// <summary>
    /// Application paths.
    /// </summary>
    public interface IApplicationPaths
    {
        /// <summary>
        /// The location of local app data.
        /// </summary>
        string LocalAppData { get; }

        /// <summary>
        /// The path to the application store.
        /// </summary>
        string Store { get; }
    }
}

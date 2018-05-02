namespace FoundaryMediaPlayer.Application
{
    /// <summary>
    /// Operating system specific methods. <see cref="IPlatform"/> is the root.
    /// </summary>
    public interface IPlatform
    {
        /// <summary>
        /// Information about the operating system.
        /// </summary>
        FOperatingSystemInfo OperatingSystem { get; }
    }
}

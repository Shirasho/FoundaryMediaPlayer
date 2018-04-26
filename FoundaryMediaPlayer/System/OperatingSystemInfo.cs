using System;

namespace FoundaryMediaPlayer
{
    /// <summary>
    /// Operating system information.
    /// </summary>
    public sealed class OperatingSystemInfo
    {
        /// <summary>
        /// The name of the operating system.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The operating system version.
        /// </summary>
        public Version Version { get; set; }
    }
}

using System.IO;
using DirectShowLib.Dvd;
using FluentAssertions;

namespace FoundaryMediaPlayer.Engine.Windows
{
    /// <summary>
    /// 
    /// </summary>
    public class WindowsMediaDvdData : MediaDvdData
    {
        /// <summary>
        /// The DVD state.
        /// </summary>
        public IDvdState DvdState { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public WindowsMediaDvdData(FileInfo file, IDvdState dvdState)
            : base(file)
        {
            DvdState = dvdState;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class WindowsMediaDeviceData : MediaData
    {
        /// <summary>
        /// The video input.
        /// </summary>
        public int VideoInput { get; set; } = -1;

        /// <summary>
        ///  The video channel.
        /// </summary>
        public int VideoChannel { get; set; } = -1;

        /// <summary>
        /// The audio input.
        /// </summary>
        public int AudioInput { get; set; } = -1;
    }
}

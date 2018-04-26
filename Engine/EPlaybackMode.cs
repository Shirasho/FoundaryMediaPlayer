using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoundaryMediaPlayer.Engine
{
    /// <summary>
    /// The playback mode.
    /// </summary>
    public enum EPlaybackMode
    {
        /// <summary>
        /// None
        /// </summary>
        None,

        /// <summary>
        /// File
        /// </summary>
        File,

        /// <summary>
        /// Dvd
        /// </summary>
        Dvd,

        /// <summary>
        /// Analog Capture
        /// </summary>
        AnalogCapture,

        /// <summary>
        /// Digital Capture
        /// </summary>
        DigitalCapture
    }
}

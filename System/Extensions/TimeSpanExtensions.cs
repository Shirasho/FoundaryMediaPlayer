using System;
using DirectShowLib;

namespace FoundaryMediaPlayer
{
    /// <summary>
    /// <see cref="TimeSpan"/> extensions.
    /// </summary>
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Converts a timespan instance to a <see cref="DsLong"/> in units of C++ REFERENCE_TIME.
        /// </summary>
        /// <param name="timespan"></param>
        /// <returns></returns>
        public static DsLong ToReferenceTime(this TimeSpan timespan)
        {
            var totalMilliseconds = (long)timespan.TotalMilliseconds;
            // One unit of REFERENCE_TIME is 100ns.
            return totalMilliseconds / 1000;
        }
    }
}

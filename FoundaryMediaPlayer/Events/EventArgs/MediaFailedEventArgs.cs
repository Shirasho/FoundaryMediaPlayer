using System;

namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// Information about a media failure event.
    /// </summary>
    public class MediaFailedEventArgs : EventArgs
    {
        /// <summary>
        /// The exception that was thrown.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// The message associated with the failure.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// 
        /// </summary>
        public MediaFailedEventArgs(string message, Exception e = null)
        {
            Message = message;
            Exception = e;
        }
    }
}

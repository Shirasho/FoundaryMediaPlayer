using System;

namespace FoundaryMediaPlayer.Engine
{
    /// <summary>
    /// An exception thrown by the media engine.
    /// </summary>
    public class MediaEngineException : Exception
    {
        /// <inheritdoc />
        public MediaEngineException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public MediaEngineException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

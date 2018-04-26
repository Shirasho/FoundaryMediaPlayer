using System.Collections.Generic;
using System.Linq;
using log4net.Core;

namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// The event sent when the user selects files to play from the Open File dialog.
    /// </summary>
    public sealed class AddFilesToPlaylistRequestEvent : EventBase<string[], AddFilesToPlaylistRequestEvent>
    {
        /// <inheritdoc />
        protected override Level LoggingLevel { get; } = Level.Info;

        /// <summary>
        /// 
        /// </summary>
        public AddFilesToPlaylistRequestEvent()
            : this(new string[0])
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        public AddFilesToPlaylistRequestEvent(IEnumerable<string> files)
            : this(files.ToArray())
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        public AddFilesToPlaylistRequestEvent(string[] files)
            : base(files)
        {

        }

        /// <inheritdoc />
        protected override string GetLoggerMessage(AddFilesToPlaylistRequestEvent payload)
        {
            return $"Request made to add {(payload.Data?.Length ?? 0)} files to the playlist.";
        }
    }
}

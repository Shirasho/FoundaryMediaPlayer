using System.Collections.Generic;
using System.Linq;

namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// The event sent when the user selects files to play from the Open File dialog.
    /// </summary>
    public sealed class FAddFilesToPlaylistRequestEvent : ARequestEventBase<string[], FAddFilesToPlaylistRequestEvent>
    {
        /// <summary>
        /// 
        /// </summary>
        public FAddFilesToPlaylistRequestEvent()
            : this(new string[0])
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        public FAddFilesToPlaylistRequestEvent(IEnumerable<string> files)
            : this(files.ToArray())
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        public FAddFilesToPlaylistRequestEvent(string[] files)
            : base(files)
        {

        }

        /// <inheritdoc />
        protected override string GetLoggerMessage(FAddFilesToPlaylistRequestEvent payload)
        {
            return $"Request made to add {(payload.Data?.Length ?? 0)} files to the playlist.";
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Foundary.Extensions;

namespace FoundaryMediaPlayer.Engine
{
    /// <summary>
    /// Contains information about the current playlist.
    /// </summary>
    public class Playlist : List<PlaylistItem>
    {
        /// <summary>
        /// The supported media formats.
        /// </summary>
        public IReadOnlyCollection<MediaFormat> MediaFormats {get;}

        private bool _bShuffle;
        
        /// <summary>
        /// The current playlist item.
        /// </summary>
        public PlaylistItem Current {get; protected set;}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediaFormats"></param>
        public Playlist(MediaFormatCollection mediaFormats)
        {
            mediaFormats.Should().NotBeNullOrEmpty();

            MediaFormats = mediaFormats;
        }

        /// <summary>
        /// Whether to shuffle the playlist elements.
        /// </summary>
        public bool bShuffle
        {
            get
            {
                return _bShuffle;
            }
            set
            {
                _bShuffle = value;
                if (bShuffle)
                {
                    this.Shuffle();
                }
            }
        }

        /// <summary>
        /// Removes all elements from the <see cref="Playlist"/>.
        /// </summary>
        public new void Clear()
        {
            base.Clear();
            Current = null;
        }

        /// <summary>
        /// Sets and returns the next playlist item according to the playlist shuffle rules.
        /// </summary>
        /// <returns>The next playlist item.</returns>
        public PlaylistItem Next()
        {
            if (Current == null)
            {
                if (bShuffle)
                {
                    this.Shuffle();
                }

                if (Count == 0)
                {
                    return null;
                }

                return Current = this[0];
            }

            var index = IndexOf(Current);
            if (index == Count - 1)
            {
                Current = null;
                return Next();
            }

            return this[index + 1];
        }

        /// <summary>
        /// Sets the playlist to only contain the specified files.
        /// </summary>
        /// <param name="files">The files to add.</param>
        public void SetFiles(IEnumerable<string> files)
        {
            Clear();

            AddRange(files.Select(file => new PlaylistItem(MediaFormats, file, EPlaylistItemType.File)));
        }
    }
}

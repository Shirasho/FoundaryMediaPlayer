using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Foundary.Extensions;

namespace FoundaryMediaPlayer.Engine
{
    public interface IReadOnlyPlaylist : IReadOnlyList<FPlaylistItem>
    {
        /// <summary>
        /// The current playlist item.
        /// </summary>
        FPlaylistItem Current { get; }

        /// <summary>
        /// Whether to shuffle the playlist elements.
        /// </summary>
        bool bShuffle { get; }
    }

    /// <summary>
    /// Contains information about the current playlist.
    /// </summary>
    public class FPlaylist : List<FPlaylistItem>, IReadOnlyPlaylist
    {
        /// <summary>
        /// The supported media formats.
        /// </summary>
        public IReadOnlyMediaFormatCollection MediaFormats { get; }

        private bool _bShuffle;

        /// <inheritdoc />
        public FPlaylistItem Current { get; protected set; }

        /// <inheritdoc />
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
        /// 
        /// </summary>
        /// <param name="mediaFormats"></param>
        public FPlaylist(IReadOnlyMediaFormatCollection mediaFormats)
        {
            mediaFormats.Should().NotBeNullOrEmpty();

            MediaFormats = mediaFormats;
        }

        /// <summary>
        /// Removes all elements from the <see cref="FPlaylist"/>.
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
        public FPlaylistItem Next()
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

            AddRange(files.Select(file => new FPlaylistItem(MediaFormats, file, EPlaylistItemType.File)));
        }
    }
}

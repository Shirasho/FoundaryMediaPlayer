using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using FluentAssertions;

namespace FoundaryMediaPlayer.Engine
{
    /// <summary>
    /// 
    /// </summary>
    public enum EPlaylistItemType
    {
        /// <summary>
        /// File
        /// </summary>
        File,

        /// <summary>
        /// Device
        /// </summary>
        Device
    }

    /// <summary>
    /// A playlist item.
    /// </summary>
    public class FPlaylistItem
    {
        /// <summary>
        /// The type.
        /// </summary>
        public EPlaylistItemType Type { get; }

        /// <summary>
        /// The <see cref="FileInfo"/> for the item.
        /// </summary>
        public FileInfo File { get; }

        /// <summary>
        /// Subs.
        /// </summary>
        /// TODO: Verify instantiation.
        public IEnumerable<string> Subs {get; set;}

        /// <summary>
        /// The application media formats.
        /// </summary>
        protected IReadOnlyMediaFormatCollection MediaFormats { get; }

        /// <summary>
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="path"/> is of an unsupported file type.</exception>
        [SuppressMessage("ReSharper", "LocalizableElement")]
        public FPlaylistItem(IReadOnlyMediaFormatCollection mediaFormats, string path, EPlaylistItemType type)
            : this(mediaFormats, new FileInfo(path), type)
        {

        }

        /// <summary>
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="file"/> is of an unsupported file type.</exception>
        [SuppressMessage("ReSharper", "LocalizableElement")]
        public FPlaylistItem(IReadOnlyMediaFormatCollection mediaFormats, FileInfo file, EPlaylistItemType type)
        {
            mediaFormats.Should().NotBeNullOrEmpty();
            file.Should().NotBeNull();
            file.Exists.Should().BeTrue();

            if (!mediaFormats.Any(f => f.HasExtension(file.Extension.TrimStart('.'))))
            {
                throw new ArgumentException($"Unsupported file type {file.Extension}", nameof(file));
            }

            MediaFormats = mediaFormats;
            File = file;
            Type = type;
        }
    }
}

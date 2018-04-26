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
    public abstract class MediaData
    {
        /// <summary>
        /// The title of the data.
        /// </summary>
        public string Title { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MediaFileData : MediaData
    {
        /// <summary>
        /// The first file.
        /// </summary>
        public FileInfo FirstFile => Files?.First();

        /// <summary>
        /// The files.
        /// </summary>
        public IEnumerable<FileInfo> Files { get; }

        /// <summary>
        /// Subs.
        /// </summary>
        public IEnumerable<string> Subs {get;}

        /// <summary>
        /// The start time.
        /// </summary>
        public TimeSpan StartTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MediaFileData(string file, IEnumerable<string> subs = null)
            : this(new FileInfo(file), subs)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public MediaFileData(FileInfo file, IEnumerable<string> subs = null)
            : this(new [] {file}, subs)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public MediaFileData(IEnumerable<string> files, IEnumerable<string> subs = null)
            : this(files.Select(f => new FileInfo(f)), subs)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public MediaFileData(IEnumerable<FileInfo> files, IEnumerable<string> subs = null)
        {
            var f = files as FileInfo[] ?? files.ToArray();

            f.Should().NotBeNull();
            foreach (var file in f)
            {
                file.Exists.Should().BeTrue();
            }

            Files = f;
            subs = subs ?? new List<string>();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class MediaDvdData : MediaData
    {
        /// <summary>
        /// The file.
        /// </summary>
        public FileInfo File { get; }

        /// <summary>
        /// 
        /// </summary>
        protected MediaDvdData(FileInfo file)
        {
            file.Should().NotBeNull();
            file.Exists.Should().BeTrue();

            File = file;
        }
    }
}

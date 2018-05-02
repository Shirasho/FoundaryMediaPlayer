using System;
using System.Collections.Generic;
using System.IO;
using DirectShowLib.Dvd;

namespace FoundaryMediaPlayer.Engine
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class AMediaData
    {
        /// <summary>
        /// The title of the data.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The file.
        /// </summary>
        public FileInfo File { get; }

        /// <summary>
        /// The start time.
        /// </summary>
        public TimeSpan StartTime { get; set; }

        protected AMediaData(string filePath)
            : this(null, new FileInfo(filePath), TimeSpan.Zero)
        {

        }

        protected AMediaData(FileInfo file)
            : this(null, file, TimeSpan.Zero)
        {

        }

        protected AMediaData(string name, string filePath)
            : this(name, new FileInfo(filePath), TimeSpan.Zero)
        {

        }

        protected AMediaData(string name, FileInfo file)
            : this(name, file, TimeSpan.Zero)
        {

        }

        protected AMediaData(string filePath, TimeSpan startTime)
            : this(null, new FileInfo(filePath), startTime)
        {

        }

        protected AMediaData(FileInfo file, TimeSpan startTime)
            : this(null, file, startTime)
        {

        }

        protected AMediaData(string title, string filePath, TimeSpan startTime)
            : this(title, new FileInfo(filePath), startTime)
        {

        }

        protected AMediaData(string title, FileInfo file, TimeSpan startTime)
        {
            Title = title;
            File = file;
            StartTime = startTime;
        }
    }

    public class FMediaFileData : AMediaData
    {
        public IEnumerable<string> Subtitles { get; }

        public FMediaFileData(string file, IEnumerable<string> subs = null)
            : this(null, new FileInfo(file), TimeSpan.Zero, subs)
        {

        }

        public FMediaFileData(FileInfo file, IEnumerable<string> subs = null)
            : this(null, file, TimeSpan.Zero, subs)
        {

        }

        public FMediaFileData(string file, TimeSpan startTime, IEnumerable<string> subs = null)
            : this(null, new FileInfo(file), startTime, subs)
        {

        }

        public FMediaFileData(FileInfo file, TimeSpan startTime, IEnumerable<string> subs = null)
            : this(null, file, startTime, subs)
        {

        }

        public FMediaFileData(string title, string file, IEnumerable<string> subs = null)
            : this(title, new FileInfo(file), TimeSpan.Zero, subs)
        {

        }

        public FMediaFileData(string title, FileInfo file, IEnumerable<string> subs = null)
            : this(title, file, TimeSpan.Zero, subs)
        {

        }

        public FMediaFileData(string title, string file, TimeSpan startTime, IEnumerable<string> subs = null)
            : this(title, new FileInfo(file), startTime, subs)
        {

        }

        public FMediaFileData(string title, FileInfo file, TimeSpan startTime, IEnumerable<string> subs = null)
            : base(title, file, startTime)
        {
            Subtitles = subs;
        }
    }

    public class FMediaDvdData : AMediaData
    {
        /// <summary>
        /// The DVD state.
        /// </summary>
        public IDvdState DvdState { get; set; }

        public FMediaDvdData(string file, IDvdState dvdState = null)
            : this(null, new FileInfo(file), TimeSpan.Zero, dvdState)
        {

        }

        public FMediaDvdData(FileInfo file, IDvdState dvdState = null)
            : this(null, file, TimeSpan.Zero, dvdState)
        {

        }

        public FMediaDvdData(string file, TimeSpan startTime, IDvdState dvdState = null)
            : this(null, new FileInfo(file), startTime, dvdState)
        {

        }

        public FMediaDvdData(FileInfo file, TimeSpan startTime, IDvdState dvdState = null)
            : this(null, file, startTime, dvdState)
        {

        }

        public FMediaDvdData(string title, string file, IDvdState dvdState = null)
            : this(title, new FileInfo(file), TimeSpan.Zero, dvdState)
        {

        }

        public FMediaDvdData(string title, FileInfo file, IDvdState dvdState = null)
            : this(title, file, TimeSpan.Zero, dvdState)
        {

        }

        public FMediaDvdData(string title, string file, TimeSpan startTime, IDvdState dvdState = null)
            : this(title, new FileInfo(file), startTime, dvdState)
        {

        }

        public FMediaDvdData(string title, FileInfo file, TimeSpan startTime, IDvdState dvdState = null)
            : base(title, file, startTime)
        {
            DvdState = dvdState;
        }
    }

    public class FMediaDeviceData : AMediaData
    {
        /// <summary>
        /// The video input.
        /// </summary>
        public int VideoInput { get; set; } = -1;

        /// <summary>
        ///  The video channel.
        /// </summary>
        public int VideoChannel { get; set; } = -1;

        /// <summary>
        /// The audio input.
        /// </summary>
        public int AudioInput { get; set; } = -1;

        /// <inheritdoc />
        public FMediaDeviceData(string filePath) : base(filePath)
        {
        }

        /// <inheritdoc />
        public FMediaDeviceData(FileInfo file) : base(file)
        {
        }

        /// <inheritdoc />
        public FMediaDeviceData(string name, string filePath) : base(name, filePath)
        {
        }

        /// <inheritdoc />
        public FMediaDeviceData(string name, FileInfo file) : base(name, file)
        {
        }

        /// <inheritdoc />
        public FMediaDeviceData(string filePath, TimeSpan startTime) : base(filePath, startTime)
        {
        }

        /// <inheritdoc />
        public FMediaDeviceData(FileInfo file, TimeSpan startTime) : base(file, startTime)
        {
        }

        /// <inheritdoc />
        public FMediaDeviceData(string title, string filePath, TimeSpan startTime) : base(title, filePath, startTime)
        {
        }

        /// <inheritdoc />
        public FMediaDeviceData(string title, FileInfo file, TimeSpan startTime) : base(title, file, startTime)
        {
        }
    }
}

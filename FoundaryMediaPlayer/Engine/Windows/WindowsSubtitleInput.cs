using DirectShowLib;
using FoundaryMediaPlayer.Platforms.Windows;

namespace FoundaryMediaPlayer.Engine.Windows
{
    /// <summary>
    /// 
    /// </summary>
    internal class WindowsSubtitleInput
    {
        public ISubStream SubStream { get; }
        public IBaseFilter SourceFilter { get; }

        /// <summary>
        /// 
        /// </summary>
        public WindowsSubtitleInput()
            : this(null, null)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subStream"></param>
        public WindowsSubtitleInput(ISubStream subStream)
            : this(subStream, null)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subStream"></param>
        /// <param name="sourceFilter"></param>
        public WindowsSubtitleInput(ISubStream subStream, IBaseFilter sourceFilter)
        {
            SubStream = subStream;
            SourceFilter = sourceFilter;
        }
    }
}

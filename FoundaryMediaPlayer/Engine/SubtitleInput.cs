using DirectShowLib;
using FoundaryMediaPlayer.Interfaces;

namespace FoundaryMediaPlayer.Engine
{
    /// <summary>
    /// 
    /// </summary>
    public class FSubtitleInput
    {
        public ISubStream SubStream { get; }
        public IBaseFilter SourceFilter { get; }

        /// <summary>
        /// 
        /// </summary>
        public FSubtitleInput()
            : this(null, null)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subStream"></param>
        public FSubtitleInput(ISubStream subStream)
            : this(subStream, null)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subStream"></param>
        /// <param name="sourceFilter"></param>
        public FSubtitleInput(ISubStream subStream, IBaseFilter sourceFilter)
        {
            SubStream = subStream;
            SourceFilter = sourceFilter;
        }
    }
}

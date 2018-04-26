using System.Windows;
using FoundaryMediaPlayer.Configuration;
using Prism.Events;

namespace FoundaryMediaPlayer.Windows
{
    /// <summary>
    /// A <see cref="WindowBase"/> that has a visual way to output
    /// media to the user.
    /// </summary>
    public abstract class MediaOutputWindowBase : WindowBase
    {
        /// <summary>
        /// 
        /// </summary>
        public abstract UIElement MediaPlayerWrapper { get; }

        /// <inheritdoc />
        protected MediaOutputWindowBase(string windowName, IEventAggregator eventAggregator, Store store)
            : base(windowName, eventAggregator, store)
        {
        }

        /// <summary>
        /// Cleans up the media player wrapper output.
        /// </summary>
        public void DisposeMedia()
        {
            //TODO: Clean up output.
        }
    }
}

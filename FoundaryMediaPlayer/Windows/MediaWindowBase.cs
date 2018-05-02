using System.Windows;
using FoundaryMediaPlayer.Application;
using Prism.Events;

namespace FoundaryMediaPlayer.Windows
{
    /// <summary>
    /// A <see cref="AWindowBase"/> that has a visual way to output
    /// media to the user.
    /// </summary>
    public abstract class AMediaWindowBase : AWindowBase
    {
        /// <summary>
        /// 
        /// </summary>
        public abstract UIElement MediaPlayerWrapper { get; }

        /// <inheritdoc />
        protected AMediaWindowBase(string windowName, IEventAggregator eventAggregator, FApplicationStore store)
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

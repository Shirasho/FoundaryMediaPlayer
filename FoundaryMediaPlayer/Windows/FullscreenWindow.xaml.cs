using System;
using System.Windows;
using FoundaryMediaPlayer.Application;
using FoundaryMediaPlayer.Events;
using Prism.Events;

namespace FoundaryMediaPlayer.Windows
{
    /// <summary>
    /// Interaction logic for FullscreenWindow.xaml
    /// </summary>
    public partial class FullscreenWindow
    {
        /// <inheritdoc />
        public override UIElement MediaPlayerWrapper { get; }

        public FullscreenWindow(IEventAggregator eventAggregator, FApplicationStore store) 
            : base(nameof(FullscreenWindow), eventAggregator, store)
        {
            InitializeComponent();

            SubscribeEvent<FToggleFullScreenRequestEvent>(OnToggleFullScreen);

            GInputBindingManager.Monitor(this);
        }

        /// <inheritdoc />
        protected override void OnClosed(EventArgs e)
        {
            GInputBindingManager.Unmonitor(this);

            UnsubscribeEvent<FToggleFullScreenRequestEvent>(OnToggleFullScreen);

            base.OnClosed(e);
        }

        /// <summary>
        /// Called when the <see cref="EKeybindableEvent.ToggleFullscreen"/> event is called.
        /// </summary>
        /// <param name="e"></param>
        protected void OnToggleFullScreen(FToggleFullScreenRequestEvent e)
        {
            // This window should only be available when transitioning from bFullscreen to !bFullscreen.
            // As such all events of this type on this window should close the fullscreen window.
            if (e.IsEventSender(this))
            {
                Close();
            }
        }
    }
}

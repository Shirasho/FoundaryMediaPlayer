using System;
using System.Windows;
using FluentAssertions;
using FoundaryMediaPlayer.Configuration;
using FoundaryMediaPlayer.Events;
using FoundaryMediaPlayer.Input;
using Prism.Events;

namespace FoundaryMediaPlayer.Windows
{
    /// <summary>
    /// Interaction logic for FullscreenWindow.xaml
    /// </summary>
    public partial class FullscreenWindow
    {
        /// <inheritdoc />
        public override UIElement MediaPlayerWrapper => null;

        /// <inheritdoc />
        public FullscreenWindow(IEventAggregator eventAggregator, Store store) 
            : base(nameof(FullscreenWindow), eventAggregator, store)
        {
            InitializeComponent();
            
            EventAggregator.GetEvent<ToggleFullScreenKeyBindingEvent>().Subscribe(OnToggleFullScreen);

            InputBindingManager.Monitor(this);
        }

        /// <inheritdoc />
        protected override void OnClosed(EventArgs e)
        {
            InputBindingManager.Unmonitor(this);

            EventAggregator.GetEvent<ToggleFullScreenKeyBindingEvent>().Unsubscribe(OnToggleFullScreen);

            base.OnClosed(e);
        }

        /// <summary>
        /// Called when the <see cref="EKeybindableEvent.ToggleFullscreen"/> event is called.
        /// </summary>
        /// <param name="e"></param>
        protected void OnToggleFullScreen(ToggleFullScreenKeyBindingEvent e)
        {
            // This window should only be available when transitioning from bFullscreen to !bFullscreen.
            // As such all events of this type should close the fullscreen window.
            Close();
        }
    }
}

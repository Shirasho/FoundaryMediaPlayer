using System;
using System.Windows;
using System.Windows.Input;
using Foundary;
using Foundary.Extensions;
using FoundaryMediaPlayer.Configuration;
using FoundaryMediaPlayer.Contexts;
using FoundaryMediaPlayer.Events;
using Prism.Events;

namespace FoundaryMediaPlayer.Windows
{
    /// <summary>
    /// Interaction logic for ShellWindow.xaml
    /// </summary>
    public partial class ShellWindow
    {
        private static ShellWindow Instance { get; set; }
        private static readonly object _InstanceLock = new object();

        /// <inheritdoc />
        public override UIElement MediaPlayerWrapper => MediaPlayer;
        
        /// <summary></summary>
        /// <exception cref="RuntimeException">Only one shell window may be created.</exception>
        public ShellWindow(
            IEventAggregator eventAggregator,
            Store store)
            : base(nameof(ShellWindow), eventAggregator, store)
        {
            lock (_InstanceLock)
            {
                if (Instance != null)
                {
                    throw new RuntimeException("Only one shell window may be created.");
                }

                Instance = this;
            }

            InitializeComponent();

            SaveWindowPosition = true;

            MonitorKeyBindings(true);
            (DataContext as ShellWindowContext)?.SetOwner(this);

            EventAggregator.GetEvent<ToggleFullScreenKeyBindingEvent>().Subscribe(OnToggleFullScreen);
        }

        /// <inheritdoc />
        protected override void OnClosed(EventArgs e)
        {
            EventAggregator.GetEvent<ToggleFullScreenKeyBindingEvent>().Unsubscribe(OnToggleFullScreen);

            base.OnClosed(e);
        }

        /// <summary>
        /// Toggles fullscreen mode if the specified event
        /// </summary>
        /// <param name="e"></param>
        protected void OnToggleFullScreen(ToggleFullScreenKeyBindingEvent e)
        {
            (DataContext as ShellWindowContext)?.MediaEngine.ToggleFullscreen();
        }
        
        /// <inheritdoc />
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (Find.AnySatisfies<object>(
                source => ReferenceEquals(source, e.Source),
                MediaPlayerContainer,
                MediaPlayer))
            {
                EventAggregator.GetEvent<ToggleFullScreenKeyBindingEvent>().Publish(new ToggleFullScreenKeyBindingEvent(this));
                e.Handled = true;
            }
        }

        /// <inheritdoc />
        /// <remarks>
        /// Mousewheel is a pain in WPF. Instead of using MVVM we will capture the event here
        /// and forward it off.
        /// </remarks>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (Find.AnySatisfies<object>(
                source => ReferenceEquals(source, e.Source),
                VolumeControlContainer,
                VolumeControl,
                MediaPlayerContainer,
                MediaPlayer))
            {
                int sign = Math.Sign(e.Delta);

                // Ex. 53 with tick of 5 and change amount is -5, we want to lower it to 50.
                // This value will, in this example, get the actual amount we are changing by.
                int rounded = (Store.Player.Volume + Store.Player.VolumeTickFrequency) % Store.Player.VolumeTickFrequency;

                // If the rounded value is 0, it is already on the tick frequency, so we can apply
                // its full amount.
                if (rounded == 0)
                {
                    rounded = Store.Player.VolumeTickFrequency;
                }

                EventAggregator.GetEvent<VolumeChangeRequestEvent>().Publish(new VolumeChangeRequestEvent
                {
                    Data = rounded * sign,
                    NumberType = EPercentNumberType.NonNormalized,
                    ValueType = EValueType.Offset
                });

                e.Handled = true;
            }

            if (Find.AnySatisfies<object>(
                source => ReferenceEquals(source, e.Source),
                SeekControlContainer,
                SeekControl
            ))
            {
                //TODO: Seek.
                e.Handled = true;
            }

            base.OnMouseWheel(e);
        }
    }
}

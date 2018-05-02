using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Foundary;
using Foundary.Extensions;
using FoundaryMediaPlayer.Application;
using FoundaryMediaPlayer.Events;
using FoundaryMediaPlayer.Windows.Contexts;
using MahApps.Metro.Controls;
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
        public override UIElement MediaPlayerWrapper { get; }

        public ShellWindow(IEventAggregator eventAggregator, FApplicationStore store)
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
            (DataContext as FShellWindowContext)?.SetOwner(this);

            SubscribeEvent<FToggleFullScreenRequestEvent>(OnToggleFullScreen);
        }

        protected override void OnClosed(EventArgs e)
        {
            UnsubscribeEvent<FToggleFullScreenRequestEvent>(OnToggleFullScreen);

            base.OnClosed(e);

            // Unfortunately MahApps windows register themselves with the application.
            // The application would have been smart enough to quit when all primary
            // windows are closed, but MahApps prevents this. We need to close the
            // application manually here. Not as smart, but close enough.
            var windowsWeCareAbout = new List<MetroWindow>();
            foreach (var appWindows in FApplication.Current.Windows)
            {
                if (appWindows is MetroWindow window)
                {
                    windowsWeCareAbout.Add(window);
                }
            }

            if (windowsWeCareAbout.Count == 1 && windowsWeCareAbout[0] is ConsoleWindow)
            {
                FApplication.Current.Shutdown();
            }
        }

        /// <summary>
        /// Toggles fullscreen mode if the specified event
        /// </summary>
        /// <param name="e"></param>
        protected void OnToggleFullScreen(FToggleFullScreenRequestEvent e)
        {
            (DataContext as FShellWindowContext)?.MediaEngine.ToggleFullscreen();
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
                DispatchEvent(new FToggleFullScreenRequestEvent(this));
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
            MouseWheelDirection direction = (MouseWheelDirection)Math.Sign(e.Delta);

            if (Find.AnySatisfies<object>(
                source => ReferenceEquals(source, e.Source),
                VolumeControlContainer,
                VolumeControl,
                MediaPlayerContainer,
                MediaPlayer))
            {
                // Ex. 53 with tick of 5 and change amount is -5, we want to lower it to 50.
                // This value will, in this example, get the actual amount we are changing by.
                int rounded = (Store.Volume + Store.VolumeTickFrequency) % Store.VolumeTickFrequency;

                // If the rounded value is 0, it is already on the tick frequency, so we can apply
                // its full amount.
                if (rounded == 0)
                {
                    rounded = Store.VolumeTickFrequency;
                }

                DispatchEvent(new FVolumeChangeRequestEvent(rounded * (int)direction)
                {
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
                DispatchEvent(new FMediaSeekRequestEvent(Store.SeekTickFrequency * (int)direction)
                {
                    NumberType = EPercentNumberType.NonNormalized, 
                    ValueType = EValueType.Offset
                });
                e.Handled = true;
            }

            base.OnMouseWheel(e);
        }
    }
}

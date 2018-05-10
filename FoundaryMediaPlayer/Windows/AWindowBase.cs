using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using FluentAssertions;
using FoundaryMediaPlayer.Application;
using FoundaryMediaPlayer.Events;
using MahApps.Metro.Controls;
using Prism.Events;

namespace FoundaryMediaPlayer.Windows
{
    /// <summary>
    /// The base class for all windows in the <see cref="Windows"/> namespace
    /// that are a type of <see cref="MetroWindow"/>.
    /// </summary>
    public abstract class AWindowBase : MetroWindow, IDisposable
    {
        /// <summary>
        /// The application event aggregator.
        /// </summary>
        private IEventAggregator _EventAggregator { get; }

        /// <summary>
        /// The application store.
        /// </summary>
        protected FApplicationStore Store { get; }

        private bool _bDisposed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected AWindowBase(
            string windowName,
            IEventAggregator eventAggregator,
            FApplicationStore store)
        {
            windowName.Should().NotBeNullOrWhiteSpace();
            eventAggregator.Should().NotBeNull();
            store.Should().NotBeNull();

            Name = windowName;
            _EventAggregator = eventAggregator;
            Store = store;

            TitleCharacterCasing = CharacterCasing.Normal;
            WindowTransitionsEnabled = false;

            Loaded += OnLoaded;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!_bDisposed)
            {
                MonitorKeyBindings(false);

                OnDispose();
                _bDisposed = true;
            }
        }

        /// <summary>
        /// Disposes of resources.
        /// </summary>
        protected virtual void OnDispose()
        {

        }

        /// <summary>
        /// Dispatches the payload to an event.
        /// </summary>
        /// <typeparam name="TEventType">The event type.</typeparam>
        /// <param name="payload">The payload.</param>
        protected void DispatchEvent<TEventType>(TEventType payload)
            where TEventType : PubSubEvent<TEventType>, new()
        {
            _EventAggregator?.GetEvent<TEventType>().Publish(payload);
        }

        /// <summary>
        /// Subscribes to an event.
        /// </summary>
        protected void SubscribeEvent<TEventType>(Action<TEventType> callback)
            where TEventType : PubSubEvent<TEventType>, new()
        {
            _EventAggregator?.GetEvent<TEventType>().Subscribe(callback);
        }

        /// <summary>
        /// Unsubscribes from an event.
        /// </summary>
        protected void UnsubscribeEvent<TEventType>(Action<TEventType> callback)
            where TEventType : PubSubEvent<TEventType>, new()
        {
            _EventAggregator?.GetEvent<TEventType>().Unsubscribe(callback);
        }

        /// <summary>
        /// Set whether the window is monitoring for key binding strokes.
        /// </summary>
        protected void MonitorKeyBindings(bool bMonitor)
        {
            if (bMonitor)
            {
                GInputBindingManager.Monitor(this);
            }
            else
            {
                GInputBindingManager.Unmonitor(this);
            }
        }

        /// <inheritdoc />
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            DispatchEvent(new FWindowBlurEvent(this));
        }

        /// <inheritdoc />
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            DispatchEvent(new FWindowFocusEvent(this));
        }

        /// <inheritdoc />
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            DispatchEvent(new FWindowClosingEvent(this));
        }

        /// <inheritdoc />
        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            DispatchEvent(new FWindowStateChangedEvent(this));
        }

        /// <summary>
        /// Called when the window has finished loading.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnLoaded(object sender, RoutedEventArgs e)
        {
            
        }

        /// <inheritdoc />
        protected override void OnClosed(EventArgs e)
        {
            var context = DataContext as IDisposable;
            context?.Dispose();

            base.OnClosed(e);
        }
    }
}

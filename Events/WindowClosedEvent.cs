using FluentAssertions;
using log4net.Core;
using MahApps.Metro.Controls;
using System.Reflection;

namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// The event sent when a window is closing.
    /// </summary>
    public sealed class WindowClosingEvent : EventBase<MetroWindow, WindowClosingEvent>
    {
        /// <inheritdoc />
        protected override Level LoggingLevel { get; } = Level.Info;

        /// <inheritdoc />
        public WindowClosingEvent()
            : this(null)
        {

        }

        /// <inheritdoc />
        public WindowClosingEvent(MetroWindow data) 
            : base(data)
        {
            
        }

        /// <inheritdoc />
        protected override string GetLoggerMessage(WindowClosingEvent payload)
        {
            return $"Closing window {(payload.Data?.Name ?? payload.Data?.GetType().GetTypeInfo().Name ?? "[Unknown]")}.";
        }
    }

    /// <summary>
    /// The event sent when a window has closed.
    /// </summary>
    public sealed class WindowClosedEvent : EventBase<MetroWindow, WindowClosedEvent>
    {
        /// <inheritdoc />
        protected override Level LoggingLevel { get; } = Level.Info;

        /// <inheritdoc />
        public WindowClosedEvent()
            : this(null)
        {

        }

        /// <inheritdoc />
        public WindowClosedEvent(MetroWindow data) 
            : base(data)
        {

        }

        /// <inheritdoc />
        protected override string GetLoggerMessage(WindowClosedEvent payload)
        {
            return $"Window {(payload.Data?.Name ?? payload.Data?.GetType().GetTypeInfo().Name ?? "[Unknown]")} closed.";
        }
    }
}

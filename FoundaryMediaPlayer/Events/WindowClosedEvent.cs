using FluentAssertions;
using log4net.Core;
using MahApps.Metro.Controls;
using System.Reflection;

namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// The event sent when a window is closing.
    /// </summary>
    public sealed class FWindowClosingEvent : AEventBase<MetroWindow, FWindowClosingEvent>
    {
        /// <inheritdoc />
        protected override Level LoggingLevel { get; } = Level.Info;

        /// <inheritdoc />
        public FWindowClosingEvent()
            : this(null)
        {

        }

        /// <inheritdoc />
        public FWindowClosingEvent(MetroWindow data) 
            : base(data)
        {
            
        }

        /// <inheritdoc />
        protected override string GetLoggerMessage(FWindowClosingEvent payload)
        {
            return $"Closing window {(payload.Data?.Name ?? payload.Data?.GetType().GetTypeInfo().Name ?? "[Unknown]")}.";
        }
    }

    /// <summary>
    /// The event sent when a window has closed.
    /// </summary>
    public sealed class FWindowClosedEvent : AEventBase<MetroWindow, FWindowClosedEvent>
    {
        /// <inheritdoc />
        protected override Level LoggingLevel { get; } = Level.Info;

        /// <inheritdoc />
        public FWindowClosedEvent()
            : this(null)
        {

        }

        /// <inheritdoc />
        public FWindowClosedEvent(MetroWindow data) 
            : base(data)
        {

        }

        /// <inheritdoc />
        protected override string GetLoggerMessage(FWindowClosedEvent payload)
        {
            return $"Window {(payload.Data?.Name ?? payload.Data?.GetType().GetTypeInfo().Name ?? "[Unknown]")} closed.";
        }
    }
}

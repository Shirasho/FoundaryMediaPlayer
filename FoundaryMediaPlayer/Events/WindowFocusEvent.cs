using System.Reflection;
using log4net.Core;
using MahApps.Metro.Controls;

namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// The event sent when a window regains focus.
    /// </summary>
    public sealed class WindowFocusEvent : EventBase<MetroWindow, WindowFocusEvent>
    {
        /// <inheritdoc />
        protected override Level LoggingLevel { get; } = Level.Info;

        /// <inheritdoc />
        public WindowFocusEvent()
            : this(null)
        {

        }

        /// <inheritdoc />
        public WindowFocusEvent(MetroWindow data) : base(data)
        {
        }


        /// <inheritdoc />
        protected override string GetLoggerMessage(WindowFocusEvent payload)
        {
            return $"Window {(payload.Data?.Name ?? payload.Data?.GetType().GetTypeInfo().Name ?? "[Unknown]")} lost focus.";
        }
    }
}

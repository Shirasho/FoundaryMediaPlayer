using log4net.Core;
using MahApps.Metro.Controls;
using System.Reflection;

namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// The event sent when a window state has been changed.
    /// </summary>
    public sealed class WindowStateChangedEvent : EventBase<MetroWindow, WindowStateChangedEvent>
    {
        /// <inheritdoc />
        protected override Level LoggingLevel { get; } = Level.Info;

        /// <inheritdoc />
        public WindowStateChangedEvent()
            : this(null)
        {

        }

        /// <inheritdoc />
        public WindowStateChangedEvent(MetroWindow data) : base(data)
        {
        }


        /// <inheritdoc />
        protected override string GetLoggerMessage(WindowStateChangedEvent payload)
        {
            return $"Window ({payload.Data?.Name ?? payload.Data?.GetType().GetTypeInfo().Name ?? "[Unknown]"}) has changed state to {(payload.Data?.WindowState.ToString() ?? "[UnknownState]")}";
        }
    }
}

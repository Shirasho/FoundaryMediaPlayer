using log4net.Core;
using MahApps.Metro.Controls;
using System.Reflection;

namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// The event sent when a window state has been changed.
    /// </summary>
    public sealed class FWindowStateChangedEvent : AEventBase<MetroWindow, FWindowStateChangedEvent>
    {
        /// <inheritdoc />
        protected override Level LoggingLevel { get; } = Level.Info;

        /// <inheritdoc />
        public FWindowStateChangedEvent()
            : this(null)
        {

        }

        /// <inheritdoc />
        public FWindowStateChangedEvent(MetroWindow data) : base(data)
        {
        }


        /// <inheritdoc />
        protected override string GetLoggerMessage(FWindowStateChangedEvent payload)
        {
            return $"Window ({payload.Data?.Name ?? payload.Data?.GetType().GetTypeInfo().Name ?? "[Unknown]"}) has changed state to {(payload.Data?.WindowState.ToString() ?? "[UnknownState]")}";
        }
    }
}

using log4net.Core;
using MahApps.Metro.Controls;
using System.Reflection;

namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// The event sent when a window is blurred.
    /// </summary>
    public sealed class WindowBlurEvent : EventBase<MetroWindow, WindowBlurEvent>
    {
        /// <inheritdoc />
        protected override Level LoggingLevel { get; } = Level.Info;

        /// <inheritdoc />
        public WindowBlurEvent()
            : this(null)
        {

        }

        /// <inheritdoc />
        public WindowBlurEvent(MetroWindow data) : base(data)
        {
        }


        /// <inheritdoc />
        protected override string GetLoggerMessage(WindowBlurEvent payload)
        {
            return $"Window {(payload.Data?.Name ?? payload.Data?.GetType().GetTypeInfo().Name ?? "[Unknown]")} lost focus.";
        }
    }
}

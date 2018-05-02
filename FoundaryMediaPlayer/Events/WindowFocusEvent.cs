using System.Reflection;
using log4net.Core;
using MahApps.Metro.Controls;

namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// The event sent when a window regains focus.
    /// </summary>
    public sealed class FWindowFocusEvent : AEventBase<MetroWindow, FWindowFocusEvent>
    {
        /// <inheritdoc />
        protected override Level LoggingLevel { get; } = Level.Info;

        /// <inheritdoc />
        public FWindowFocusEvent()
            : this(null)
        {

        }

        /// <inheritdoc />
        public FWindowFocusEvent(MetroWindow data) : base(data)
        {
        }


        /// <inheritdoc />
        protected override string GetLoggerMessage(FWindowFocusEvent payload)
        {
            return $"Window {(payload.Data?.Name ?? payload.Data?.GetType().GetTypeInfo().Name ?? "[Unknown]")} lost focus.";
        }
    }
}

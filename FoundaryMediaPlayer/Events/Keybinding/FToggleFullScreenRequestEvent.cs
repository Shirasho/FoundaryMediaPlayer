using System.Reflection;
using System.Windows;
using log4net.Core;

namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// Toggle full screen event.
    /// </summary>
    public sealed class FToggleFullScreenRequestEvent : AKeyBindingEventBase<FToggleFullScreenRequestEvent>
    {
        /// <inheritdoc />
        protected override Level LoggingLevel { get; } = Level.Info;

        /// <summary>
        /// 
        /// </summary>
        public FToggleFullScreenRequestEvent()
            : base(EKeybindableEvent.ToggleFullscreen, null)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public FToggleFullScreenRequestEvent(Window focusedWindow)
            : base(EKeybindableEvent.ToggleFullscreen, focusedWindow)
        {
        }

        /// <inheritdoc />
        protected override string GetLoggerMessage(FToggleFullScreenRequestEvent payload)
        {
            return $"Window {(payload.Window?.Name ?? payload.Window?.GetType().GetTypeInfo().Name ?? "[Unknown]")} lost focus.";
        }
    }
}

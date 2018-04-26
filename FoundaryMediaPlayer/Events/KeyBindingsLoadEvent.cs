using System.Collections.Generic;
using System.Text;
using log4net.Core;

namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// The event sent when a window is loading its key bindings.
    /// </summary>
    public sealed class KeyBindingsLoadingEvent : EventBase<KeyBindingsLoadingEvent>
    {
        /// <inheritdoc />
        protected override Level LoggingLevel { get; } = Level.Info;

        /// <inheritdoc />
        protected override string GetLoggerMessage(KeyBindingsLoadingEvent payload)
        {
            return "Loading key bindings from store.";
        }
    }

    /// <summary>
    /// The event sent when a window has finished loading its key bindings.
    /// </summary>
    public sealed class KeyBindingsLoadedEvent : EventBase<IList<string>, KeyBindingsLoadedEvent>
    {
        /// <inheritdoc />
        protected override Level LoggingLevel => (Data?.Count ?? 0) == 0  ? Level.Info : Level.Warn;

        /// <inheritdoc />
        public KeyBindingsLoadedEvent()
            : this(null)
        {

        }

        /// <inheritdoc />
        public KeyBindingsLoadedEvent(IList<string> data) 
            : base(data)
        {
        }

        /// <inheritdoc />
        protected override string GetLoggerMessage(KeyBindingsLoadedEvent payload)
        {
            if (Data == null || Data.Count == 0)
            {
                return "Key bindings loaded from store successfully.";
            }

            var stringBuilder = new StringBuilder();
            foreach (var error in Data)
            {
                stringBuilder.Append($"{error}{NewMessageIndicator}");
            }

            return stringBuilder.ToString();
        }
    }
}

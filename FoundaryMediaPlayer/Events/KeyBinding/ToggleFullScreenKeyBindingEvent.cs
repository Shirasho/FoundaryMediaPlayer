using System.Windows;
using FoundaryMediaPlayer.Input;

namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// Toggle full screen event.
    /// </summary>
    public sealed class ToggleFullScreenKeyBindingEvent : KeyBindingEventBase<ToggleFullScreenKeyBindingEvent>
    {
        /// <summary>
        /// 
        /// </summary>
        public ToggleFullScreenKeyBindingEvent()
            : base(EKeybindableEvent.ToggleFullscreen, null)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public ToggleFullScreenKeyBindingEvent(Window focusedWindow)
            : base(EKeybindableEvent.ToggleFullscreen, focusedWindow)
        {
        }
    }
}

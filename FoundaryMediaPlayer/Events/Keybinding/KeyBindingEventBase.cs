using System.Windows;

namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// A key binding event.
    /// </summary>
    public abstract class AKeyBindingEventBase<TImpl> : AEventBase<EKeybindableEvent, TImpl>
        where TImpl : AKeyBindingEventBase<TImpl>, new()
    {
        /// <summary>
        /// The window that has focus at the time of key binding execution.
        /// </summary>
        public Window Window { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bindableEvent"></param>
        /// <param name="focusedWindow"></param>
        protected AKeyBindingEventBase(EKeybindableEvent bindableEvent, Window focusedWindow)
            : base(bindableEvent)
        {
            Window = focusedWindow;
        }

        /// <summary>
        /// Returns whether the specified window is the one that sent the event.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public bool IsEventSender(Window window)
        {
            return Window.Equals(window);
        }
    }
}

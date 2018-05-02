using System;
using System.Windows.Input;
using FoundaryMediaPlayer.Application;

namespace FoundaryMediaPlayer.Events
{
    public class KeyBindingEventArgs : EventArgs
    {
        public FMergedInputGesture KeyBinding { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="modifiers"></param>
        public KeyBindingEventArgs(Key key, ModifierKeys modifiers)
        {
            KeyBinding = new FMergedInputGesture(key, modifiers);
        }
    }
}

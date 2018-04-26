using System;
using System.Windows.Input;
using FoundaryMediaPlayer.Input;

namespace FoundaryMediaPlayer.Events
{
    internal class KeyBindingEventArgs : EventArgs
    {
        public MergedInputGesture KeyBinding { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="modifiers"></param>
        public KeyBindingEventArgs(Key key, ModifierKeys modifiers)
        {
            KeyBinding = new MergedInputGesture(key, modifiers);
        }
    }
}

using System;
using System.Windows.Input;
using FoundaryMediaPlayer.Input;

namespace FoundaryMediaPlayer.Configuration
{
    /// <summary>
    /// A key binding.
    /// </summary>
    [Serializable]
    public sealed class StoreKeyBinding
    {
        /// <summary>
        /// The ID of the event.
        /// </summary>
        public EKeybindableEvent Event { get; set; }

        /// <summary>
        /// The key to activate the hotkey.
        /// </summary>
        public Key Key { get; set; }

        /// <summary>
        /// The modifiers associated with the hotkey.
        /// </summary>
        public ModifierKeys Modifiers { get; set; }
    }
}

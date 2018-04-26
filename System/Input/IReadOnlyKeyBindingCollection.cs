using System.Collections.Generic;
using System.Windows.Input;

namespace FoundaryMediaPlayer.Input
{
    /// <summary>
    /// A read-only <see cref="KeyBindingCollection"/>.
    /// </summary>
    public interface IReadOnlyKeyBindingCollection : IReadOnlyDictionary<EKeybindableEvent, HashSet<MergedInputGesture>>
    {
        /// <summary>
        /// Returns whether the specified key binding is already registered.
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        bool IsBindingRegistered(MergedInputGesture binding);

        /// <summary>
        /// Returns the key binding associated with the bindable event.
        /// </summary>
        /// <param name="bindableEvent"></param>
        /// <returns></returns>
        HashSet<MergedInputGesture> GetKeyBindings(EKeybindableEvent bindableEvent);
    }
}

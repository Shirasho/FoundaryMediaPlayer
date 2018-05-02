using System.Collections.Generic;
using FoundaryMediaPlayer.Events;

namespace FoundaryMediaPlayer.Application
{
    /// <summary>
    /// A read-only <see cref="FKeyBindingCollection"/>.
    /// </summary>
    public interface IReadOnlyKeyBindingCollection : IReadOnlyDictionary<EKeybindableEvent, HashSet<FMergedInputGesture>>
    {
        /// <summary>
        /// Returns whether the specified key binding is already registered.
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        bool IsBindingRegistered(FMergedInputGesture binding);

        /// <summary>
        /// Returns the key binding associated with the bindable event.
        /// </summary>
        /// <param name="bindableEvent"></param>
        /// <returns></returns>
        HashSet<FMergedInputGesture> GetKeyBindings(EKeybindableEvent bindableEvent);
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FluentAssertions;
using Foundary.Collections;
using FoundaryMediaPlayer.Configuration;

namespace FoundaryMediaPlayer.Input
{
    /// <summary>
    /// A collection of key bindings.
    /// </summary>
    internal sealed class KeyBindingCollection : MultiValueDictionary<EKeybindableEvent, MergedInputGesture>, IReadOnlyKeyBindingCollection
    {
        /// <summary>
        /// Default key bindings for the application.
        /// </summary>
        public static IReadOnlyKeyBindingCollection DefaultBindings { get; } = new KeyBindingCollection
        {
            // Key, Modifier, Overridable, Priority --> all defaults should have 0 priority.
            {EKeybindableEvent.ToggleFullscreen, new MergedInputGesture(Key.Enter, ModifierKeys.Alt)},
            {EKeybindableEvent.IncreaseVolume, new MergedInputGesture(Key.VolumeUp, ModifierKeys.None)},
            {EKeybindableEvent.IncreaseVolume, new MergedInputGesture(Key.VolumeUp, ModifierKeys.None)},
            {EKeybindableEvent.DecreaseVolume, new MergedInputGesture(Key.VolumeDown, ModifierKeys.None)},
            {EKeybindableEvent.DecreaseVolume, new MergedInputGesture(Key.VolumeUp, ModifierKeys.None)},
        };

        /// <summary>
        /// 
        /// </summary>
        public KeyBindingCollection()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public KeyBindingCollection(KeyBindingCollection other)
            : base(other)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public KeyBindingCollection(IReadOnlyKeyBindingCollection other)
        {
            foreach (var element in other)
            {
                Add(element.Key, element.Value);
            }
        }

        /// <summary>
        /// Removes the value from all keys for the <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="value"></param>
        public void Remove(MergedInputGesture value)
        {
            foreach (var v in Values)
            {
                v.Remove(value);
            }
        }

        /// <summary>
        /// Removes the value with the specified key for the <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="bKeepUnassigned">Whether to keep the binding unassigned the next time the application starts up.</param>
        public void Remove(EKeybindableEvent key, bool bKeepUnassigned)
        {
            if (!ContainsKey(key))
            {
                return;
            }

            Remove(key);

            if (bKeepUnassigned)
            {
                Add(key, new MergedInputGesture(Key.None, priority: 1));
            }
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsBindingRegistered(MergedInputGesture binding)
        {
            if (!binding.IsValidBinding())
            {
                return true;
            }

            foreach (var values in Values)
            {
                foreach (var v in values)
                {
                    if (v.Key == binding.Key && v.Modifiers == binding.Modifiers && v.IsValidBinding())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <inheritdoc />
        public HashSet<MergedInputGesture> GetKeyBindings(EKeybindableEvent bindableEvent)
        {
            return TryGetValue(bindableEvent, out HashSet<MergedInputGesture> result) ? result : null;
        }

        /// <summary>
        /// Finds the first <see cref="EKeybindableEvent"/> that contains the specified binding.
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        public EKeybindableEvent? FindKey(MergedInputGesture binding)
        {
            foreach (var element in this)
            {
                if (element.Value.Contains(binding))
                {
                    return element.Key;
                }
            }

            return null;
        }

        /// <summary>
        /// Loads the key bindings from the store.
        /// </summary>
        /// <param name="store"></param>
        /// <returns>A list of outputs that need to be sent to the log regarding the loading process.</returns>
        internal IList<string> LoadBindingsFromStore(Store store)
        {
            store.Should().NotBeNull();

            var result = new List<string>();

            return result;

#pragma warning disable 162
            var storeBindings = store.Interface.Bindings;
            if (storeBindings.Count == 0)
            {
                return result;
            }

            var nonOverridables = DefaultBindings.Values.SelectMany(b => b.Where(kb => !kb.bCanOverride)).ToArray();

            // For every binding requested in the store...
            foreach (var storeBinding in storeBindings)
            {
                // If the binding does not exist
                if (!DefaultBindings.ContainsKey(storeBinding.Key))
                {
                    // Go on to the next binding.
                    continue;
                }

                var defaultKeyBindings = GetKeyBindings(storeBinding.Key);
                var storeKeyBindings = storeBinding.Value;

                // For every key binding requested in the store for this event...
                foreach (var storeKeyBinding in storeKeyBindings)
                {
                    // Check that the keybinding is not reserved (it should be overridable).
                    bool bCanOverride = nonOverridables.Any(b => storeKeyBinding.Equals(b));
                    if (!bCanOverride)
                    {
                        result.Add($"Unable to apply stored keybinding for event {storeBinding.Key} - new key binding [{storeKeyBinding.Key} , {storeKeyBinding.Modifiers}] is reserved.");
                        continue;
                    }

                    // We only support 2 key bindings per event at this time.
                    if (defaultKeyBindings.Count >= 2)
                    {
                        result.Add($"Unable to apply stored keybinding for event {storeBinding.Key} - the event already has the maximum number of supported bindings.");
                        continue;
                    }

                    // If the binding is not registered.
                    if (!IsBindingRegistered(storeKeyBinding))
                    {
                        //TODO: Finish up
                        var priority = storeKeyBinding.Priority;
                        /*   
                         *   if priority == 0 and (binding at 0 is overridable or does not exist)
                        *                      bind new binding to slot 0
                        *                  else
                        *                      log an elevation of priority due to existing non-overridable binding
                        *                      bind new binding to slot 1
                        */
                    }
                    else
                    {
                        /* 
                         * Unbind existing.
                         */

                        var priority = storeKeyBinding.Priority;

                        /*   if priority == 0 and (binding at 0 is overridable or does not exist)
                         *                      bind new binding to slot 0
                         *                  else
                         *                      log an elevation of priority due to existing non-overridable binding
                         *                      bind new binding to slot 1
                         */
                    }
                }
            }

            return result;
#pragma warning restore 162
        }
    }
}

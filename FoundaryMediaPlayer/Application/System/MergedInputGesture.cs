using System;
using System.Windows.Input;
using Newtonsoft.Json;

namespace FoundaryMediaPlayer.Application
{
    /// <summary>
    /// Key binding information.
    /// </summary>
    public class FMergedInputGesture : InputGesture, IEquatable<KeyGesture>, IEquatable<MouseGesture>, IEquatable<FMergedInputGesture>
    {
        /// <summary>
        /// The key.
        /// </summary>
        public Key Key { get; }

        /// <summary>
        /// Key modifiers.
        /// </summary>
        public ModifierKeys Modifiers { get; }

        /// <summary>
        /// The mouse wheel direction.
        /// </summary>
        public MouseWheelDirection MouseWheelDirection { get; }

        /// <summary>
        /// The mouse action.
        /// </summary>
        public MouseAction MouseAction { get; }

        /// <summary>
        /// The priority of the key binding. This value only matters when sorting
        /// multiple key bindings for the same event to determine what order they 
        /// are displayed in the GUI.
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Whether this key binding can be overridden.
        /// </summary>
        [JsonIgnore]
        public bool bCanOverride { get; }

        /// <summary>
        /// 
        /// </summary>
        public FMergedInputGesture(Key key, bool bCanOverride = true, int priority = 0)
            : this(key, ModifierKeys.None, bCanOverride, priority)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public FMergedInputGesture(MouseAction mouseAction, bool bCanOverride = true, int priority = 0)
            : this(mouseAction, ModifierKeys.None, bCanOverride, priority)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        public FMergedInputGesture(Key key, ModifierKeys modifiers, bool bCanOverride = true, int priority = 0)
            : this(key, MouseAction.None, modifiers, MouseWheelDirection.None, bCanOverride, priority)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public FMergedInputGesture(MouseAction mouseAction, ModifierKeys modifiers, bool bCanOverride = true, int priority = 0)
            : this(Key.None, mouseAction, modifiers, MouseWheelDirection.None, bCanOverride, priority)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public FMergedInputGesture(MouseWheelDirection mouseWheelDirection, ModifierKeys modifiers, bool bCanOverride = true, int priority = 0)
            : this(Key.None, MouseAction.None, modifiers, mouseWheelDirection, bCanOverride, priority)
        {

        }

        [JsonConstructor]
        private FMergedInputGesture(Key key, MouseAction mouseAction, ModifierKeys modifiers, MouseWheelDirection mouseWheelDirection, bool bCanOverride = true, int priority = 0)
        {
            if (!IsDefinedKey(key))
            {
                throw new ArgumentException(nameof(key));
            }

            if (!IsDefinedMouseAction(mouseAction))
            {
                throw new ArgumentException(nameof(mouseAction));
            }

            if (!IsDefinedMouseWheelDirection(mouseWheelDirection))
            {
                throw new ArgumentNullException(nameof(mouseWheelDirection));
            }

            Key = key;
            MouseAction = mouseAction;
            Modifiers = modifiers;
            MouseWheelDirection = mouseWheelDirection;
            this.bCanOverride = bCanOverride;
            Priority = priority;
        }

        /// <summary>
        /// Returns whether this key binding represents a valid key combination.
        /// </summary>
        /// <returns></returns>
        public bool IsValidBinding()
        {
            return !(Key == Key.None && MouseAction == MouseAction.None && MouseWheelDirection == MouseWheelDirection.None && Modifiers == ModifierKeys.None);
        }

        /// <inheritdoc />
        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            if (inputEventArgs is MouseEventArgs me)
            {
                var mouseAction = GetMouseAction(me);
                if (mouseAction != MouseAction.None)
                {
                    return MouseAction == mouseAction && Modifiers == Keyboard.Modifiers;
                }

                return me is MouseWheelEventArgs mwe && (MouseWheelDirection == MouseWheelDirection.Up && mwe.Delta > 0 ||
                                                         MouseWheelDirection == MouseWheelDirection.Down && mwe.Delta < 0) 
                                                     && Modifiers == Keyboard.Modifiers;
            }

            if (inputEventArgs is KeyEventArgs ke && IsDefinedKey(ke.Key))
            {
                return Key == ke.Key && Modifiers == Keyboard.Modifiers;
            }

            return false;
        }

        private static bool IsDefinedMouseWheelDirection(MouseWheelDirection mouseWheelDirection)
        {
            return mouseWheelDirection >= MouseWheelDirection.Down && mouseWheelDirection <= MouseWheelDirection.Up;
        }

        private static bool IsDefinedMouseAction(MouseAction mouseAction)
        {
            return mouseAction >= MouseAction.None && mouseAction <= MouseAction.MiddleDoubleClick;
        }

        internal static bool IsDefinedKey(Key key)
        {
            return (key >= Key.None && key <= Key.OemClear);
        }

        private static MouseAction GetMouseAction(InputEventArgs inputArgs)
        {
            MouseAction mouseAction = MouseAction.None;

            if (inputArgs is MouseEventArgs)
            {
                if (inputArgs is MouseWheelEventArgs)
                {
                    mouseAction = MouseAction.WheelClick;
                }
                else
                {
                    MouseButtonEventArgs args = inputArgs as MouseButtonEventArgs;

                    switch (args?.ChangedButton)
                    {
                        case MouseButton.Left:
                            {
                                if (args.ClickCount == 2)
                                    mouseAction = MouseAction.LeftDoubleClick;
                                else if (args.ClickCount == 1)
                                    mouseAction = MouseAction.LeftClick;
                            }
                            break;

                        case MouseButton.Right:
                            {
                                if (args.ClickCount == 2)
                                    mouseAction = MouseAction.RightDoubleClick;
                                else if (args.ClickCount == 1)
                                    mouseAction = MouseAction.RightClick;
                            }
                            break;

                        case MouseButton.Middle:
                            {
                                if (args.ClickCount == 2)
                                    mouseAction = MouseAction.MiddleDoubleClick;
                                else if (args.ClickCount == 1)
                                    mouseAction = MouseAction.MiddleClick;
                            }
                            break;
                    }
                }
            }
            return mouseAction;
        }

        /// <inheritdoc />
        public bool Equals(KeyGesture other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return MouseAction == MouseAction.None && MouseWheelDirection == MouseWheelDirection.None &&
                Key == other.Key && Modifiers == other.Modifiers;
        }

        /// <inheritdoc />
        public bool Equals(MouseGesture other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return Key == Key.None && MouseWheelDirection == MouseWheelDirection.None &&
                MouseAction == other.MouseAction && Modifiers == other.Modifiers;
        }

        /// <inheritdoc />
        public bool Equals(FMergedInputGesture other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Key == other.Key &&
                   Modifiers == other.Modifiers &&
                   MouseWheelDirection == other.MouseWheelDirection &&
                   MouseAction == other.MouseAction;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((FMergedInputGesture)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)Key;
                hashCode = (hashCode * 397) ^ (int)Modifiers;
                hashCode = (hashCode * 397) ^ (int)MouseWheelDirection;
                hashCode = (hashCode * 397) ^ (int)MouseAction;
                return hashCode;
            }
        }
    }
}

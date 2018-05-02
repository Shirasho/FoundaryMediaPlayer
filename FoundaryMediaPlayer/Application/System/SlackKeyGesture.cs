using System;
using System.ComponentModel;
using System.Windows.Input;

namespace FoundaryMediaPlayer.Application
{
    public sealed class FSlackKeyGesture : InputGesture
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------
        #region Constructors
        /// <summary>
        ///  constructor
        /// </summary>
        /// <param name="key">key</param>
        public FSlackKeyGesture(Key key)
            : this(key, ModifierKeys.None)
        {
        }

        /// <summary>
        ///  constructor
        /// </summary>
        /// <param name="modifiers">modifiers</param>
        /// <param name="key">key</param>
        public FSlackKeyGesture(Key key, ModifierKeys modifiers)
            : this(key, modifiers, String.Empty, true)
        {
        }

        /// <summary>
        ///  constructor
        /// </summary>
        /// <param name="modifiers">modifiers</param>
        /// <param name="key">key</param>
        /// <param name="displayString">display string</param>
        public FSlackKeyGesture(Key key, ModifierKeys modifiers, string displayString)
            : this(key, modifiers, displayString, true)
        {
        }


        /// <summary>
        /// Internal constructor used by KeyBinding to avoid key and modifier validation
        /// This allows setting KeyBinding.Key and KeyBinding.Modifiers without regard
        /// to order.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="modifiers">Modifiers</param>
        /// <param name="validateGesture">If true, throws an exception if the key and modifier are not valid</param>
        internal FSlackKeyGesture(Key key, ModifierKeys modifiers, bool validateGesture)
            : this(key, modifiers, String.Empty, validateGesture)
        {
        }

        /// <summary>
        /// Private constructor that does the real work.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="modifiers">Modifiers</param>
        /// <param name="displayString">display string</param>
        /// <param name="validateGesture">If true, throws an exception if the key and modifier are not valid</param>
        private FSlackKeyGesture(Key key, ModifierKeys modifiers, string displayString, bool validateGesture)
        {
            if (!ModifierKeysConverter.IsDefinedModifierKeys(modifiers))
                throw new InvalidEnumArgumentException(nameof(modifiers), (int)modifiers, typeof(ModifierKeys));

            if (!IsDefinedKey(key))
                throw new InvalidEnumArgumentException(nameof(key), (int)key, typeof(Key));

            if (validateGesture && !IsValid(key, modifiers))
            {
                throw new NotSupportedException("Invalid key modifier.");
            }

            Modifiers = modifiers;
            Key = key;
            DisplayString = displayString ?? throw new ArgumentNullException(nameof(displayString));
        }
        #endregion Constructors

        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------
        #region Public Methods
        /// <summary>
        /// Modifier
        /// </summary>
        public ModifierKeys Modifiers { get; }

        /// <summary>
        /// Key
        /// </summary>
        public Key Key { get; }

        /// <summary>
        /// DisplayString
        /// </summary>
        public string DisplayString { get; }

        /// <summary>
        /// Compares InputEventArgs with current Input
        /// </summary>
        /// <param name="targetElement">the element to receive the command</param>
        /// <param name="inputEventArgs">inputEventArgs to compare to</param>
        /// <returns>True - KeyGesture matches, false otherwise.
        /// </returns>
        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            if (inputEventArgs is KeyEventArgs keyEventArgs && IsDefinedKey(keyEventArgs.Key))
            {
                return Key == keyEventArgs.Key && (Modifiers == Keyboard.Modifiers);
            }
            return false;
        }

        // Check for Valid enum, as any int can be casted to the enum.
        internal static bool IsDefinedKey(Key key)
        {
            return (key >= Key.None && key <= Key.OemClear);
        }

        #endregion Public Methods

        //------------------------------------------------------
        //
        //  Internal Methods
        //
        //------------------------------------------------------
        #region Internal Methods
        ///<summary>
        /// Is Valid Keyboard input to process for commands
        ///</summary>
        internal static bool IsValid(Key key, ModifierKeys modifiers, bool bUseInternalMethod = false)
        {
            if (!bUseInternalMethod ||
                //  Don't enforce any rules on the Function keys or on the number pad keys.
                (key >= Key.F1 && key <= Key.F24 || key >= Key.NumPad0 && key <= Key.Divide))
            {
                return true;
            }
            
            //  We check whether Control/Alt/Windows key is down for modifiers. We don't check
            //  for shift at this time as Shift with any combination is already covered in above check.
            //  Shift alone as modifier case, we defer to the next condition to avoid conflicing with
            //  TextInput.

            if ((modifiers & (ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Windows)) == 0)
            {
                return (key < Key.D0 || key > Key.D9) && (key < Key.A || key > Key.Z);
            }

            switch (key)
            {
                case Key.LeftCtrl:
                case Key.RightCtrl:
                case Key.LeftAlt:
                case Key.RightAlt:
                case Key.LWin:
                case Key.RWin:
                    return false;

                default:
                    return true;
            }
        }

        #endregion Internal Methods
    }
}

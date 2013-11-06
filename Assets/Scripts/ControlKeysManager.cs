namespace Assets.Scripts
{
    using System.Collections.Generic;
    using UI;
    using UnityEngine;

    /// <summary>
    /// Manages a set of control (keyboard) keys.
    /// </summary>
    /// <remarks>
    /// Used for Characters.
    /// </remarks>
    public class ControlKeysManager : System.ICloneable
    {
        #region private fields
        private static readonly Dictionary<KeyCode, string> JoystickButtons = new Dictionary<KeyCode, string>();

        /// <summary>
        /// The prefix used when storing keys in the configuration file.
        /// </summary>
        private string _configPrefix = "Input.Keys.Player";

        private KeyCode _attackKey;

        private KeyCode _defendKey;

        private KeyCode _jumpKey;

        private KeyCode _pickupKey;

        private KeyCode _dropItemKey;

        private KeyCode _runKey;

        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlKeysManager"/> class.
        /// Sets the specified control keys.
        /// </summary>
        /// <param name="playerNo">Number of player these keys belong to</param>
        /// <param name="attackKey">Attack key</param>
        /// <param name="defendKey">Defend key</param>
        /// <param name="jumpKey">Jump key</param>
        /// <param name="pickupKey">Pickup key</param>
        /// <param name="dropItemKey">Drop key</param>
        /// <param name="runKey">Run key</param>
        /// <param name="forwardKey">Key for forward movement</param>
        /// <param name="backwardKey">Key for backward movement</param>
        /// <param name="leftKey">Key for movement to the left</param>
        /// <param name="rightKey">Key for movement to the right</param>
        /// <param name="cheatKey">Key for opening the cheat console, see <see cref="CheatKey"/></param>
        public ControlKeysManager(int playerNo,
            KeyCode attackKey,
            KeyCode defendKey,
            KeyCode jumpKey,
            KeyCode pickupKey,
            KeyCode dropItemKey,
            KeyCode runKey,
            KeyCode forwardKey,
            KeyCode backwardKey,
            KeyCode leftKey,
            KeyCode rightKey,
            KeyCode cheatKey)
        {
            PlayerNo = playerNo;

            AttackKey = attackKey;
            DefendKey = defendKey;
            JumpKey = jumpKey;
            PickupKey = pickupKey;
            DropItemKey = dropItemKey;
            RunKey = runKey;
            ForwardKey = forwardKey;
            BackwardKey = backwardKey;
            LeftKey = leftKey;
            RightKey = rightKey;
            CheatKey = cheatKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlKeysManager"/> class.
        /// Does not set any control keys.
        /// </summary>
        /// <param name="playerNo">Number of player these keys belong to</param>
        public ControlKeysManager(int playerNo)
        {
            PlayerNo = playerNo;
        }
        #endregion

        #region public properties
        /// <summary>
        /// Gets or sets the prefix used when storing keys in the configuration file.
        /// </summary>
        /// <remarks><see cref="PlayerNo"/> gets appended automatically in the getter.</remarks>
        public string ConfigPrefix
        {
            get
            {
                return _configPrefix + PlayerNo;
            }

            set
            {
                _configPrefix = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of the player these keys belong to.
        /// </summary>
        public int PlayerNo
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the KeyCode for the attack key.
        /// </summary>
        public KeyCode AttackKey
        {
            get
            {
                return _attackKey;
            }

            set
            {
                _attackKey = value;
                JoystickButtons[value] = "Attack" + PlayerNo;
            }
        }

        /// <summary>
        /// Gets or sets the KeyCode for the defend key.
        /// </summary>
        public KeyCode DefendKey
        {
            get
            {
                return _defendKey;
            }

            set
            {
                _defendKey = value;
                JoystickButtons[value] = "Defend" + PlayerNo;
            }
        }

        /// <summary>
        /// Gets or sets the KeyCode for the jump key.
        /// </summary>
        public KeyCode JumpKey
        {
            get
            {
                return _jumpKey;
            }

            set
            {
                _jumpKey = value;
                JoystickButtons[value] = "Jump" + PlayerNo;
            }
        }

        /// <summary>
        /// Gets or sets the KeyCode used to pick up/use items.
        /// </summary>
        public KeyCode PickupKey
        {
            get
            {
                return _pickupKey;
            }

            set
            {
                _pickupKey = value;
                JoystickButtons[value] = "ItemPickupUse" + PlayerNo;
            }
        }

        /// <summary>
        /// Gets or sets the KeyCode used to drop items.
        /// </summary>
        public KeyCode DropItemKey
        {
            get
            {
                return _dropItemKey;
            }

            set
            {
                _dropItemKey = value;
                JoystickButtons[value] = "ItemDrop" + PlayerNo;
            }
        }

        /// <summary>
        /// Gets or sets the KeyCode used to drop items.
        /// </summary>
        public KeyCode RunKey
        {
            get
            {
                return _runKey;
            }

            set
            {
                _runKey = value;
                JoystickButtons[value] = "Run" + PlayerNo;
            }
        }

        /// <summary>
        /// Gets or sets the KeyCode for forward movement.
        /// </summary>
        public KeyCode ForwardKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the KeyCode for backward movement.
        /// </summary>
        public KeyCode BackwardKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the KeyCode for movement to the left.
        /// </summary>
        public KeyCode LeftKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the KeyCode for movement to the right.
        /// </summary>
        public KeyCode RightKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the KeyCode for opening the cheat console.
        /// </summary>
        /// <remarks>
        /// Currently unused! There is a global cheat key (F1), see <see cref="CheatBox"/>.
        /// </remarks>
        public KeyCode CheatKey
        {
            get;
            set;
        }

        /// <summary>
        /// Tests if the Key or the corresponding button is pressed
        /// </summary>
        /// <param name="keyCode">the keycode to test</param>
        /// <returns>if the button is pressed</returns>
        public static bool GetKeyDown(KeyCode keyCode)
        {
            bool joyStickDown = false;
            if (JoystickButtons.ContainsKey(keyCode))
            {
                if (JoystickButtons[keyCode].EndsWith("1"))
                {
                    if (Input.GetJoystickNames().Length > 0)
                    {
                        joyStickDown = Input.GetButtonDown(JoystickButtons[keyCode]);
                    }
                }
                else
                {
                    if (Input.GetJoystickNames().Length > 1)
                    {
                        joyStickDown = Input.GetButtonDown(JoystickButtons[keyCode]);
                    }
                }
            }

            return Input.GetKeyDown(keyCode) || joyStickDown;
        }

        /// <summary>
        /// Tests if the Key or the corresponding button is hold down
        /// </summary>
        /// <param name="keyCode">the keycode to test</param>
        /// <returns>if the button is hold down</returns>
        public static bool GetKey(KeyCode keyCode)
        {
            bool joyStickDown = false;
            if (JoystickButtons.ContainsKey(keyCode))
            {
                if (JoystickButtons[keyCode].EndsWith("1"))
                {
                    if (Input.GetJoystickNames().Length > 0)
                    {
                        joyStickDown = Input.GetButton(JoystickButtons[keyCode]);
                    }
                }
                else
                {
                    if (Input.GetJoystickNames().Length > 1)
                    {
                        joyStickDown = Input.GetButton(JoystickButtons[keyCode]);
                    }
                }
            }

            return Input.GetKey(keyCode) || joyStickDown;
        }

        #endregion

        #region public methods
        /// <summary>
        /// Save the keys to the configuration file.
        /// </summary>
        /// <returns>True on success; false on failure.</returns>
        public bool SaveKeys()
        {
            // Oh well, this is not very nice.
            PlayerPrefs.SetInt(ConfigPrefix + ".Attack", (int)AttackKey);
            PlayerPrefs.SetInt(ConfigPrefix + ".Defend", (int)DefendKey);
            PlayerPrefs.SetInt(ConfigPrefix + ".Jump", (int)JumpKey);
            PlayerPrefs.SetInt(ConfigPrefix + ".Pickup", (int)PickupKey);
            PlayerPrefs.SetInt(ConfigPrefix + ".Drop", (int)DropItemKey);
            PlayerPrefs.SetInt(ConfigPrefix + ".MoveForward", (int)ForwardKey);
            PlayerPrefs.SetInt(ConfigPrefix + ".MoveBackward", (int)BackwardKey);
            PlayerPrefs.SetInt(ConfigPrefix + ".MoveLeft", (int)LeftKey);
            PlayerPrefs.SetInt(ConfigPrefix + ".MoveRight", (int)RightKey);
            PlayerPrefs.SetInt(ConfigPrefix + ".Cheat", (int)CheatKey);

            return true;
        }

        /// <summary>
        /// Load the keys from the configuration file. Uses PlayerNo.
        /// </summary>
        /// <returns>True on success, false on failure.</returns>
        public bool LoadKeys()
        {
            // Oh well, this is not very nice.
            AttackKey = (KeyCode)PlayerPrefs.GetInt(ConfigPrefix + ".Attack", (int)AttackKey);
            DefendKey = (KeyCode)PlayerPrefs.GetInt(ConfigPrefix + ".Defend", (int)DefendKey);
            JumpKey = (KeyCode)PlayerPrefs.GetInt(ConfigPrefix + ".Jump", (int)JumpKey);
            PickupKey = (KeyCode)PlayerPrefs.GetInt(ConfigPrefix + ".Pickup", (int)PickupKey);
            DropItemKey = (KeyCode)PlayerPrefs.GetInt(ConfigPrefix + ".Drop", (int)DropItemKey);
            ForwardKey = (KeyCode)PlayerPrefs.GetInt(ConfigPrefix + ".MoveForward", (int)ForwardKey);
            BackwardKey = (KeyCode)PlayerPrefs.GetInt(ConfigPrefix + ".MoveBackward", (int)BackwardKey);
            LeftKey = (KeyCode)PlayerPrefs.GetInt(ConfigPrefix + ".MoveLeft", (int)LeftKey);
            RightKey = (KeyCode)PlayerPrefs.GetInt(ConfigPrefix + ".MoveRight", (int)RightKey);
            CheatKey = (KeyCode)PlayerPrefs.GetInt(ConfigPrefix + ".Cheat", (int)CheatKey);
            return true;
        }

        /// <summary>
        /// Create a shallow copy of this Object.
        /// </summary>
        /// <returns>Shallow copy of this Object</returns>
        public System.Object Clone()
        {
            return MemberwiseClone();
        }
        #endregion
    }
}

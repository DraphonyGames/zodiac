namespace Assets.Scripts
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Manages the game's configuration.
    /// </summary>
    /// <remarks>
    /// In particular, this allows for retrieving and settings
    /// configuration items.
    /// </remarks>
    public sealed class ConfigManager
    {
        #region private fields
        /// <summary>
        /// The only instance of this singleton.
        /// </summary>
        private static ConfigManager _instance;

        /// <summary>t
        /// The control keys for all the players.
        /// </summary>
        private Dictionary<int, ControlKeysManager> _playerControlKeys;

        /// <summary>
        /// The default keybindings for player 1.
        /// </summary>
        private ControlKeysManager _defaultControlKeysPlayer1 = 
            new ControlKeysManager(1,
                KeyCode.R,
                KeyCode.T,
                KeyCode.Z,
                KeyCode.Alpha5,
                KeyCode.Alpha6,
                KeyCode.LeftShift,
                KeyCode.W,
                KeyCode.S,
                KeyCode.A,
                KeyCode.D,
                KeyCode.F1);

        /// <summary>
        /// The default keybindings for player 2.
        /// </summary>
        private ControlKeysManager _defaultControlKeysPlayer2 = 
            new ControlKeysManager(2,
                KeyCode.J,
                KeyCode.K,
                KeyCode.L,
                KeyCode.I,
                KeyCode.O,
                KeyCode.Keypad0,
                KeyCode.UpArrow,
                KeyCode.DownArrow,
                KeyCode.LeftArrow,
                KeyCode.RightArrow,
                KeyCode.F1);
        #endregion

        #region constructors
        /// <summary>
        /// Prevents a default instance of the <see cref="ConfigManager"/>
        /// class from being created. Loads the default (hard-coded) configuration.
        /// </summary>
        /// <remarks>
        /// Use GetInstance() to obtain an instance of this class.
        /// </remarks>
        private ConfigManager()
        {
            _playerControlKeys = new Dictionary<int, ControlKeysManager>();

            // First, load the default config.
            LoadDefaultConfig();

            // Then, try to override it with the saved values. Not all values may be
            // saved yet in the config stored on disk -- that's why we load default values
            // so that there is a valid config at all times.
            LoadConfig();
        }
        #endregion

        #region public properties
        /// <summary>
        /// Gets or sets the default keybindings for player 1
        /// </summary>
        public ControlKeysManager DefaultPlayer1
        {
            get { return _defaultControlKeysPlayer1; }
            set { _defaultControlKeysPlayer1 = value; }
        }

        /// <summary>
        /// Gets or sets the default keybindungs for player 2
        /// </summary>
        public ControlKeysManager DefaultPlayer2
        {
            get { return _defaultControlKeysPlayer2; }
            set { _defaultControlKeysPlayer2 = value; }
        }

        /// <summary>
        /// Gets or sets
        /// the Sound Effect Level for all sfx objects
        /// </summary>
        public float SoundLevel { get; set; }

        /// <summary>
        /// Gets or sets
        /// the Music Level for all music objects
        /// </summary>
        public float MusicLevel { get; set; }
        #endregion

        #region public methods
        /// <summary>
        /// Beautifies the default KeyCode.ToString()
        /// </summary>
        /// <param name="key">The keycode</param>
        /// <returns>The keycode to string</returns>
        public static string KeyToString(KeyCode key)
        {
            return key.ToString().Replace("Alpha", string.Empty).Replace("Keypad", "NUM").Replace("Arrow", string.Empty);
        }

        /// <summary>
        /// Get the only instance of this class (singleton).
        /// </summary>
        /// <returns>The only instance of this class.</returns>
        public static ConfigManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ConfigManager();
            }

            return _instance;
        }

        /// <summary>
        /// Get the ControlKeys for a certain player.
        /// </summary>
        /// <param name="player">Player ID</param>
        /// <returns>The player's ControlKeys or NULL if none set.</returns>
        public ControlKeysManager GetControlKeysForPlayer(int player)
        {
            ControlKeysManager keys;

            if (!_playerControlKeys.TryGetValue(player, out keys))
            {
                return null;
            }

            return keys;
        }

        /// <summary>
        /// Try to load a configuration file.
        /// </summary>
        /// <remarks>
        /// You should not call this every time you use the ConfigManager. This method
        /// is already called automatically when the ConfigManager object is created.
        /// </remarks>
        /// <returns>true on success, false on failure</returns>
        public bool LoadConfig()
        {
            // Load player keys.
            foreach (ControlKeysManager keys in _playerControlKeys.Values)
            {
                if (!keys.LoadKeys())
                {
                    return false;
                }
            }

            // Load sound levels.
            SoundLevel = PlayerPrefs.GetFloat("Audio.Output.SoundLevel", SoundLevel);
            MusicLevel = PlayerPrefs.GetFloat("Audio.Output.MusicLevel", MusicLevel);

            return true;
        }

        /// <summary>
        /// Save the current configuration to the config file.
        /// </summary>
        /// <returns>true on success, false on failure</returns>
        public bool SaveConfig()
        {
            // Save player keys.
            foreach (ControlKeysManager keys in _playerControlKeys.Values)
            {
                if (!keys.SaveKeys())
                {
                    return false;
                }
            }

            // Save sound levels.
            PlayerPrefs.SetFloat("Audio.Output.SoundLevel", SoundLevel);
            PlayerPrefs.SetFloat("Audio.Output.MusicLevel", MusicLevel);

            return true;
        }

        /// <summary>
        /// Set the ControlKeys for a player.
        /// </summary>
        /// <param name="player">Player ID</param>
        /// <param name="keys">Control keys</param>
        public void SetControlKeysForPlayer(int player, ControlKeysManager keys)
        {
            _playerControlKeys[player] = keys;
        }

        #endregion

        #region private methods
        /// <summary>
        /// Load the default (hard-coded) configuration.
        /// </summary>
        private void LoadDefaultConfig()
        {
            // Default control keys for player 1
            SetControlKeysForPlayer(1, (ControlKeysManager)DefaultPlayer1.Clone());

            // Default control keys for player 2
            SetControlKeysForPlayer(2, (ControlKeysManager)DefaultPlayer2.Clone());

            // Music and sound levels.
            SoundLevel = 100;
            MusicLevel = 20;
        }
        #endregion
    }
}

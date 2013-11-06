namespace Assets.Scripts.UI
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// UI used for the KeySettingsMenu
    /// </summary>
    public class KeySettings : MonoBehaviour
    {
        /// <summary>
        /// the GUISkin used
        /// </summary>
        private GUISkin _zodiac;

        /// <summary>
        /// the GUISkin used
        /// </summary>
        private GUISkin _mySkin2;

        private float _settingsWidth = Screen.width / 10;
        private float _settingsHeight = Screen.height / 10;

        private string _leftKeyP1;
        private string _rightKeyP1;
        private string _upKeyP1;
        private string _downKeyP1;
        private string _attackKeyP1;
        private string _defendKeyP1;
        private string _jumpKeyP1;
        private string _pickUpKeyP1;
        private string _dropKeyP1;

        private string _leftKeyP2;
        private string _rightKeyP2;
        private string _upKeyP2;
        private string _downKeyP2;
        private string _attackKeyP2;
        private string _defendKeyP2;
        private string _jumpKeyP2;
        private string _pickUpKeyP2;
        private string _dropKeyP2;

        private bool _pickUpToggle1;
        private bool _dropKeyToggle1;
        private bool _leftKeyToggle1;
        private bool _rightKeyToggle1;
        private bool _upKeyToggle1;
        private bool _downKeyToggle1;
        private bool _attackKeyToggle1;
        private bool _defendKeyToggle1;
        private bool _useKeyToggle1;
        private bool _jumpKeyToggle1;

        private bool _pickUpToggle2;
        private bool _dropKeyToggle2;
        private bool _leftKeyToggle2;
        private bool _rightKeyToggle2;
        private bool _upKeyToggle2;
        private bool _downKeyToggle2;
        private bool _attackKeyToggle2;
        private bool _defendKeyToggle2;
        private bool _useKeyToggle2;
        private bool _jumpKeyToggle2;

        private ControlKeysManager _player1;
        private ControlKeysManager _player2;
        private ControlKeysManager _default1;
        private ControlKeysManager _default2;

        private Event _keyEvent;

        /// <summary>
        /// initializing the values
        /// </summary>
        public void Start()
        {
            _default1 = ConfigManager.GetInstance().DefaultPlayer1;
            _default2 = ConfigManager.GetInstance().DefaultPlayer2;
            _player1 = (ControlKeysManager)ConfigManager.GetInstance().GetControlKeysForPlayer(1).Clone();
            _player2 = (ControlKeysManager)ConfigManager.GetInstance().GetControlKeysForPlayer(2).Clone();
            RefreshStrings();
            _zodiac = Resources.Load("GUI/zodiac") as GUISkin;
            _mySkin2 = Resources.Load("GUI/Options") as GUISkin;
        }

        /// <summary>
        /// Draw the KeyBoxes and sets the new values when the Button looses its focus
        /// </summary>
        public void OnGUI()
        {
            GUI.skin = _zodiac;
            float fontSize = 0.03f * Screen.height;
            GUI.skin.button.fontSize = (int)fontSize;
            float buttonWidth = Screen.width / 9;
            float buttonHeight = Screen.height / 10;

            if (GUI.Button(new Rect((Screen.width / 6) - (7 * buttonWidth / 5), Screen.height * (8.5f / 14), 7 * buttonWidth / 5, buttonHeight), "Main Menu"))
            {
                Application.LoadLevel("MainMenu");
            }

            if (GUI.Button(new Rect((Screen.width / 6) - (7 * buttonWidth / 5), Screen.height - (Screen.height * (3.5f / 14)), 7 * buttonWidth / 5, buttonHeight), "Back"))
            {
                Application.LoadLevel("GeneralOptions");
            }

            if (GUI.Button(new Rect(Screen.width - (9 * _settingsWidth / 4), Screen.height - (Screen.height * (3.5f / 14)), _settingsWidth * 2, _settingsHeight), "Restore Defaults"))
            {
                SetToDefault();
                RefreshStrings();
                ConfigManager.GetInstance().SetControlKeysForPlayer(1, _player1);
                ConfigManager.GetInstance().SetControlKeysForPlayer(2, _player2);
                ConfigManager.GetInstance().SaveConfig();
            }

            if (GUI.Button(new Rect(Screen.width - (9 * _settingsWidth / 4), Screen.height * (8.5f / 14), _settingsWidth * 2, _settingsHeight), "Apply"))
            {
                // TODO: Error checking!
                ConfigManager.GetInstance().SetControlKeysForPlayer(1, _player1);
                ConfigManager.GetInstance().SetControlKeysForPlayer(2, _player2);
                ConfigManager.GetInstance().SaveConfig();
            }

            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.fontSize = (int)(_settingsWidth * 0.2);
            labelStyle.normal.textColor = Color.green;
            _keyEvent = Event.current;
            DrawPlayer1Settings();
            DrawPlayer2Settings();
        }

        /// <summary>
        /// refreshes all strings to correspond to the actual settings
        /// </summary>
        private void RefreshStrings()
        {
            _leftKeyP1 = ConfigManager.KeyToString(_player1.LeftKey).ToString();
            _rightKeyP1 = ConfigManager.KeyToString(_player1.RightKey).ToString();
            _upKeyP1 = ConfigManager.KeyToString(_player1.ForwardKey).ToString();
            _downKeyP1 = ConfigManager.KeyToString(_player1.BackwardKey).ToString();
            _attackKeyP1 = ConfigManager.KeyToString(_player1.AttackKey).ToString();
            _defendKeyP1 = ConfigManager.KeyToString(_player1.DefendKey).ToString();
            _jumpKeyP1 = ConfigManager.KeyToString(_player1.JumpKey).ToString();
            _pickUpKeyP1 = ConfigManager.KeyToString(_player1.PickupKey).ToString();
            _dropKeyP1 = ConfigManager.KeyToString(_player1.DropItemKey).ToString();

            _leftKeyP2 = ConfigManager.KeyToString(_player2.LeftKey).ToString();
            _rightKeyP2 = ConfigManager.KeyToString(_player2.RightKey).ToString();
            _upKeyP2 = ConfigManager.KeyToString(_player2.ForwardKey).ToString();
            _downKeyP2 = ConfigManager.KeyToString(_player2.BackwardKey).ToString();
            _attackKeyP2 = ConfigManager.KeyToString(_player2.AttackKey).ToString();
            _defendKeyP2 = ConfigManager.KeyToString(_player2.DefendKey).ToString();
            _jumpKeyP2 = ConfigManager.KeyToString(_player2.JumpKey).ToString();
            _pickUpKeyP2 = ConfigManager.KeyToString(_player2.PickupKey).ToString();
            _dropKeyP2 = ConfigManager.KeyToString(_player2.DropItemKey).ToString();
        }

        /// <summary>
        /// sets all KeyCodes to the default value
        /// </summary>
        private void SetToDefault()
        {
            _player1.BackwardKey = _default1.BackwardKey;
            _player1.AttackKey = _default1.AttackKey;
            _player1.DefendKey = _default1.DefendKey;
            _player1.DropItemKey = _default1.DropItemKey;
            _player1.ForwardKey = _default1.ForwardKey;
            _player1.JumpKey = _default1.JumpKey;
            _player1.LeftKey = _default1.LeftKey;
            _player1.PickupKey = _default1.PickupKey;
            _player1.RightKey = _default1.RightKey;

            _player2.BackwardKey = _default2.BackwardKey;
            _player2.AttackKey = _default2.AttackKey;
            _player2.DefendKey = _default2.DefendKey;
            _player2.DropItemKey = _default2.DropItemKey;
            _player2.ForwardKey = _default2.ForwardKey;
            _player2.JumpKey = _default2.JumpKey;
            _player2.LeftKey = _default2.LeftKey;
            _player2.PickupKey = _default2.PickupKey;
            _player2.RightKey = _default2.RightKey;
        }

        /// <summary>
        /// Draws the Buttons for Player2
        /// </summary>
        private void DrawPlayer2Settings()
        {
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.fontSize = (int)(_settingsWidth * 0.2);
            labelStyle.normal.textColor = Color.green;
            float settingsLeftP2 = Screen.width / 2;
            float settingsTopFirstRowP2 = (Screen.height / 30) + _settingsHeight;
            float settingsTopSecondRowP2 = settingsTopFirstRowP2 + (_settingsHeight * 1.3f);
            float settingsTopThirdRowP2 = settingsTopSecondRowP2 + (_settingsHeight * 1.3f);
            GUI.skin = _mySkin2;
            GUI.Box(new Rect(settingsLeftP2, settingsTopFirstRowP2 - _settingsHeight, 4 * _settingsWidth, _settingsHeight), "Player 2");
            GUI.skin = _zodiac;
            if (_pickUpToggle2 = LabeledButton(new Rect(settingsLeftP2 + (2 * _settingsWidth), settingsTopFirstRowP2, _settingsWidth, _settingsHeight), "Pick up/ \r\n Use", _pickUpKeyP2, _pickUpToggle2))
            {
                if (_keyEvent.keyCode != KeyCode.None)
                {
                    _player2.PickupKey = _keyEvent.keyCode;
                    _pickUpKeyP2 = ConfigManager.KeyToString(_keyEvent.keyCode);
                    _pickUpToggle2 = false;
                }
            }

            if (_dropKeyToggle2 = LabeledButton(new Rect(settingsLeftP2 + (3 * _settingsWidth), settingsTopFirstRowP2, _settingsWidth, _settingsHeight), "Drop", _dropKeyP2, _dropKeyToggle2))
            {
                if (_keyEvent.keyCode != KeyCode.None)
                {
                    _player2.DropItemKey = _keyEvent.keyCode;
                    _dropKeyP2 = ConfigManager.KeyToString(_keyEvent.keyCode);
                    _dropKeyToggle2 = false;
                }
            }

            if (_upKeyToggle2 = LabeledButton(new Rect(settingsLeftP2 + (1 * _settingsWidth), settingsTopSecondRowP2, _settingsWidth, _settingsHeight), "Up", _upKeyP2, _upKeyToggle2))
            {
                if (_keyEvent.keyCode != KeyCode.None)
                {
                    _player2.ForwardKey = _keyEvent.keyCode;
                    _upKeyP2 = ConfigManager.KeyToString(_keyEvent.keyCode);
                    _upKeyToggle2 = false;
                }
            }

            if (_jumpKeyToggle2 = LabeledButton(new Rect(settingsLeftP2 + (3 * _settingsWidth), settingsTopSecondRowP2, _settingsWidth, _settingsHeight), "Jump", _jumpKeyP2, _jumpKeyToggle2))
            {
                if (_keyEvent.keyCode != KeyCode.None)
                {
                    _player2.JumpKey = _keyEvent.keyCode;
                    _jumpKeyP2 = ConfigManager.KeyToString(_keyEvent.keyCode);
                    _jumpKeyToggle2 = false;
                }
            }

            if (_attackKeyToggle2 = LabeledButton(new Rect(settingsLeftP2 + (0 * _settingsWidth), settingsTopFirstRowP2, _settingsWidth, _settingsHeight), "Attack", _attackKeyP2, _attackKeyToggle2))
            {
                if (_keyEvent.keyCode != KeyCode.None)
                {
                    _player2.AttackKey = _keyEvent.keyCode;
                    _attackKeyP2 = ConfigManager.KeyToString(_keyEvent.keyCode);
                    _attackKeyToggle2 = false;
                }
            }

            if (_defendKeyToggle2 = LabeledButton(new Rect(settingsLeftP2 + (1 * _settingsWidth), settingsTopFirstRowP2, _settingsWidth, _settingsHeight), "Defend", _defendKeyP2, _defendKeyToggle2))
            {
                if (_keyEvent.keyCode != KeyCode.None)
                {
                    _player2.DefendKey = _keyEvent.keyCode;
                    _defendKeyP2 = ConfigManager.KeyToString(_keyEvent.keyCode);
                    _defendKeyToggle2 = false;
                }
            }

            if (_leftKeyToggle2 = LabeledButton(new Rect(settingsLeftP2 + (0 * _settingsWidth), settingsTopThirdRowP2, _settingsWidth, _settingsHeight), "Left", _leftKeyP2, _leftKeyToggle2))
            {
                if (_keyEvent.keyCode != KeyCode.None)
                {
                    _player2.LeftKey = _keyEvent.keyCode;
                    _leftKeyP2 = ConfigManager.KeyToString(_keyEvent.keyCode);
                    _leftKeyToggle2 = false;
                }
            }

            if (_downKeyToggle2 = LabeledButton(new Rect(settingsLeftP2 + (1 * _settingsWidth), settingsTopThirdRowP2, _settingsWidth, _settingsHeight), "Down", _downKeyP2, _downKeyToggle2))
            {
                if (_keyEvent.keyCode != KeyCode.None)
                {
                    _player2.BackwardKey = _keyEvent.keyCode;
                    _downKeyP2 = ConfigManager.KeyToString(_keyEvent.keyCode);
                    _downKeyToggle2 = false;
                }
            }

            if (_rightKeyToggle2 = LabeledButton(new Rect(settingsLeftP2 + (2 * _settingsWidth), settingsTopThirdRowP2, _settingsWidth, _settingsHeight), "Right", _rightKeyP2, _rightKeyToggle2))
            {
                if (_keyEvent.keyCode != KeyCode.None)
                {
                    _player2.RightKey = _keyEvent.keyCode;
                    _rightKeyP2 = ConfigManager.KeyToString(_keyEvent.keyCode);
                    _rightKeyToggle2 = false;
                }
            }
        }

        /// <summary>
        /// Draw the Buttons for Player1
        /// </summary>
        private void DrawPlayer1Settings()
        {
            float settingsLeftP1 = Screen.width / 4;
            float settingsTopFirstRowP1 = Screen.height * (5f / 8);
            float settingsTopSecondRowP1 = settingsTopFirstRowP1 + (_settingsHeight * 1.3f);
            float settingsTopThirdRowP1 = settingsTopSecondRowP1 + (_settingsHeight * 1.3f);
            _keyEvent = Event.current;
            GUI.skin = _mySkin2;
            GUI.Box(new Rect(settingsLeftP1, settingsTopFirstRowP1 - _settingsHeight, _settingsWidth * 4, _settingsHeight), "Player 1");
            GUI.skin = _zodiac;
            if (_pickUpToggle1 = LabeledButton(new Rect(settingsLeftP1 + (2 * _settingsWidth), settingsTopFirstRowP1, _settingsWidth, _settingsHeight), "Pick up/ \r\n Use", _pickUpKeyP1, _pickUpToggle1))
            {
                if (_keyEvent.keyCode != KeyCode.None)
                {
                    _player1.PickupKey = _keyEvent.keyCode;
                    _pickUpKeyP1 = ConfigManager.KeyToString(_keyEvent.keyCode);
                    _pickUpToggle1 = false;
                }
            }

            if (_dropKeyToggle1 = LabeledButton(new Rect(settingsLeftP1 + (3 * _settingsWidth), settingsTopFirstRowP1, _settingsWidth, _settingsHeight), "Drop", _dropKeyP1, _dropKeyToggle1))
            {
                if (_keyEvent.keyCode != KeyCode.None)
                {
                    _player1.DropItemKey = _keyEvent.keyCode;
                    _dropKeyP1 = ConfigManager.KeyToString(_keyEvent.keyCode);
                    _dropKeyToggle1 = false;
                }
            }

            if (_upKeyToggle1 = LabeledButton(new Rect(settingsLeftP1 + (1 * _settingsWidth), settingsTopSecondRowP1, _settingsWidth, _settingsHeight), "Up", _upKeyP1, _upKeyToggle1))
            {
                if (_keyEvent.keyCode != KeyCode.None)
                {
                    _player1.ForwardKey = _keyEvent.keyCode;
                    _upKeyP1 = ConfigManager.KeyToString(_keyEvent.keyCode);
                    _upKeyToggle1 = false;
                }
            }

            if (_jumpKeyToggle1 = LabeledButton(new Rect(settingsLeftP1 + (3 * _settingsWidth), settingsTopSecondRowP1, _settingsWidth, _settingsHeight), "Jump", _jumpKeyP1, _jumpKeyToggle1))
            {
                if (_keyEvent.keyCode != KeyCode.None)
                {
                    _player1.JumpKey = _keyEvent.keyCode;
                    _jumpKeyP1 = ConfigManager.KeyToString(_keyEvent.keyCode);
                    _jumpKeyToggle1 = false;
                }
            }

            if (_attackKeyToggle1 = LabeledButton(new Rect(settingsLeftP1 + (0 * _settingsWidth), settingsTopFirstRowP1, _settingsWidth, _settingsHeight), "Attack", _attackKeyP1, _attackKeyToggle1))
            {
                if (_keyEvent.keyCode != KeyCode.None)
                {
                    _player1.AttackKey = _keyEvent.keyCode;
                    _attackKeyP1 = ConfigManager.KeyToString(_keyEvent.keyCode);
                    _attackKeyToggle1 = false;
                }
            }

            if (_defendKeyToggle1 = LabeledButton(new Rect(settingsLeftP1 + (1 * _settingsWidth), settingsTopFirstRowP1, _settingsWidth, _settingsHeight), "Defend", _defendKeyP1, _defendKeyToggle1))
            {
                if (_keyEvent.keyCode != KeyCode.None)
                {
                    _player1.DefendKey = _keyEvent.keyCode;
                    _defendKeyP1 = ConfigManager.KeyToString(_keyEvent.keyCode);
                    _defendKeyToggle1 = false;
                }
            }

            if (_leftKeyToggle1 = LabeledButton(new Rect(settingsLeftP1 + (0 * _settingsWidth), settingsTopThirdRowP1, _settingsWidth, _settingsHeight), "Left", _leftKeyP1, _leftKeyToggle1))
            {
                if (_keyEvent.keyCode != KeyCode.None)
                {
                    _player1.LeftKey = _keyEvent.keyCode;
                    _leftKeyP1 = ConfigManager.KeyToString(_keyEvent.keyCode);
                    _leftKeyToggle1 = false;
                }
            }

            if (_downKeyToggle1 = LabeledButton(new Rect(settingsLeftP1 + (1 * _settingsWidth), settingsTopThirdRowP1, _settingsWidth, _settingsHeight), "Down", _downKeyP1, _downKeyToggle1))
            {
                if (_keyEvent.keyCode != KeyCode.None)
                {
                    _player1.BackwardKey = _keyEvent.keyCode;
                    _downKeyP1 = ConfigManager.KeyToString(_keyEvent.keyCode);
                    _downKeyToggle1 = false;
                }
            }

            if (_rightKeyToggle1 = LabeledButton(new Rect(settingsLeftP1 + (2 * _settingsWidth), settingsTopThirdRowP1, _settingsWidth, _settingsHeight), "Right", _rightKeyP1, _rightKeyToggle1))
            {
                if (_keyEvent.keyCode != KeyCode.None)
                {
                    _player1.RightKey = _keyEvent.keyCode;
                    _rightKeyP1 = ConfigManager.KeyToString(_keyEvent.keyCode);
                    _rightKeyToggle1 = false;
                }
            }
        }

        /// <summary>
        /// Draws a labeled Button in the given Rectangular
        /// </summary>
        /// <param name="position">the position an size of the label</param>
        /// <param name="label">the text of the label</param>
        /// <param name="ButtonText">the text of the button</param>
        /// <param name="value">the currentBoolvalue</param>
        /// <returns>if the Button was pressed</returns>
        private bool LabeledButton(Rect position, string label, string ButtonText, bool value)
        {
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.fontSize = (int)(0.1 * position.width);
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.alignment = TextAnchor.MiddleCenter;
            buttonStyle.fontStyle = FontStyle.Normal;
            buttonStyle.fontSize = (int)(0.1 * position.width);
            float labelDivision = 0.5f;
            GUI.Label(new Rect(position.x, position.y, position.width * labelDivision, position.height), label, labelStyle);
            value = GUI.Toggle(new Rect(position.x + (position.width * labelDivision), position.y, position.width * (1 - labelDivision), position.height), value, ButtonText, buttonStyle);
            return value;
        }
    }
}
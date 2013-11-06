namespace Assets.Scripts.UI
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// This class shows the Host / Join menu to host or connect to a server.
    /// </summary>
    public class SpecialModeHostJoin : MonoBehaviour
    {
        /// <summary>
        /// Used Gui skin
        /// </summary>
        private GUISkin _zodiacStyle;

        /// <summary>
        /// The NetworkController that holds the host and connection logic.
        /// </summary>
        private NetworkController _net;

        /// <summary>
        /// The users name.
        /// </summary>
        private string _input;

        private string _ignorInput;
        private string _ingorInput2;

        /// <summary>
        /// Executed on start, sets the GUIStyle and the NetworkController
        /// </summary>
        public void Start()
        {
            _net = GameManager.GetInstance().NetworkController;
            _zodiacStyle = Resources.Load("GUI/zodiac") as GUISkin;

            // Naming
            _ignorInput = "DEFAULT NAME";
            _ingorInput2 = string.Empty;
            _input = _net.Username.Equals(_ingorInput2) ? _ignorInput : _net.Username;
        }

        /// <summary>
        /// Draws the menu buttons.
        /// The buttons calling connection scene or initialise a server and loading the host scene.
        /// </summary>
        public void OnGUI()
        {
            GUI.skin = _zodiacStyle;
            float buttonWidth = Screen.width / 5;
            float buttonHeight = Screen.width / 10;
            float buttonLeft = Screen.width / 10;
            float buttonHorizontalOffset = Screen.width / 30;
            float buttonVerticalOffset = Screen.height / 10;
            float buttonTop = Screen.height - buttonHeight - buttonVerticalOffset;
            float textFieldHorizontal = Screen.width * 0.4f;
            float textFieldVertical = Screen.height * 0.2f;

           /* if (GUI.Button(new Rect(0, 0, Screen.width / 3, Screen.height / 3), "shortcut to GameSelection"))
            {
                Application.LoadLevel("GameSelection");
            }
            */
            _input = GUI.TextField(new Rect(textFieldHorizontal, textFieldVertical, buttonTop * 0.7f, buttonHeight / 2f), _input, 15);

            if (GUI.Button(new Rect(buttonLeft, buttonTop, buttonWidth, buttonHeight), "Host"))
            {
                if (_input.Equals(_ingorInput2))
                {
                    _input = _ignorInput;
                }

                if (!_input.Equals(_ignorInput))
                {
                    _net.Username = _input;
                }

                _net.CreateServer();
                Application.LoadLevel("SMHostConfig");
            }

            if (GUI.Button(new Rect(buttonLeft + buttonWidth + buttonHorizontalOffset, buttonTop, buttonWidth, buttonHeight), "Join"))
            {
                if (_input.Equals(_ingorInput2))
                {
                    _input = _ignorInput;
                }

                if (!_input.Equals(_ignorInput))
                {
                    _net.Username = _input;
                }

                Application.LoadLevel("SMJoinConfig");
            }

            if (GUI.Button(new Rect(Screen.width - (Screen.width / 7), Screen.height * (8.5f / 14), (5 * buttonWidth) / 8, (3 * buttonHeight) / 4), "Main Menu"))
            {
                Application.LoadLevel("MainMenu");
            }
        }
    }
}

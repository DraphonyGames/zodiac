namespace Assets.Scripts.UI
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Connectionscreen for clients in specialmode.
    /// </summary>
    public class SpecialModeJoinConfig : MonoBehaviour
    {
        /// <summary>
        /// the IP inserted by the player as String set when Return is pressed an IP is valid
        /// </summary>
        public string InsertedIp = "127.0.0.1";

        /// <summary>
        /// ipValue for TextField
        /// </summary>
        private string _ip = "127.0.0.1";

        /// <summary>
        /// Used Gui skin
        /// </summary>
        private GUISkin _zodiacStyle;

        /// <summary>
        /// The NetworkController that holds the host and connection logic.
        /// </summary>
        private NetworkController _net;

        /// <summary>
        /// Gets or sets an errorMessage to Display
        /// </summary>
        public string ErrorMessage
        {
            get;
            set;
        }

        /// <summary>
        /// Executed on start, sets the GUIStyle and the NetworkController.
        /// </summary>
        public void Start()
        {
            _net = GameManager.GetInstance().NetworkController;
            _zodiacStyle = Resources.Load("GUI/zodiac") as GUISkin;
        }

        /// <summary>
        /// Draws the Buttons and textfields needed to connect to a usergiven ip.
        /// </summary>
        public void OnGUI()
        {
            ErrorMessage = string.Empty;
            GUI.skin = _zodiacStyle;
            float labelWidth = Screen.width / 4;
            float labelHeight = Screen.width / 20;
            float buttonWidth = Screen.width / 5;
            float buttonHeight = Screen.width / 10;
            float buttonTop = Screen.height * (2f / 3);
            float buttonLeft = Screen.width * (1f / 3);
            float iPLabelTop = Screen.height / 10;
            float iPLabelLeft = Screen.width / 10;
            float NoLabelTop = iPLabelTop;
            float NoLabelLeft = Screen.width * (2f / 3);
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = (int)(labelHeight * 0.4f);
            _ip = GUI.TextField(new Rect(iPLabelLeft, iPLabelTop, labelWidth, labelHeight), _ip);
            if (_net.Status == NetworkPeerType.Connecting)
            {
                labelStyle.normal.textColor = Color.yellow;
                ErrorMessage = "Connecting...";
            }
            else if (!ErrorMessage.Equals(string.Empty))
            {
                labelStyle.normal.textColor = Color.red;
                ErrorMessage = "Failed to connect";
            }

            if (Event.current.keyCode == KeyCode.Return)
            {
                InsertedIp = _ip;
                _net.Connect(InsertedIp);
            }

            GUI.Label(new Rect(NoLabelLeft, NoLabelTop, labelWidth, labelHeight), ErrorMessage, labelStyle);
            if (GUI.Button(new Rect(Screen.width - (Screen.width / 7), Screen.height * (8.5f / 14), (5 * buttonWidth) / 8, (3 * buttonHeight) / 4), "Back"))
            {
                Application.LoadLevel("SMHostJoin");
            }

            if (GUI.Button(new Rect(buttonLeft, buttonTop, buttonWidth, buttonHeight), "Connect"))
            {
                InsertedIp = _ip;
                _net.Connect(InsertedIp);
            }
        }
    }
}

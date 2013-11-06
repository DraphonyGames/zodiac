namespace Assets.Scripts.UI
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Scenescript that shows the initialised servers ip and the menu for the host.
    /// </summary>
    public class SpecialModeHostConfig : MonoBehaviour
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
        /// Connection setting: how much clients are allowed?
        /// </summary>
        private float _connections;

        private GameManager _manager = GameManager.GetInstance();

        /// <summary>
        /// Executed on start, sets the GUIStyle and the NetworkController
        /// </summary>
        public void Start()
        {
            _net = _manager.NetworkController;
            _zodiacStyle = Resources.Load("GUI/zodiac") as GUISkin;
            _connections = _net.MaxPlayer;
        }

        /// <summary>
        /// Draws the Buttons and shows how many clients are connected to this players initialised server.
        /// </summary>
        public void OnGUI()
        {
            GUI.skin = _zodiacStyle;
            float labelWidth = Screen.width / 3;
            float labelHeight = Screen.width / 10;
            float buttonWidth = Screen.width / 5;
            float buttonHeight = Screen.width / 10;
            float buttonTop = Screen.height * (2f / 3);
            float buttonLeft = Screen.width * (1f / 3);
            float IPLabelTop = Screen.height / 10;
            float IPLabelLeft = Screen.width / 10;
            float NoLabelTop = IPLabelTop;
            float NoLabelLeft = Screen.width * (2f / 3);
            float sliderWidth = Screen.width / 3;
            float sliderHeight = Screen.height / 10;
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = (int)(labelHeight * 0.2f);

            string[] ip = _net.Ip.Split(':');
            string ip2 = ip[1].StartsWith("UN") ? "" : "\r\n local: \r\n" + ip[1];
            GUI.Label(new Rect(IPLabelLeft, IPLabelTop, labelWidth, labelHeight), "Your IP is: \r\n " + ip[0] + ip2, labelStyle);
            GUI.Label(new Rect(NoLabelLeft, NoLabelTop, labelWidth, labelHeight), "There are " + _net.NumOfConnections + " Players Connected", labelStyle);

            _connections = Mathf.Round(GUIOperations.LabeledSlider(new Rect(NoLabelLeft * 0.8f, IPLabelTop + sliderHeight, sliderWidth, sliderHeight), "Maximum Players: " + _connections.ToString(), _connections, 2, NetworkController.REALMAXIMUMOFPLAYER));
            _net.MaxPlayer = (int)_connections; // dat's rly ugly ...is called every OnGUI() :/

            if (GUI.Button(new Rect(Screen.width - (Screen.width / 7), Screen.height * (8.5f / 14), (5 * buttonWidth) / 8, (3 * buttonHeight) / 4), "Back"))
            {
                _net.Disconnect();
                Application.LoadLevel("SMHostJoin");
            }

            if (_net.Ready != _net.MaxPlayer)
            { // SUBJECT TO CHANGE
                GUI.enabled = false;
            }

            if (GUI.Button(new Rect(buttonLeft, buttonTop, buttonWidth, buttonHeight), "Start"))
            {
                GameManager.GetInstance().ResetLevel();
                _manager.Difficulty = 0;
                _manager.HeroCount = 1;
                _manager.PlayerCount = 1;
                _manager.MercenaryCount = 0;
                _manager.SectionNo = 1;
                _manager.TeamCount = 2;
                Application.LoadLevel("CharacterSelection");
            }

            GUI.enabled = true;
        }
    }
}

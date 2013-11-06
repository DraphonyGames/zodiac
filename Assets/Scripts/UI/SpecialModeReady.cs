namespace Assets.Scripts.UI
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Scenescript that shows the clients menu after choosing a hero in specialmode.
    /// </summary>
    public class SpecialModeReady : MonoBehaviour
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
        /// Sets the ready button active or inactive
        /// </summary>
        private bool _ready;

        /// <summary>
        /// Executed on start, sets the GUIStyle and the NetworkController.
        /// Also set the ready button on enabled until clicked.
        /// </summary>
        public void Start()
        {
            // SET BOOL FOR SMHostConfigButton HERE!!!!
            _net = GameManager.GetInstance().NetworkController;
            _zodiacStyle = Resources.Load("GUI/zodiac") as GUISkin;
            _ready = false;
        }

        /// <summary>
        /// Draws the Buttons for the client after choosing a hero.
        /// </summary>
        public void OnGUI()
        {
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
            labelStyle.fontSize = (int)(labelHeight * 0.8f);
            GUI.Label(new Rect(iPLabelLeft, iPLabelTop, labelWidth, labelHeight), "Connected", labelStyle);
            if (GUI.Button(new Rect(NoLabelLeft, NoLabelTop, buttonWidth, buttonHeight), "Disconnect"))
            {
                // dissconectionLogic here
                _net.Disconnect();
                Application.LoadLevel("SMJoinConfig");
            }

            if (GUI.Button(new Rect(Screen.width - (Screen.width / 7), Screen.height * (8.5f / 14), (5 * buttonWidth) / 8, (3 * buttonHeight) / 4), "Back"))
            {
                // don't know if this buttun will be ingame at the end
                _net.Disconnect();
                Application.LoadLevel("SMJoinConfig");
            }

            if (_ready)
            {
                GUI.enabled = false;
            }

            if (GUI.Button(new Rect(buttonLeft, buttonTop, buttonWidth, buttonHeight), "Ready"))
            {
                _ready = true;
                _net.IncReady();
                Resources.Load("LoadingScreen");
            }
        }
    }
}

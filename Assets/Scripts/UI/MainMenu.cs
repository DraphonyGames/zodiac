namespace Assets.Scripts.UI
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Draws the MainMenu
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        /// <summary>
        /// Variable for instance of gamemanager
        /// </summary>
        private GameManager manager;

        /// <summary>
        /// Gets or sets Variable for custom GUI skin
        /// </summary>
        public GUISkin MySkin
        {
            get;
            set;
        }

        /// <summary>
        /// Instantiates the Network GameObject.
        /// </summary>
        public void Start()
        {
            manager = GameManager.GetInstance();
            manager.ResetTeams();
            manager.ResetMercenaries();
            manager.ResetLevel();
            MySkin = Resources.Load("GUI/zodiac") as GUISkin;
            GameObject net = GameObject.Find("Network");
            if (net == null)
            {
                // ensure that no second Network GameObject will be instantiated
                net = (GameObject)Instantiate(Resources.Load("Network"));
                net.name = "Network";
            }
            else
            {
                if (net.GetComponent<NetworkController>().IsServer || net.GetComponent<NetworkController>().IsClient)
                {
                    Debug.Log("Disconnect from MainMenu");
                    net.GetComponent<NetworkController>().Disconnect();
                }
            }
        }

        /// <summary>
        /// Draws the Buttons for the main menu and then changes the scene
        /// </summary>
        public void OnGUI()
        {
            GUI.skin = MySkin;

            float buttonWidth = Screen.width / 9;
            float buttonHeight = Screen.height / 10;
            float offset = Screen.width / 20;

            // enables scalebal fonts (depending on screen width) the 0.04 was detemined by try and error
            float fontSize = 0.06f * Screen.height;
            GUI.skin.button.fontSize = (int)fontSize;

            // create the buttons and then start the specific Scene
            if (GUI.Button(new Rect((Screen.width / 2) - (7 * buttonWidth / 2), (Screen.height / 2) - (2 * buttonHeight), 3 * buttonWidth, 3 * buttonHeight), "Story"))
            {
                GameManager.GetInstance().GameMode = GameManager.Mode.PLAY;
                Application.LoadLevel("GameSelection");
            }

            if (GUI.Button(new Rect((Screen.width / 2) + (1 * buttonWidth / 2), (Screen.height / 2) - (2 * buttonHeight), 3 * buttonWidth, 3 * buttonHeight), "Conquest"))
            {
                GameManager.GetInstance().GameMode = GameManager.Mode.SPECIAL;
                Application.LoadLevel("SMHostJoin");
            }

            // enables scalebal fonts (depending on screen width) the 0.04 was detemined by try and error
            fontSize = 0.03f * Screen.height;
            GUI.skin.button.fontSize = (int)fontSize;

            if (GUI.Button(new Rect(Screen.width - (Screen.width / 6), Screen.height - (Screen.height * (3.5f / 14)), 7 * buttonWidth / 5, buttonHeight), "Settings"))
            {
                GameManager.GetInstance().GameMode = GameManager.Mode.SETTINGS;
                Application.LoadLevel("GeneralOptions");
            }

            if (GUI.Button(new Rect((Screen.width / 6) - (7 * buttonWidth / 5), Screen.height - (Screen.height * (3.5f / 14)), 7 * buttonWidth / 5, buttonHeight), "Quit"))
            {
                Application.Quit();
            }
        }
    }
}

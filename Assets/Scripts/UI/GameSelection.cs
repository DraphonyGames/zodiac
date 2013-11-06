namespace Assets.Scripts.UI
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Settings screen for multiplayer.. also shows the connected status as client on specialmode.
    /// </summary>
    public class GameSelection : MonoBehaviour
    {
        /// <summary>
        /// Variable for custom GUI skin
        /// </summary>
        public GUISkin Myskin;

        private NetworkController _net;
        private float _difficulty = 2;
        private float _section = 1;
        private float _humans = 1;
        private float _mercs = 6;
        private float _heroes = 0;

        private float _maxDifficulty = 5;
        private float _maxSection = 3;
        private float _maxHeroes = 8;
        private float _maxHumans = 8;
        private float _maxMercs = 30;
        private string _difficultyString;
        private string _levelStringMultiplayer;

        private GameManager manager = GameManager.GetInstance();

        private GUISkin _myskin2;

        /// <summary>
        /// The backgroundobjects of this scene.
        /// </summary>
        private GameObject _firstBackground, _secondBackground;

        /// <summary>
        /// Initializes and sets defaults and styls.
        /// </summary>
        public void Start()
        {
            _firstBackground = GameObject.Find("game_selection");
            _secondBackground = GameObject.Find("ConnectedBackground");
            _firstBackground.GetComponent<GUITexture>().gameObject.SetActive(true);
            _secondBackground.GetComponent<GUITexture>().gameObject.SetActive(false);
            _net = manager.NetworkController;
            _heroes = (int)manager.HeroCount;
            _humans = (int)manager.PlayerCount;
            _difficulty = (int)manager.Difficulty;
            _section = (int)manager.SectionNo;

            if (manager.GameMode == GameManager.Mode.MULTI)
            {
                _humans = 2;
            }

            _myskin2 = Resources.Load("GUI/Options") as GUISkin;
        }

        /// <summary>
        /// Draw the character selection screen and sets gameoptions.
        /// calculates the box sizes
        /// </summary>
        public void OnGUI()
        {
            GUI.skin = _myskin2;
            if (manager.GameMode == GameManager.Mode.PLAY)
            {
                // all values are calculated relativ to screen in order to fit the Background
                float boxHeight = Screen.height / 10;
                float boxWidth = Screen.width / 6;
                float sliderWidth = Screen.width / 3;
                float buttonWidth = Screen.width / 9;
                float buttonHeight = Screen.height / 10;
                float offsetDifficultyWidth = 3 * Screen.width / 5;
                float offsetDifficultyHeight = Screen.width / 20;
                float offsetSectionWidth = Screen.width / 20;
                float offsetSectionHeight = Screen.width / 20;
                float offsetHeroWidth = Screen.width / 9;
                float offsetHeroHeight = 3 * Screen.height / 5;
                float offsetAIWidth = Screen.width / 3;
                float offsetAIHeight = 3 * Screen.height / 4;

                _maxHumans = 2;
                _maxHeroes = 8;

                // The Difficulty and Level sliders are drawn here
                switch ((int)_difficulty)
                {
                    case 0:
                        _difficultyString = "N00b";
                        break;
                    case 1:
                        _difficultyString = "Easy going";
                        break;
                    case 2:
                        _difficultyString = "Come get some";
                        break;
                    case 3:
                        _difficultyString = "Hurt me plently";
                        break;
                    case 4:
                        _difficultyString = "Kick ass!";
                        break;
                    case 5:
                        _difficultyString = "Insane";
                        break;
                    default:
                        break;
                }

                // Difficulty box and slider
                GUI.Box(new Rect(offsetDifficultyWidth, offsetDifficultyHeight, 2 * boxWidth, boxHeight), "Difficulty");
                _difficulty = Mathf.Round(GUIOperations.LabeledSlider(new Rect(offsetDifficultyWidth, offsetDifficultyHeight + (boxHeight / 3), sliderWidth, boxHeight), string.Empty, _difficulty, 0, _maxDifficulty));
                GUI.Button(new Rect(offsetDifficultyWidth, offsetDifficultyHeight + (2 * boxHeight / 3), 2 * boxWidth, boxHeight), _difficultyString);

                // Difficulty box and slider
                GUI.Box(new Rect(offsetSectionWidth, offsetSectionHeight, 2 * boxWidth, boxHeight), "Section");
                _section = Mathf.Round(GUIOperations.LabeledSlider(new Rect(offsetSectionWidth, offsetSectionHeight + (boxHeight / 3), sliderWidth, boxHeight), string.Empty, _section, 1, _maxSection));
                GUI.Button(new Rect(offsetSectionWidth, offsetSectionHeight + (2 * boxHeight / 3), 2 * boxWidth, boxHeight), _section.ToString());

                // Hero box and slider 
                GUI.Box(new Rect(offsetHeroWidth, offsetHeroHeight, 2 * boxWidth, boxHeight), "Humans");
                _humans = Mathf.Round(GUIOperations.LabeledSlider(new Rect(offsetHeroWidth + (sliderWidth / 4), offsetHeroHeight + (boxHeight / 3), sliderWidth / 2, boxHeight), string.Empty, _humans, 1, _maxHumans));
                GUI.Button(new Rect(offsetHeroWidth, offsetHeroHeight + (2 * boxHeight / 3), 2 * boxWidth, boxHeight), _humans.ToString());

                if ((int)_heroes > ((int)_maxHeroes - (int)_humans))
                {
                    _heroes = (float)((int)_maxHeroes - (int)_humans);
                }

                // AI-Hero box and slider
                GUI.Box(new Rect(offsetAIWidth, offsetAIHeight, 2 * boxWidth, boxHeight), "AI-Heroes");
                _heroes = Mathf.Round(GUIOperations.LabeledSlider(new Rect(offsetAIWidth, offsetAIHeight + (boxHeight / 3), sliderWidth, boxHeight), string.Empty, _heroes, 0, (float)((int)_maxHeroes - (int)_humans)));
                GUI.Button(new Rect(offsetAIWidth, offsetAIHeight + (2 * boxHeight / 3), 2 * boxWidth, boxHeight), _heroes.ToString());

                // Skin for Menu-Buttons
                GUI.skin = Myskin;
                float fontSize = 0.03f * Screen.height;
                GUI.skin.button.fontSize = (int)fontSize;

                // Both menu buttons
                if (GUI.Button(new Rect((Screen.width / 6) - (7 * buttonWidth / 5), Screen.height - (Screen.height * (3.5f / 14)), 7 * buttonWidth / 5, buttonHeight), "Back"))
                {
                    Application.LoadLevel("MainMenu");
                }

                if (GUI.Button(new Rect(Screen.width - (Screen.width / 6), Screen.height - (Screen.height * (3.5f / 14)), 7 * buttonWidth / 5, buttonHeight), "Continue"))
                {
                    GameManager.GetInstance().ResetLevel();
                    manager.HeroCount = (int)_heroes;
                    manager.PlayerCount = (int)_humans;
                    manager.Difficulty = (int)_difficulty;
                    manager.SectionNo = (int)_section;
                    manager.TeamCount = 1;
                    Application.LoadLevel("CharacterSelection");
                }
            }
            else if (manager.GameMode == GameManager.Mode.SPECIAL)
            {
                // all values are calculated relativ to screen in order to fit the Background
                float offset = Screen.width / 30;
                float boxHeight = Screen.height / 10;
                float boxWidth = Screen.width / 5;
                float buttonWidth = Screen.width / 9;
                float buttonHeight = Screen.height / 10;
                if (!_net.IsServer && !_net.IsClient)
                { // only needed for the shortcut
                
                    _maxHumans = 8;
                    _maxHeroes = 8;

                    // offset for the hero/merc/team settings 
                    float offHumanTop = (Screen.height / 2) + (3f * offset);

                    // sliders for the hero/merc/team settings;
                    _humans = Mathf.Round(GUIOperations.LabeledSlider(new Rect((1 * offset) + (boxWidth / 2), offHumanTop + (boxHeight / 2), boxWidth + 50, boxHeight), "Number of Humans: " + (int)_humans, _humans, 2, _maxHumans));

                    // _mercs = Mathf.Round(GUIOperations.LabeledSlider(new Rect(3 * offset + boxWidth / 2, offHumanTop - boxHeight, boxWidth + 50, boxHeight), "Number of Mercs: " + (int)_mercs, _mercs, 0, _maxMercs));
                    if ((int)_heroes > ((int)_maxHeroes - (int)_humans))
                    {
                        _heroes = (float)((int)_maxHeroes - (int)_humans);
                    }

                    _heroes = GUIOperations.LabeledSlider(new Rect(offset + (3 * boxWidth / 2), offHumanTop + (2 * boxHeight), boxWidth + 50, boxHeight), "Number of AI-Heroes: " + (int)_heroes, _heroes, 0, (float)((int)_maxHeroes - (int)_humans));
                    GUI.skin = Myskin;
                    float fontSize = 0.03f * Screen.height;
                    GUI.skin.button.fontSize = (int)fontSize;
                    if (GUI.Button(new Rect(Screen.width - (Screen.width / 7), Screen.height * (8.5f / 14), 6 * buttonWidth / 5, buttonHeight), "Main Menu"))
                    {
                        Application.LoadLevel("MainMenu");
                    }

                    if (GUI.Button(new Rect(Screen.width - (Screen.width / 7), Screen.height - (Screen.height * (3.5f / 14)), 6 * buttonWidth / 5, buttonHeight), "Continue"))
                    {
                        manager.Difficulty = 0;
                        manager.HeroCount = (int)_heroes;
                        manager.PlayerCount = (int)_humans;
                        manager.MercenaryCount = (int)_mercs;
                        manager.SectionNo = 1;
                        manager.TeamCount = 2;
                        Application.LoadLevel("CharacterSelection");
                    }
                }

                if (_net.IsClient)
                {
                    _firstBackground.GetComponent<GUITexture>().gameObject.SetActive(false);
                    _secondBackground.GetComponent<GUITexture>().gameObject.SetActive(true);
                    manager.Difficulty = 0;
                    manager.HeroCount = (int)_heroes;
                    manager.PlayerCount = (int)_humans;
                    manager.MercenaryCount = (int)_mercs;
                    manager.SectionNo = 1;
                    manager.TeamCount = 2;
                    GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
                    labelStyle.normal.textColor = Color.green;
                    float labelWidth = Screen.width / 3;
                    float labelHeight = Screen.height / 7;
                    labelStyle.fontSize = (int)(0.15 * labelWidth);
                    GUI.Label(new Rect(Screen.width / 3, Screen.height - (Screen.height / 4) - (Screen.height / 8), labelWidth, labelHeight), "Connected", labelStyle);
                    GUIStyle buttonStyle = new GUIStyle(Myskin.button);
                    buttonStyle.fontSize = (int)(0.3f * buttonHeight);
                    if (GUI.Button(new Rect(Screen.width - (Screen.width / 7), Screen.height - (Screen.height * (3.5f / 14)), 6 * buttonWidth / 5, buttonHeight), "Continue", buttonStyle))
                    {
                        Application.LoadLevel("CharacterSelection");
                    }
                }
            }
        }
    }
}
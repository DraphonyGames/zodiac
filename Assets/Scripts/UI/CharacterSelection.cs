namespace Assets.Scripts.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Class for Characterselection, where all human and ai-heroes are choosen and assigned to their teams.
    /// </summary>
    public class CharacterSelection : MonoBehaviour
    {
        /// <summary>
        /// Skins for all gui elements except arrows
        /// </summary>
        public GUISkin Myskin;

        /// <summary>
        /// Skin for left arrow in character selection
        /// </summary>
        public GUISkin MyskinLeft;

        /// <summary>
        /// Skin for right arrow in character selection
        /// </summary>
        public GUISkin MyskinRight;

        /// <summary>
        /// Skin for infobox in character selection
        /// </summary>
        public GUISkin MyskinInfo;

        /// <summary>
        /// Background scaling factor to fit the Models to the Platform
        /// </summary>
        private static Vector3 _backgroundScale = new Vector3(26, 26, 26);

        /// <summary>
        /// Foreground scaling factor
        /// </summary>
        private static Vector3 _foregroundScale = 1.2f * _backgroundScale;

        /// <summary>
        /// Networkcontroller for playing over TCP-IP
        /// </summary>
        private NetworkController _net;

        /// <summary>
        /// List that holds all Characters
        /// </summary>
        private List<GameObject> _characterList;

        /// <summary>
        /// list that holds all positions (the character on index 0 of _characterList is on Positio _positions[0])
        /// </summary>
        private List<Vector3> _positions;

        /// <summary>
        /// Previous selection of hero/mob toolbar
        /// </summary>
        private int _toolbarSelectionOld;

        /// <summary>
        /// Actual selection of hero/mob toolbar
        /// </summary>
        private int _toolbarSelectionNew;

        /// <summary>
        /// String that display infos about the current character
        /// </summary>
        private string _displayString;
        
        /// <summary>
        /// String that represents the actual nick of player
        /// </summary>
        private string _charName;

        /// <summary>
        /// Playernumber, who has choice
        /// </summary>
        private int _currentChoice;

        /// <summary>
        /// Playernumber, who has choice
        /// </summary>
        private bool _animationIsPlaying;

        /// <summary>
        /// Number of players
        /// </summary>
        private int _noPlayer;

        /// <summary>
        /// Team 1 
        /// </summary>
        private Team _team1;

        /// <summary>
        /// Team 2
        /// </summary>
        private Team _team2;

        /// <summary>
        /// total number of characters
        /// </summary>
        private int _totalCharacters;

        /// <summary>
        /// Select the team if multiplayer
        /// </summary>
        private int _teamSelection;

        /// <summary>
        /// the character that is currently selected
        /// </summary>
        private int _currentCharacter;

        /// <summary>
        /// saves the basicScale of the models
        /// </summary>
        private List<Vector3> _basicScales;

        /// <summary>
        /// Initialises all private variables
        /// </summary>
        public void Start()
        {
            GameManager.GetInstance().ResetItems();
            _net = GameManager.GetInstance().NetworkController;
            _noPlayer = GameManager.GetInstance().PlayerCount;
            _currentChoice = 0;
            _toolbarSelectionOld = 0;
            _toolbarSelectionNew = 0;
            _displayString = string.Empty;
            _team1 = new Team(1);
            _team2 = new Team(2);
            MyskinInfo = Resources.Load("GUI/Options") as GUISkin;
            _animationIsPlaying = false;
            GameManager.GetInstance().AddTeam(_team1);
            if (GameManager.GetInstance().TeamCount == 2)
            {
                GameManager.GetInstance().AddTeam(_team2);
            }

            InitCharacters(0);
            Hero currentCharacterHero = _characterList[_currentCharacter].GetComponent<Hero>();
            _displayString = "Char: " + currentCharacterHero.DisplayName
            + "\r\n" + "Max HP: " + currentCharacterHero.MaxHP + "\r\n"
            + "Max MP: " + currentCharacterHero.MaxMP + "\r\n"
            + "Basic Damage: " + currentCharacterHero.BasicDamage;

            _charName = "Player " + (_currentChoice + 1);
        }

        /// <summary>
        /// The GUI is drawn here
        /// </summary>
        public void OnGUI()
        {
            GUI.skin = Myskin;

            float boxHeight = Screen.height / 10;
            float boxWidth = Screen.width / 6;
            float buttonWidth = Screen.width / 9;
            float buttonHeight = Screen.height / 10;
            float lowerButtonsTop = Screen.height - (Screen.height * (13f / 24));
            float middleOffset = Screen.width / 10;
            float labelWidth = Screen.width / 4;
            float labelHeight = Screen.height / 3;
            float toolbarWidth = 250;
            float toolbarHeight = 25;
            float toolbarOffsetHeight = Screen.height / 6;
            float infoBoxOffsetWidth = 15 * Screen.width / 20;
            float infoBoxOffsetHeight = 3 * Screen.height / 10;

            string[] stringArray = new string[2] { "Heroes", "Enemies" };
            string[] stringArray2 = new string[2] { "Team 1", "Team 2" };

            Hero currentCharacterHero;
            Mob currentCharacterMob;

            GUI.skin = MyskinInfo;
            GUI.Box(new Rect((Screen.width / 2) - (3 * boxWidth / 2), Screen.height / 25, 3 * boxWidth, 2 * boxHeight), "Player " + (_currentChoice + 1) + "\nChoose your Character");
            GUI.skin = Myskin;

           // _toolbarSelectionNew = GUI.Toolbar(new Rect((Screen.width / 2) - (toolbarWidth / 2), toolbarOffsetHeight, toolbarWidth, toolbarHeight), _toolbarSelectionOld, stringArray);
            if (GameManager.GetInstance().TeamCount == 2)
            {
                /*if (_net.IsClient)
                {
                    GUI.enabled = false;
                    _teamSelection = GUI.Toolbar(new Rect((Screen.width / 2) - (toolbarWidth / 2), toolbarOffsetHeight + toolbarHeight, toolbarWidth, toolbarHeight), 0, stringArray2);
                    GUI.enabled = true;
                }
                else if (_net.IsServer)
                {
                    GUI.enabled = false;
                    _teamSelection = GUI.Toolbar(new Rect((Screen.width / 2) - (toolbarWidth / 2), toolbarOffsetHeight + toolbarHeight, toolbarWidth, toolbarHeight), 1, stringArray2);
                    GUI.enabled = true;
                }*/
                if (_net.IsClient || _net.IsServer)
                {
                    _teamSelection = _net.TeamNumber - 1;
                    GUI.Label(new Rect((Screen.width / 2) - (toolbarWidth / 2), toolbarOffsetHeight + toolbarHeight, toolbarWidth, toolbarHeight), "Your Team: " + (_teamSelection + 1));
                }
                else
                {
                    _teamSelection = GUI.Toolbar(new Rect((Screen.width / 2) - (toolbarWidth / 2), toolbarOffsetHeight + toolbarHeight, toolbarWidth, toolbarHeight), _teamSelection, stringArray2);
                }
            }

            if (GUI.changed && _toolbarSelectionOld != _toolbarSelectionNew)
            {
                _toolbarSelectionOld = _toolbarSelectionNew;
                DestroyCharacters();
                InitCharacters(_toolbarSelectionNew);
                if (_toolbarSelectionNew == 0)
                {
                    currentCharacterHero = _characterList[_currentCharacter].GetComponent<Hero>();
                    _displayString = "Char: " + currentCharacterHero.DisplayName
                    + "\r\n" + "Max HP: " + currentCharacterHero.MaxHP + "\r\n"
                    + "Max MP: " + currentCharacterHero.MaxMP + "\r\n"
                    + "Basic Damage: " + currentCharacterHero.BasicDamage;
                }
                else
                {
                    currentCharacterMob = _characterList[_currentCharacter].GetComponent<Mob>();
                    _displayString = "Char: " + currentCharacterMob.DisplayName
                    + "\r\n" + "Max HP: " + currentCharacterMob.MaxHP + "\r\n"
                    + "Max MP: " + currentCharacterMob.MaxMP + "\r\n"
                    + "Basic Damage: " + currentCharacterMob.BasicDamage;
                }
            }

            if (_toolbarSelectionOld == _toolbarSelectionNew)
            {
                if (_toolbarSelectionNew == 0)
                {
                    currentCharacterHero = _characterList[_currentCharacter].GetComponent<Hero>();
                    _displayString = "Char: " + currentCharacterHero.DisplayName
                    + "\r\n" + "MaxHP: " + currentCharacterHero.MaxHP + "\r\n"
                    + "MaxMP: " + currentCharacterHero.MaxMP + "\r\n"
                    + "Basic Damage: " + currentCharacterHero.BasicDamage;
                }
                else
                {
                    currentCharacterMob = _characterList[_currentCharacter].GetComponent<Mob>();
                    _displayString = "Char: " + currentCharacterMob.DisplayName
                    + "\r\n" + "MaxHP: " + currentCharacterMob.MaxHP + "\r\n"
                    + "MaxMP: " + currentCharacterMob.MaxMP + "\r\n"
                    + "Basic Damage: " + currentCharacterMob.BasicDamage;
                }
            }

            GUI.skin = MyskinRight;
            if (GUI.Button(new Rect((Screen.width / 2) + (middleOffset - (buttonWidth / 4)), lowerButtonsTop, buttonWidth / 2, buttonWidth / 2), string.Empty))
            {
                NextCharacterLeft();
            }

            GUI.skin = MyskinLeft;
            if (GUI.Button(new Rect((Screen.width / 2) - (middleOffset + (buttonWidth / 4)), lowerButtonsTop, buttonWidth / 2, buttonWidth / 2), string.Empty))
            {
                NextCharacterRight();
            }

            GUI.skin = Myskin;
            if (GUI.Button(new Rect((Screen.width / 2) - buttonWidth, Screen.height - (Screen.height / 5), 2 * buttonWidth, buttonHeight), "Fight!"))
            {
                if (!_animationIsPlaying)
                {
                    SetCharacter();
                }
            }

            if (GUI.Button(new Rect((Screen.width / 6) - (7 * buttonWidth / 5), Screen.height * (8.5f / 14), 7 * buttonWidth / 5, buttonHeight), "Main Menu"))
            {
                if (_net.IsServer)
                {
                    _net.Disconnect();
                }

                Application.LoadLevel("MainMenu");
            }

            if (GameManager.GetInstance().GameMode == GameManager.Mode.PLAY)
            {
                if (GUI.Button(new Rect((Screen.width / 6) - (7 * buttonWidth / 5), Screen.height - (Screen.height * (3.5f / 14)), 7 * buttonWidth / 5, buttonHeight), "Back"))
                {
                    Application.LoadLevel("GameSelection");
                }
            }
            else
            {
                if (GUI.Button(new Rect((Screen.width / 6) - (7 * buttonWidth / 5), Screen.height - (Screen.height * (3.5f / 14)), (7 * buttonWidth) / 5, buttonHeight), "Quit"))
                {
                    if (_net.IsServer)
                    {
                        _net.Disconnect();
                    }

                    Application.Quit();
                }
            }

            GUI.skin = MyskinInfo;
            GUI.Box(new Rect(infoBoxOffsetWidth, infoBoxOffsetHeight, 3 * boxWidth / 2, boxHeight), "Details");
            GUI.Button(new Rect(infoBoxOffsetWidth, infoBoxOffsetHeight, labelWidth, labelHeight), _displayString);
            _characterList[_currentCharacter].transform.Rotate(new Vector3(0, -10 * Time.deltaTime, 0));

            if (GameManager.GetInstance().GameMode == GameManager.Mode.PLAY)
            {
                GUI.Box(new Rect(infoBoxOffsetWidth, infoBoxOffsetHeight + labelHeight, 3 * boxWidth / 2, boxHeight), "Nickname:");
                _charName = GUI.TextField(new Rect(infoBoxOffsetWidth + (boxWidth / 4), infoBoxOffsetHeight + labelHeight + (boxWidth / 4), boxWidth, 2 * boxHeight / 3), _charName);
            }
        }

        /// <summary>
        /// Initializes the characters
        /// </summary>
        /// <param name="index">Index of character which should be initialized</param>
        private void InitCharacters(int index)
        {
            if (index == 0)
            {
                _characterList = new List<GameObject>(Datasheet.Heroes());
            }
            else
            {
                _characterList = new List<GameObject>(Datasheet.Mobs());
            }

            _totalCharacters = _characterList.Count;
            Debug.Log("[CharacterSelection.cs]: Number of heroes: " + _characterList.Count);
            _basicScales = new List<Vector3>(_totalCharacters);
            for (int i = 0; i < _totalCharacters; i++)
            {
                _basicScales.Add(new Vector3());
            }

            _currentCharacter = 0;
            float arc = 2 * Mathf.PI / _totalCharacters;
            float currentArc = 0;
            _positions = new List<Vector3>(_totalCharacters);
            for (int i = 0; i < _totalCharacters; i++)
            {
                _positions.Add(new Vector3((-80) * Mathf.Sin(currentArc), 0, (-80) * Mathf.Cos(currentArc)));
                currentArc += arc;
            }

            for (int i = 0; i < _totalCharacters; i++)
            {
                _characterList[i] = (GameObject)Instantiate(_characterList[i]);
                _characterList[i].GetComponent<Character>().Type = Character.Types.CHARACTER_SELECTION;
                if (index == 0)
                {
                    _basicScales[i] = _characterList[i].transform.localScale / 1.6f;
                    _characterList[i].transform.position = _positions[i] + (new Vector3(0, 0, 0) * _basicScales[i].y);
                }
                else
                {
                    _basicScales[i] = _characterList[i].transform.localScale / 1.6f;
                    _characterList[i].transform.position = _positions[i] + (new Vector3(0, 0, 0) * _basicScales[i].y);
                }
                
                SetScaleToBackground(i);
            }

            SetScaleToForeground(_currentCharacter);
        }

        private void DestroyCharacters()
        {
            for (int i = 0; i < _totalCharacters; i++)
            {
                Destroy(_characterList[i].gameObject);
            }
        }

        /// <summary>
        /// Sets the selected Character in GameManager and Loads the nextScene
        /// </summary>
        private void SetCharacter()
        {
            Character chosenCharacter;

            if (_toolbarSelectionNew == 0)
            {
                chosenCharacter = Datasheet.Heroes()[_currentCharacter].GetComponent<Hero>();
                chosenCharacter.DisplayName = _characterList[_currentCharacter].GetComponent<Hero>().DisplayName;
                chosenCharacter.Type = Character.Types.HUMAN;
                Debug.Log("Chosen character: " + chosenCharacter.Type);
                Hero animator = _characterList[_currentCharacter].GetComponent<Hero>();
                if (animator.animation != null)
                {
                    List<string> specialAttackNames = animator.SpecialAttacks();
                    if (specialAttackNames.Count > 0)
                    {
                        string animationName = specialAttackNames[Random.Range(0, specialAttackNames.Count)];
                        animator.animation.Play(animationName);
                        StartCoroutine(Wait(animator.animation[animationName].length / animator.animation[animationName].speed, chosenCharacter));
                        return; 
                    }
                }
            }
            else
            {
                chosenCharacter = Datasheet.Mobs()[_currentCharacter].GetComponent<Mob>();
                chosenCharacter.Type = Character.Types.HUMAN;

                Mob animator = _characterList[_currentCharacter].GetComponent<Mob>();
                if (animator.animation != null)
                {
                        animator.animation.Play("Attacking");
                        StartCoroutine(Wait(animator.animation["Attacking"].length / animator.animation["Attacking"].speed, chosenCharacter));
                        return;
                }
            }

            // If we reach this, then there wasn't an animation to play. Bummer.
            // Continue and add this character without playing an animation.
            AddCharacter(chosenCharacter);
        }

        /// <summary>
        /// Selects the previous Character and rotates the positions, sets Characters to the new Position
        /// </summary>
        private void NextCharacterLeft()
        {
            SetScaleToBackground(_currentCharacter);
            if (_currentCharacter == 0)
            {
                _currentCharacter = _totalCharacters;
                Debug.Log(_currentCharacter);
            }

            _currentCharacter -= 1;
            Vector3 tmpPos = _positions[0];
            for (int i = 0; i < _totalCharacters - 1; i++)
            {
                _positions[i] = _positions[i + 1];
            }

            _positions[_totalCharacters - 1] = tmpPos;
            UpdateCharacterPositions();
            SetScaleToForeground(_currentCharacter);
        }

        /// <summary>
        /// Selects the next Character and rotates the positions, sets Characters to the new Positio
        /// </summary>
        private void NextCharacterRight()
        {
            SetScaleToBackground(_currentCharacter);
            _currentCharacter = (_currentCharacter + 1) % _totalCharacters;
            Vector3 tmpPos = _positions[_totalCharacters - 1];
            for (int i = _totalCharacters - 1; i > 0; i--)
            {
                _positions[i] = _positions[i - 1];
            }

            _positions[0] = tmpPos;
            UpdateCharacterPositions();
            SetScaleToForeground(_currentCharacter);
            Debug.Log(_currentCharacter);
        }

        /// <summary>
        /// Sets the scale and rotation of the character at the given position in the _characterList to background Scale
        /// </summary>
        /// <param name="index">the index of the Character to scale</param>
        private void SetScaleToBackground(int index)
        {
            _characterList[index].transform.localScale = new Vector3(_basicScales[index].x * _backgroundScale.x, _basicScales[index].y * _backgroundScale.y, _basicScales[index].z * _backgroundScale.z);
            _characterList[index].transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }

        /// <summary>
        /// Sets the scale to Foregound Scale
        /// </summary>
        /// <param name="index">Character to Scale</param>
        private void SetScaleToForeground(int index)
        {
            _characterList[index].transform.localScale = new Vector3(_basicScales[index].x * _foregroundScale.x, _basicScales[index].y * _foregroundScale.y, _basicScales[index].z * _foregroundScale.z);
            if (_toolbarSelectionNew == 0)
            {
                _characterList[index].transform.position = _characterList[index].transform.position + (new Vector3(0, -13, 0) * _basicScales[index].y);
            }
            else
            {
                _characterList[index].transform.position = _characterList[index].transform.position + (new Vector3(-1, -2, 0) * _basicScales[index].y);
                _characterList[index].transform.position = _characterList[index].transform.position = new Vector3(0, -13, _characterList[index].transform.position.z);
                Debug.Log(_characterList[index].transform.position.y);
                Debug.Log(_characterList[index].name);
            }
        }

        /// <summary>
        /// updates the Character positions according to _positions
        /// </summary>
        private void UpdateCharacterPositions()
        {
            for (int i = 0; i < _totalCharacters; i++)
            {
                _characterList[i].transform.position = _positions[i];
            }
        }

        /// <summary>
        /// Chooses Character with animation
        /// </summary>
        /// <param name="waitTime">Time of transition</param>
        /// <param name="chosenCharacter">Character which should be moved</param>
        /// <returns>IEnumator for Coroutine</returns>
        private IEnumerator Wait(float waitTime, Character chosenCharacter)
        {
            _animationIsPlaying = true;
            yield return new WaitForSeconds(waitTime);
            AddCharacter(chosenCharacter);

            _animationIsPlaying = false;
        }

        /// <summary>
        /// Add a selected character to the game and start it if all characters are chosen.
        /// </summary>
        /// <param name="chosenCharacter">Character to add.</param>
        private void AddCharacter(Character chosenCharacter)
        {
            if (_net.IsClient || _net.IsServer)
            {
                _net.Character = chosenCharacter.name;
            }
            else
            {
                if (GameManager.GetInstance().GameMode == GameManager.Mode.PLAY)
                {
                    chosenCharacter.DisplayName = _charName;
                }

                if (GameManager.GetInstance().TeamCount == 2)
                {
                    if (_teamSelection == 0)
                    {
                        _team1.AddMember(chosenCharacter);
                    }
                    else
                    {
                        _team2.AddMember(chosenCharacter);
                    }
                }
                else
                {
                    _team1.AddMember(chosenCharacter);
                }
            }

            if (_currentChoice + 1 == _noPlayer)
            {
                if (GameManager.GetInstance().GameMode == GameManager.Mode.PLAY)
                {
                    GameManager.GetInstance().SceneToBeLoaded = "DummyLevelBenny";
                    Application.LoadLevel("LoadingScreen");
                }
                else if (GameManager.GetInstance().GameMode == GameManager.Mode.SPECIAL)
                {
                    GameManager.GetInstance().SceneToBeLoaded = "SM_Game";
                    if (_net.IsClient)
                    {
                        Application.LoadLevel("SMReady");
                    }
                    else if (_net.IsServer)
                    {
                        _net.NetworkLoadLevel("LoadingScreen");
                    }
                    else
                    {
                        // Application.LoadLevel("SM_Game");
                        Application.LoadLevel("LoadingScreen");
                    }
                }
            }
            else
            {
                _currentChoice++;
                _charName = "Player " + (_currentChoice + 1);
            }
        }

        /// <summary>
        /// Returns list of all animation names attached to gameObject
        /// </summary>
        /// <param name="anim">Animation compoment</param>
        /// <returns>Stringarray with names of animations attached to component</returns>
        private string[] AnimationNames(Animation anim)
        {
            List<string> tmplist = new List<string>();

            foreach (AnimationState state in anim)
            {
                tmplist.Add(state.name);
            }

            return tmplist.ToArray();
        }
    }
}

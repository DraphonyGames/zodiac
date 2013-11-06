namespace Assets.Scripts.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Assets.Scripts.Mobs;
    using UnityEngine;

    /// <summary>
    /// Draws the statisticScreen for the Game
    /// </summary>
    public class Statistics : MonoBehaviour
    {
        /// <summary>
        /// the used Skin
        /// </summary>
        private GUISkin _mySkin;

        /// <summary>
        /// GUIStyle to beutify box without overriding GUISkin
        /// </summary>
        private GUIStyle _boxStyle;

        /// <summary>
        /// GUIStyle to beutify button without overriding GUISkin
        /// </summary>
        private GUIStyle _buttonStyle;

        /// <summary>
        /// the total Offset to the Left
        /// </summary>
        private float _leftOffset;

        /// <summary>
        /// totalOffset to the right
        /// </summary>
        private float _rightOffset;

        /// <summary>
        /// total Offset on top
        /// </summary>
        private float _topOffset;

        /// <summary>
        /// total offset from Button
        /// </summary>
        private float _buttonOffset;

        /// <summary>
        /// Width of the area to display statboxes in is calculated based on left and right offset
        /// </summary>
        private float _displayWidth;

        /// <summary>
        /// height of the area to dispay statboxes in, is calculated based on top and button offset
        /// </summary>
        private float _displayHeight;

        /// <summary>
        /// the y coordinate of the topleft corner of the displayArea
        /// </summary>
        private float _displayTop;

        /// <summary>
        /// the x coordinate of the topleft corner of the displayArea
        /// </summary>
        private float _displayLeft;

        /// <summary>
        /// the Width of an indiviual HeroStatBox, calculated based on _displayWidth and number of Heroes
        /// </summary>
        private float _heroWidth;

        /// <summary>
        /// the Height of an individual HeroStatBox, calculated based on _displayHeight and number of Heroes
        /// </summary>
        private float _heroHeight;

        /// <summary>
        /// Number of Heroes to Display
        /// </summary>
        private int _playerCount;

        /// <summary>
        /// The List of Heroe to Display
        /// </summary>
        private List<Character> _humanCharacters;

        /// <summary>
        /// Width of a single Button
        /// </summary>
        private float _buttonWidth;

        /// <summary>
        /// Heigt of a single button
        /// </summary>
        private float _buttonHeight;

        /// <summary>
        /// y coordinate of topleft corner of button
        /// </summary>
        private float _buttonTop;

        /// <summary>
        /// x Coordinate of topleft button corner
        /// </summary>
        private float _buttonLeft;

        private int _winningTeam;

        private List<Team> _teamList;

        /// <summary>
        /// Controller which holds all connection / diconnection logic.
        /// </summary>
        private NetworkController _net;

        /// <summary>
        /// initializes GUISkin
        /// </summary>
        public void Start()
        {
            _mySkin = Resources.Load("GUI/zodiac") as GUISkin;
            _teamList = GameManager.GetInstance().GetAllTeams();
            _net = GameManager.GetInstance().NetworkController;

            if (GameManager.GetInstance().GameMode == GameManager.Mode.SPECIAL)
            {
                Debug.Log(_teamList[0].TeamNo);
                if (_teamList[0].Base.HP <= 0 ^ _teamList[1].Base.HP <= 0)
                {
                    _winningTeam = (_teamList[0].Base.HP == 0) ? 2 : 1;
                }

                if ((_winningTeam == 0) && _teamList[0].Points >= GameManager.SPECIALWINNINGPOINTS ^ _teamList[1].Points >= GameManager.SPECIALWINNINGPOINTS)
                {
                    _winningTeam = (_teamList[0].Points >= GameManager.SPECIALWINNINGPOINTS) ? 1 : 2;
                }

                if (_teamList[0].Points >= GameManager.SPECIALWINNINGPOINTS &&
                    _teamList[1].Points >= GameManager.SPECIALWINNINGPOINTS)
                {
                    _winningTeam = (_teamList[0].Points > _teamList[1].Points) ? 1 : 2;
                    if (_winningTeam == 0)
                    {
                        _winningTeam = 3;
                    }
                }

                Debug.Log("Winning Team " + _winningTeam);
            }
        }

        /// <summary>
        /// The GUI Magic happens here
        /// </summary>
        public void OnGUI()
        {
            InitializeValues();
            DrawStatBoxes();
            DrawButtons();
        }

        /// <summary>
        /// Called when this GameObject is destroyed by Unity.
        /// </summary>
        public void OnDestroy()
        {
            // Make sure we destroy all the immortal team members (we needed them on this screen
            // to display stats etc.).
            GameManager.GetInstance().CleanupTeamMembers();
        }

        /// <summary>
        /// Draws the MainMenuButton;
        /// </summary>
        private void DrawButtons()
        {
            if (GUI.Button(new Rect(_buttonLeft, _buttonTop, _buttonWidth, _buttonHeight), "Main Menu", _buttonStyle))
            {
                Application.LoadLevel("MainMenu");
            }
        }

        /// <summary>
        /// Draws the actual Statistic boxes
        /// </summary>
        private void DrawStatBoxes()
        {
            for (int i = 0; i < _playerCount; i++)
            {
                Character currentCharacter = _humanCharacters[i];
                int ceiledHalf = Mathf.CeilToInt(_playerCount / 2f);

                // offsets are calculated to make 2 rows of boxes
                float heroOffset = (i < ceiledHalf || _playerCount <= 2) ? i : i - ceiledHalf;
                float heroTop = (i < ceiledHalf || _playerCount <= 2) ? _displayTop : _displayTop + _heroHeight;

                // the acutal InfoString
                string winLoose = String.Empty;
                string notSpecialmodeStats = String.Empty;
                string ticketString = String.Empty;
                string nameString = currentCharacter.DisplayName;
                if (GameManager.GetInstance().GameMode == GameManager.Mode.PLAY)
                {
                    winLoose = (currentCharacter.Team.GetTotalHP() <= 0) ? "You lose" : "You win";
                    notSpecialmodeStats = "Freed " + currentCharacter.StatsMercenariesFreed + " Mercenaries \r\n" +
                    "Lost " + currentCharacter.StatsMercenariesLost + " Mercernaries \r\n";
                }

                if (GameManager.GetInstance().GameMode == GameManager.Mode.SPECIAL)
                {
                    _boxStyle.normal.textColor = currentCharacter.Team.TeamNo == 1 ? Color.blue : Color.red;
                    nameString += "(Team " + currentCharacter.Team.TeamNo + ")";
                    ticketString = (GameManager.GetInstance().GameMode == GameManager.Mode.SPECIAL)
                                        ? "Recieved " + currentCharacter.Team.Points + " points \r\n" : string.Empty;
                    winLoose = (currentCharacter.Team.TeamNo == _winningTeam || _winningTeam == 3 || _net.OpponentEscaped == currentCharacter.Team.TeamNo) ? "Your team wins" : "Your team loses";
                }

                string characterString = nameString + "\r\n\r\n" +
                                        "Lost " + currentCharacter.StatsLostHP + " HP \r\n" +
                                        "Used " + currentCharacter.StatsUsedHealthPacks + " Health Packs \r\n" +
                                        "Killed " + currentCharacter.StatsKillCount + " Enemies\r\n" +
                                        "Dealt " + currentCharacter.StatsEnemyDealtDamage + " Damage \r\n" +
                                        notSpecialmodeStats + ticketString + winLoose;

                GUI.Box(new Rect(_leftOffset + (heroOffset * _heroWidth), heroTop, _heroWidth, _heroHeight), characterString, _boxStyle);
            }
        }

        /// <summary>
        /// Counts the HUMAN players and adds them to the HumanCharacters List
        /// </summary>
        /// <returns>the number of human players</returns>
        private int CountPlayers()
        {
            int playerCount = 0;
            List<Team> teamList = GameManager.GetInstance().GetAllTeams();
            _humanCharacters = new List<Character>();
            foreach (Team t in teamList)
            {
                List<Character> teamMembers = t.GetAllTeamMembers();
                for (int i = 0; i < teamMembers.Count; i++)
                {
                    if (teamMembers[i].Type == Character.Types.HUMAN && !(teamMembers[i] is Mobs.Base))
                    {
                        playerCount++;
                        _humanCharacters.Add(teamMembers[i]);
                    }
                }
            }

            return playerCount;
        }

        /// <summary>
        /// inititlizes the most important values
        /// </summary>
        private void InitializeValues()
        {
            _leftOffset = Screen.width * (1f / 20);
            _rightOffset = Screen.width * (5f / 20);
            _buttonOffset = Screen.width * (1f / 20);
            _topOffset = Screen.width * (1f / 10);
            _displayWidth = Screen.width - (_leftOffset + _rightOffset);
            _displayHeight = Screen.height - (_topOffset + _buttonOffset);
            _displayTop = _topOffset;
            _buttonWidth = Screen.width / 7;
            _buttonHeight = Screen.height / 8;
            _buttonLeft = Screen.width - (_rightOffset * (8f / 10));
            _buttonTop = Screen.height * (2f / 8);
            _playerCount = CountPlayers();
            _heroHeight = _playerCount <= 2 ? _displayHeight : _displayHeight / 2;
            _heroWidth = _playerCount <= 2 ? (_displayWidth / _playerCount) : _displayWidth / Mathf.CeilToInt(_playerCount / 2f);
            GUI.skin = _mySkin;
            _playerCount = CountPlayers();
            _heroHeight = _playerCount <= 2 ? _displayHeight : _displayHeight / 2;
            _heroWidth = _playerCount <= 2 ? (_displayWidth / _playerCount) : _displayWidth / Mathf.CeilToInt(_playerCount / 2f);
            _boxStyle = new GUIStyle(GUI.skin.box);
            _boxStyle.fontSize = (int)(0.04 * _heroWidth);
            _boxStyle.alignment = TextAnchor.MiddleCenter;
            _buttonStyle = new GUIStyle(GUI.skin.button);
            _buttonStyle.fontSize = (int)(0.1 * _buttonWidth);
        }
    }
}
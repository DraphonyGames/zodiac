namespace Assets.Scripts.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The Class is put on the Camera in a Game to draw the in game UI
    /// </summary>
    public class InGame : MonoBehaviour
    {
        /// <summary>
        /// The NetworkController
        /// </summary>
        private NetworkController _net;

        /// <summary>
        /// the Texture for the Healthbar
        /// </summary>
        private Texture2D _healthBarTexture;

        /// <summary>
        /// the Prefab of the Manabar
        /// </summary>
        private Texture2D _manaBarTexture;

        /// <summary>
        /// the Prefab of the Staminabar
        /// </summary>
        private Texture2D _staminaBarTexture;

        /// <summary>
        /// list of all Teams that are currently on the map
        /// </summary>
        private List<Team> _teamList;

        /// <summary>
        /// the List of all teams with hero players
        /// </summary>
        private List<Team> _teamListWithHeroes;

        /// <summary>
        /// gives the Offset in x direction
        /// </summary>
        private float _uiOffsetHeight = 0;

        /// <summary>
        /// gives the Offset in y direction
        /// </summary>
        private float _uiOffsetWidth = 0;

        /// <summary>
        /// the percentage of the Screen Taken by the InGameUI
        /// </summary>
        private float _fillingLevel = 0.15f;

        /// <summary>
        /// The Offset of the team area to the left
        /// </summary>
        private float _teamAreaOffsetLeft = 0;

        /// <summary>
        /// the Offset of the team area to the right.
        /// </summary>
        private float _teamAreaOffsetRight;

        /// <summary>
        /// the total width of the Team Area
        /// </summary>
        private float _teamAreaWidth;

        /// <summary>
        /// the used Skin
        /// </summary>
        private GUISkin _mySkin;

        /// <summary>
        /// the total width used to display things 
        /// </summary>
        private float _displayWidth;

        /// <summary>
        /// the percentage of the display used for the InfoSection
        /// </summary>
        private float _infoPercentage = 1f / 6;

        /// <summary>
        /// the Left Coordinate of the display
        /// </summary>
        private float _displayLeft;

        /// <summary>
        /// the top Coordinate of the display
        /// </summary>
        private float _displayTop;

        private GameManager _manager = GameManager.GetInstance();

        /// <summary>
        /// initialising the values;
        /// </summary>
        public void Start()
        {
            _healthBarTexture = Resources.Load("HealthBarTexture") as Texture2D;
            _manaBarTexture = Resources.Load("ManaBarTexture") as Texture2D;
            _staminaBarTexture = Resources.Load("StaminaBarTexture") as Texture2D;
            _teamListWithHeroes = new List<Team>();
            _mySkin = Resources.Load("GUI/zodiac") as GUISkin;
            _net = _manager.NetworkController;
        }

        /// <summary>
        /// This is where the inGameGUI is defined
        /// </summary>
        public void OnGUI()
        {
            _displayWidth = Screen.width - (2 * _uiOffsetWidth);
            _displayLeft = _uiOffsetWidth;
            _displayTop = _uiOffsetHeight;
            if (_manager.GameMode == GameManager.Mode.PLAY)
            {
                if (_teamListWithHeroes.Count == 1 && (GameManager.GetInstance().GetAllHeroes().Count <= 2))
                {
                    _uiOffsetWidth = Screen.width / 6;
                }
                else
                {
                    _uiOffsetWidth = 0;
                }

                GUI.skin = _mySkin;
                _teamList = GameManager.GetInstance().GetAllTeams();
                if (_teamList.Count > 0)
                {
                    DrawTeamBoxesPlay();
                }

                DrawInfoBox();
                DrawMobBox();
            }
            else if (_manager.GameMode == GameManager.Mode.SPECIAL)
            {
                GUI.skin = _mySkin;
                _teamList = GameManager.GetInstance().GetAllTeams();
                DrawTeamBoxesSpecial();
            }
        }

        /// <summary>
        /// Draws the TeamBoxes for the Special mode
        /// </summary>
        private void DrawTeamBoxesSpecial()
        {
            float teamAreaLeft = _displayLeft;
            float teamAreaTop = _displayTop;
            _infoPercentage = 0;
            _teamAreaWidth = _displayWidth * (1 - _infoPercentage);
            GUIStyle teamLabelStyle = new GUIStyle(GUI.skin.label);
            GUIStyle baseLabelStyle = new GUIStyle(GUI.skin.label);
            GUIStyle infoTextStyle = new GUIStyle(GUI.skin.label);
            baseLabelStyle.alignment = TextAnchor.MiddleLeft;
            teamLabelStyle.alignment = TextAnchor.UpperLeft;
            infoTextStyle.alignment = TextAnchor.MiddleCenter;
            float teamWidth = _teamAreaWidth / _teamList.Count;
            float teamHeight = Screen.height * _fillingLevel * (2f / 3);
            teamLabelStyle.fontSize = (int)(teamWidth * .02f);

            // the height of the Team area is determined by the filling level (it is 2/3 because the mercenary boxes also have to fit)
            for (int i = 0; i < _teamList.Count; i++)
            {
                Texture2D barTexture = (i == 0) ? _manaBarTexture : _healthBarTexture;
                List<Character> characterList = _teamList[i].GetAllTeamMembers();

                int mCount = _teamList[i].GetMercenaryTeamMembers().Count + _teamList[i].GetMobTeamMembers().Count;
                float teamXPos = _displayLeft + (i * teamWidth);
                GUI.Label(new Rect(teamXPos, teamAreaTop, teamWidth, teamHeight), "Team " + _teamList[i].TeamNo + ":", teamLabelStyle);
                float boxOffset = 1f / 10 * teamWidth;
                float boxWidth = teamWidth - boxOffset;
                float baseLabelWidth = boxWidth / 8;
                float baseLabelLeft = (teamXPos + boxOffset) + (boxOffset / 10);
                float barWidth = (boxWidth / 2) - baseLabelWidth;
                float barFactor = ((float)_teamList[i].Base.HP) / _teamList[i].Base.MaxHP;
                string infoString = "Captured " + _teamList[i].CapturedCheckpoints.Count + " Checkpoint\r\n" +
                                   "Received " + _teamList[i].Points + " Points\r\n" +
                                   "There are " + mCount + " servants";
                GUI.Box(new Rect(teamXPos + boxOffset, teamAreaTop, boxWidth, teamHeight), string.Empty);
                GUI.Label(new Rect(baseLabelLeft, teamAreaTop, baseLabelWidth, teamHeight), "Base: ", baseLabelStyle);
                GUI.DrawTexture(new Rect(baseLabelLeft + baseLabelWidth, teamAreaTop + (teamHeight / 3), barFactor * barWidth, teamHeight / 3), barTexture);
                GUI.Label(new Rect(baseLabelLeft + baseLabelWidth + barWidth + (boxOffset / 10), teamAreaTop, (teamWidth / 2) - (boxOffset / 10), teamHeight), infoString, baseLabelStyle);
            }

            int c = 0;
            if (_net.IsServer)
            {
                c = GameObject.Find("HeroServer").GetComponent<Hero>().RespawnCountdown;
            }
            else if (_net.IsClient)
            {
                c = GameObject.Find("HeroClient" + _net.ClientNumber).GetComponent<Hero>().RespawnCountdown;
            }
            else
            {
                c = GameObject.Find("Hero1").GetComponent<Hero>().RespawnCountdown;
            }

            if (c > 0)
            {
                GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.normal.textColor = Color.red;
                labelStyle.fontSize = (int)(0.2 * Screen.width);
                GUIOperations.DrawLabelCenteredAt(Screen.width / 2, Screen.height / 3, (int)(0.05 * Screen.width), "Respawning in:");
                GUIOperations.DrawLabelCenteredAt(Screen.width / 2, 2 * Screen.height / 3, (int)(0.2 * Screen.width), c.ToString(), labelStyle);
            }
        }

        /// <summary>
        /// Draws the InfoBox with currentLevel, currentSection, spwaningCountdown
        /// </summary>
        private void DrawInfoBox()
        {
            // float infoBoxLeft = Screen.width - _teamAreaOffsetRight-_UIOffsetWidth;
            float infoBoxLeft = _displayLeft + (_displayWidth * (1 - _infoPercentage));
            float infoBoxTop = _displayTop;
            float infoBoxWidth = _displayWidth * _infoPercentage;
            float infoBoxHeight = Screen.height * _fillingLevel;
            GUIStyle infoBoxStyle = new GUIStyle(GUI.skin.GetStyle("box"));
            infoBoxStyle.fontSize = (int)(0.08f * infoBoxWidth);
            infoBoxStyle.alignment = TextAnchor.UpperCenter;
            string mobString = string.Empty;
            if (GameManager.GetInstance().CurrentSection != null)
            {
                mobString = (GameManager.GetInstance().CurrentSection.MobSpawner.Countdown == -1) ? "All mobs spawned" : "Mob Countdown: " + GameManager.GetInstance().CurrentSection.MobSpawner.Countdown;
            }

            string infoString = " ";
            if (_manager.GameMode == GameManager.Mode.PLAY)
            {
                infoString = "Game information:\r\n" +
                    "Level: " + GameManager.GetInstance().LevelNo + "\r\n" +
                    "Section: " + GameManager.GetInstance().SectionNo + "\r\n" + "\r\n" + mobString;
            }

            GUI.Box(new Rect(infoBoxLeft, infoBoxTop, infoBoxWidth, infoBoxHeight), infoString, infoBoxStyle);
        }

        /// <summary>
        /// Draws the Box to display Mob statistics
        /// </summary>
        private void DrawMobBox()
        {
            if (GameManager.GetInstance().CurrentSection != null)
            {
                float mobBoxWidth = _displayWidth * _infoPercentage;
                float mobBoxHeight = Screen.height / 12;
                float mobBoxLeft = _displayLeft + _displayWidth - mobBoxWidth;
                GUIStyle mobStyle = new GUIStyle(GUI.skin.box);
                mobStyle.fontSize = (int)(0.07f * mobBoxWidth);
                GUI.Box(new Rect(mobBoxLeft, Screen.height - mobBoxHeight, mobBoxWidth, mobBoxHeight), "Remaining Mobs: " + GameManager.GetInstance().CurrentSection.RemainingMobs(), mobStyle);
            }
        }

        /// <summary>
        /// iterates over all Team and calls the DrawCharacterBox method to draw the Boxes
        /// The logic for positioning the Boxes happens here
        /// </summary>
        private void DrawTeamBoxesPlay()
        {
            float teamAreaLeft = _displayLeft;
            float teamAreaTop = _displayTop;
            _teamAreaWidth = _displayWidth * (1 - _infoPercentage);
            GUIStyle teamLabelStyle = new GUIStyle(GUI.skin.label);

            teamLabelStyle.alignment = TextAnchor.UpperLeft;

            GUIStyle mercBoxStyle = new GUIStyle(GUI.skin.box);

            // iterates trough all teams, the values for the xPosition, the y position, the offset for the hero Boxes and the size of the
            // hero Boxes are set realtiv to the offsets and the width of the team
            for (int i = 0; i < _teamList.Count; i++)
            {
                if (_teamList[i].GetHeroTeamMembers().Count > 0 && !_teamListWithHeroes.Contains(_teamList[i]))
                {
                    _teamListWithHeroes.Add(_teamList[i]);
                }
            }

            // each Team has got the same space on the Screen (and has to fit in)
            // there can be an Offset (if wanted)
            float teamWidth = _teamAreaWidth / _teamListWithHeroes.Count;
            teamLabelStyle.fontSize = (int)(teamWidth * .02f);

            // the height of the Team area is determined by the filling level (it is 2/3 because the mercenary boxes also have to fit)
            float teamHeight = Screen.height * _fillingLevel * (2f / 3);
            for (int i = 0; i < _teamListWithHeroes.Count; i++)
            {
                float teamXPos = _displayLeft + (i * teamWidth);
                GUI.Label(new Rect(teamXPos, teamAreaTop, teamWidth, teamHeight), "Team " + _teamListWithHeroes[i].TeamNo + ":", teamLabelStyle);
                float heroBoxOffset = 1f / 10 * teamWidth;
                float heroBoxWidth = (teamWidth / Mathf.CeilToInt(_teamListWithHeroes[i].GetHeroTeamMembers().Count / 2f)) - heroBoxOffset;
                float heroBoxHeight = teamHeight / 2;

                // positions of all heroBoxes in a team are calculated and the boxes are drawn
                for (int j = 0; j < _teamListWithHeroes[i].GetHeroTeamMembers().Count; j++)
                {
                    float roundedHalf = Mathf.CeilToInt(_teamListWithHeroes[i].GetHeroTeamMembers().Count / 2f);
                    float heroBoxXposFactor = j < roundedHalf ? j : j - roundedHalf;
                    float heroBoxYpos = j < roundedHalf ? 0 : heroBoxHeight;
                    float heroBoxXpos = teamXPos + (heroBoxXposFactor * heroBoxWidth) + heroBoxOffset;
                    DrawCharacterBox(heroBoxXpos, heroBoxYpos, heroBoxWidth, heroBoxHeight, _teamListWithHeroes[i].GetHeroTeamMembers()[j]);
                }

                // the boxes for the mercanaries are drawn
                List<Mercenary> mercMembers = _teamListWithHeroes[i].GetMercenaryTeamMembers();
                int mercenaryLife = 0;
                int mercenaryAmount = 0;
                for (int n = 0; n < mercMembers.Count; n++)
                {
                    mercenaryLife += mercMembers[n].HP;
                    mercenaryAmount += 1;
                }

                mercBoxStyle.fontSize = (int)(heroBoxWidth * 0.03f);
                GUI.Box(new Rect(teamXPos + heroBoxOffset, (Screen.height * _fillingLevel * (2f / 3)) + _displayTop, heroBoxWidth, heroBoxHeight), mercenaryAmount + " Mercenaries with " + mercenaryLife + " HP", mercBoxStyle);
            }
        }

        /// <summary>
        /// Draws the Box with Health and Manabar for the given hero
        /// </summary>
        /// <param name="left">the minimal x-Coordinate of the Box</param>
        /// <param name="top"> tht minimal y-Coordinate of the Box</param>
        /// <param name="width">the width of the box</param>
        /// <param name="height">the Height of the Box</param>
        /// <param name="hero">the hero of which the box is to be drawn</param>
        private void DrawCharacterBox(float left, float top, float width, float height, Hero hero)
        {
            float barHeight = height / 7;
            float itemInfoTop = top;
            float itemInfoHeight = height;
            float itemInfoWidth = itemInfoHeight;
            float barRightOffset = itemInfoWidth;
            float barLeftOffset = width / 4;
            float itemInfoLeft = (left + width) - itemInfoWidth;
            float barBasicWidth = width - (barRightOffset + barLeftOffset);
            float healthBarWidth = barBasicWidth * hero.HP / hero.MaxHP;
            float manaBarWidth = barBasicWidth * hero.MP / hero.MaxMP;
            float staminaBarWidth = barBasicWidth * hero.Stamina / hero.MaxStamina;
            float barTopOffset = barHeight;
            float healthBarTop = top + barTopOffset;
            float healthBarLeft = left + barLeftOffset;
            float manaBarTop = healthBarTop + barTopOffset + barHeight;
            float manaBarLeft = healthBarLeft;
            float staminaBarTop = manaBarTop + barTopOffset + barHeight;
            float staminaBarLeft = healthBarLeft;

            GUIStyle itemInfoStyle = new GUIStyle(GUI.skin.label);
            itemInfoStyle.fontSize = (int)(0.02 * width);
            itemInfoStyle.alignment = TextAnchor.MiddleCenter;
            GUIStyle characterBoxStyle = new GUIStyle(GUI.skin.box);
            characterBoxStyle.fontSize = (int)(0.04 * (width - barLeftOffset));
            GUI.skin.label.alignment = TextAnchor.UpperLeft;
            GUI.skin.box.alignment = TextAnchor.MiddleLeft;

            GUI.Box(new Rect(left, top, width, height), hero.DisplayName, characterBoxStyle);
            GUI.DrawTexture(new Rect(healthBarLeft, healthBarTop, healthBarWidth, barHeight), _healthBarTexture);
            GUI.DrawTexture(new Rect(manaBarLeft, manaBarTop, manaBarWidth, barHeight), _manaBarTexture);
            GUI.DrawTexture(new Rect(staminaBarLeft, staminaBarTop, staminaBarWidth, barHeight), _staminaBarTexture);
            if (hero.CurrentItem != null)
            {
                GUI.DrawTexture(new Rect(itemInfoLeft, itemInfoTop, itemInfoWidth, itemInfoHeight), hero.CurrentItem.ItemTexture);
                if (hero.CurrentItem.ItemInUse)
                {
                    GUI.Label(new Rect(itemInfoLeft, itemInfoTop, itemInfoWidth, itemInfoHeight), "u", itemInfoStyle);
                }
            }
        }
    }
}

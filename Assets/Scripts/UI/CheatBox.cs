namespace Assets.Scripts.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Implements a cheat console.
    /// </summary>
    public class CheatBox : MonoBehaviour
    {
        /// <summary>
        /// Dictionary holding the associations between command names
        /// and command callbacks.
        /// </summary>
        private Dictionary<string, CheatCommand> _cheatCommands;

        private bool _screenTrigger;
        private string _cheatString;
        private string _cheatStateString;
        private GUISkin _mySkin;

        /// <summary>
        /// Delegate for cheat commands.
        /// </summary>
        /// <param name="argv">The argument vector. Index 0 contains the command name.</param>
        private delegate void CheatCommand(string[] argv);

        /// <summary>
        /// Unity callback; used for initialization.
        /// </summary>
        public void Start()
        {
            _mySkin = Resources.Load("GUI/zodiac") as GUISkin;
            _cheatCommands = new Dictionary<string, CheatCommand>();

            // Add cheat commands.
            _cheatCommands["godmode"]   = Godmode;
            _cheatCommands["heal"]      = Heal;
            _cheatCommands["kill"]      = Kill;
            _cheatCommands["stuck"]     = Stuck;
            _cheatCommands["killmobs"]  = KillAllMobs;
            _cheatCommands["endgame"]   = EndGame;
            _cheatCommands["wingamespecial"] = WinGameSpecial;
            _cheatCommands["destroybase"] = DestroyBase;
        }

        /// <summary>
        /// Unity callback; called once per frame.
        /// </summary>
        public void Update()
        {
            if (ControlKeysManager.GetKeyDown(KeyCode.F1))
            {
                TriggerSheet();
            }
        }

        /// <summary>
        /// Show or hide the console, depending on its current state.
        /// </summary>
        public void TriggerSheet()
        {
            _cheatString = string.Empty;
            _cheatStateString = "Enter a command:";

            _screenTrigger = !_screenTrigger;
            Time.timeScale = _screenTrigger ? 0 : 1;
        }

        /// <summary>
        /// Unity callback; called when the GUI should be (re-)drawn.
        /// </summary>
        /// <remarks>Attention: May be called more than once in a single frame!</remarks>
        public void OnGUI()
        {
            GUI.skin = _mySkin;
            if (_screenTrigger)
            {
                float cheatBoxLeft = 0;
                float cheatWidth = Screen.width / 3;
                float cheatHeight = Screen.height / 10;
                float cheatTop = Screen.height - cheatHeight;
                float cheatTextWidth = cheatWidth - (cheatWidth / 5);
                float cheatTextLeft = cheatBoxLeft + cheatWidth - cheatTextWidth;

                GUIStyle cheatStyle = new GUIStyle(GUI.skin.box);
                cheatStyle.fontSize = (int)(0.03f * cheatWidth);
                cheatStyle.alignment = TextAnchor.MiddleLeft;

                GUIStyle cheatTextStyle = new GUIStyle(GUI.skin.textField);
                cheatTextStyle.alignment = TextAnchor.MiddleLeft;
                cheatTextStyle.fontSize = cheatStyle.fontSize;

                GUIStyle cheatStateStyle = new GUIStyle(cheatStyle);
                cheatStateStyle.alignment = TextAnchor.MiddleLeft;
                cheatStateStyle.fontSize = cheatStyle.fontSize;
                cheatStateStyle.normal.background = null;

                GUI.Box(new Rect(cheatBoxLeft, cheatTop, cheatWidth, cheatHeight), " CHEATS", cheatStyle);
                GUI.Box(new Rect(cheatTextLeft, cheatTop, cheatTextWidth, cheatHeight / 3),
                    _cheatStateString,
                    cheatStateStyle);

                GUI.SetNextControlName("CheatInput");
                _cheatString = GUI.TextField(new Rect(cheatTextLeft,
                        cheatTop + (cheatHeight / 3),
                        cheatTextWidth - 6,
                        (cheatHeight * 2 / 3) - 3),
                    _cheatString,
                    cheatTextStyle);
                GUI.FocusControl("CheatInput");

                if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return)
                {
                    string[] command = ParseInput(_cheatString);
                    if (command[0] == string.Empty || PerformCommand(command))
                    {
                        // Nothing entered or command executed successfully,
                        // simply close the console.
                        TriggerSheet();
                    }
                    else
                    {
                        // Command not found!
                        _cheatString = string.Empty;
                        _cheatStateString = "Command \"" + command[0] + "\" unknown! Try again:";
                    }
                }
            }
        }

        private string[] ParseInput(string input)
        {
            input = input.ToLowerInvariant();
            string[] erg = input.Split(null);
            return erg;
        }

        private bool PerformCommand(string[] command)
        {
            if (command.Length == 0)
            {
                Debug.LogError("CheatBox::PerformCommand() called with zero-length argument vector!");
                return false;
            }

            CheatCommand cmd;
            if (!_cheatCommands.TryGetValue(command[0], out cmd))
            {
                // No such command.
                Debug.Log("[CheatBox.cs] No such command: " + command[0]);
                return false;
            }

            // Call the command.
            Debug.Log("[CheatBox.cs] Executing command: " + command[0]);
            cmd(command);

            return true;
        }

        private void EndGame(string[] command)
        {
            Application.LoadLevel("StatisticScreen");
        }

        private void Stuck(string[] command)
        {
            List<Hero> herolist = GameManager.GetInstance().GetAllHeroes();
            for (int i = 0; i < command.Length - 1; i++)
            {
                foreach (Hero hero in herolist)
                {
                    if (hero.DisplayName.ToLowerInvariant().Equals(command[1 + i]))
                    {
                        Vector3 size = GameManager.GetInstance().CurrentSection.TerrainSize;
                        Vector3 position = GameManager.GetInstance().CurrentSection.transform.position;

                        float xP = (hero.transform.position.x + 5 < (size.x / 2) + position.x) ? 5 : 0;
                        float xN = (hero.transform.position.x - 5 > (size.x / 2) - position.x) ? -5 : 0;

                        float zP = (hero.transform.position.z + 5 < (size.z / 2) + position.z) ? 5 : 0;
                        float zN = (hero.transform.position.z - 5 > (size.z / 2) - position.z) ? -5 : 0;

                        hero.transform.position = new Vector3(hero.transform.position.x + Random.Range(xN, xP), hero.transform.position.y + 5, hero.transform.position.z + Random.Range(zN, zP));
                    }
                    else
                    {
                        Debug.Log("Not Found");
                    }
                }
            }
        }

        private void KillAllMobs(string[] command)
        {
            List<Mob> mobList = GameManager.GetInstance().CurrentSection.GetActiveMobs();
            foreach (Mob mob in mobList)
            {
                mob.HP = 0;
                mob.ReceiveDamage(0, Quaternion.identity, null);
            }
        }

        private void Godmode(string[] command)
        {
            switch (command.Length - 1)
            {
                case 0: // All human controlled heros into / out of  godmode
                    List<Hero> herolist = GameManager.GetInstance().GetAllHeroes();
                    foreach (Hero hero in herolist)
                    {
                        if (hero.Type == Hero.Types.HUMAN && !hero.Godmode)
                        {
                            hero.Godmode = true;
                        }
                        else
                        {
                            hero.Godmode = false;
                        }
                    }

                    break;
            }
        }

        private void Heal(string[] command)
        {
            switch (command.Length - 1)
            {
                case 0: // Heal all human controlled heros
                    List<Hero> herolist = GameManager.GetInstance().GetAllHeroes();
                    foreach (Hero hero in herolist)
                    {
                        if (hero.Type == Hero.Types.HUMAN && !hero.Godmode)
                        {
                            hero.HP = hero.MaxHP;
                        }
                    }

                    break;
            }
        }

        private void DestroyBase(string[] argv)
        {
            int teamNo = System.Convert.ToInt32(argv[1]);
            if (teamNo <= GameManager.GetInstance().GetAllTeams().Count)
            {
                GameManager.GetInstance().GetTeam(teamNo).Base.HP = 0;
                GameManager.GetInstance().GetTeam(teamNo).Base.ReceiveDamage(0, null);
            }
        }

        private void WinGameSpecial(string[] argv)
        {
            int teamNo = System.Convert.ToInt32(argv[1]);
            if (teamNo < GameManager.GetInstance().GetAllTeams().Count)
            {
                GameManager.GetInstance().GetTeam(teamNo).Points = GameManager.SPECIALWINNINGPOINTS + 1;
            }
        }

        private void Kill(string[] command)
        {
            List<Hero> herolist = GameManager.GetInstance().GetAllHeroes();
            switch (command.Length - 1)
            {
                case 0: // Kill all human controlled heros
                    foreach (Hero hero in herolist)
                    {
                        if (hero.Type == Hero.Types.HUMAN)
                        {
                            hero.HP = 0;
                            hero.ReceiveDamage(0, Quaternion.identity, null);
                        }
                    }

                    break;
                case 1:
                    int number;
                    if (int.TryParse(command[1], out number))
                    {
                        Debug.Log("Player number: " + number); // insert here the code to kill player 0,1,2,...
                    }
                    else // kill hero(s) with name given by the first parameter
                    {
                        foreach (Hero hero in herolist)
                        {
                            if (hero.DisplayName.ToLowerInvariant().Equals(command[1]))
                            {
                                hero.HP = 0;
                                hero.ReceiveDamage(0, Quaternion.identity, null);
                            }
                        }
                    }

                    break;
            }
        }
    }
}

namespace Assets.Scripts
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Class that holds general settings and information about the
    /// current game.
    /// </summary>
    public sealed class GameManager
    {
        #region "Global"

        #region "Constants"

        /// <summary>
        /// The special team all mobs belong to.
        /// </summary>
        public const int MOBTEAMNO = 666;

        /// <summary>
        /// The count of points required to win in special mode.
        /// </summary>
        public const int SPECIALWINNINGPOINTS = 1000;

        #endregion

        #region "Fields"

        #region "private"

        /// <summary>
        /// The only instance of this singleton.
        /// </summary>
        private static GameManager _instance;

        /// <summary>
        /// All current teams.
        /// </summary>
        private SortedDictionary<int, Team> _teams;

        #endregion

        #region "constructor"

        /// <summary>
        /// Prevents a default instance of the <see cref="GameManager"/> class from being created.
        /// Constructor that inits the GameManager.
        /// </summary>
        private GameManager()
        {
            Reset();
        }

        #endregion

        #region "enums"

        /// <summary>
        /// Enum with choosable options from main menu.
        /// </summary>
        public enum Mode
        {
            PLAY, MULTI, VS, SPECIAL, TEAM, SETTINGS, OTHERMULTI
        }

        #endregion

        #endregion

        #region "Properties"

        /// <summary>
        /// Gets the NetworkController whch holds the network connection logic
        /// </summary>
        public NetworkController NetworkController 
        { 
            get
            { 
                return GameObject.Find("Network").GetComponent<NetworkController>(); 
            } 
        }

        /// <summary>
        /// Gets or sets
        /// the overall awesomeness
        /// </summary>
        public float Awesomeness { get; set; }

        /// <summary>
        /// Gets or sets
        /// the scene that will be loaded.
        /// </summary>
        public string SceneToBeLoaded { get; set; }

        /// <summary>
        /// Gets or sets
        /// the next available mob ID.
        /// </summary>
        public int NextMobId { get; set; }

        /// <summary>
        /// Gets or sets
        /// the next available mercenary ID.
        /// </summary>
        public int NextMercenaryId { get; set; }

        /// <summary>
        /// Gets
        /// the losing team that caused the game to end.
        /// </summary>
        public Team LosingTeam { get; private set; }

        /// <summary>
        /// Gets
        /// the time that has passed since the level has started. (Statistics)
        /// Set when the level ends.
        /// </summary>
        public float LevelTime { get; private set; }

        /// <summary>
        /// Gets or sets
        /// mercenaries, even those not associated with a team.
        /// </summary>
        public ICollection<Mercenary> Mercenaries { get; set; }

        /// <summary>
        /// Gets or sets
        /// items in the level.
        /// </summary>
        public ICollection<Item> Items { get; set; }

        /// <summary>
        /// Gets or sets
        /// Checkpoints in the level
        /// </summary>
        public ICollection<Checkpoint> Checkpoints { get; set; }

        /// <summary>
        /// Gets or sets
        /// the current section of the current level.
        /// </summary>
        public Section CurrentSection { get; set; }

        /// <summary>
        /// Gets or sets
        /// the current level.
        /// </summary>
        public Level CurrentLevel { get; set; }

        /// <summary>
        /// Gets or sets
        /// the gamemode.
        /// </summary>
        public Mode GameMode { get; set; }

        /// <summary>
        /// Gets or sets
        /// the game difficulty.
        /// Higher integer means harder difficulty.
        /// </summary>
        public int Difficulty { get; set; }

        /// <summary>
        /// Gets or sets
        /// the number of the level to generate/load.
        /// </summary>
        public int LevelNo { get; set; }

        /// <summary>
        /// Gets or sets
        /// the number of the level to generate/load.
        /// </summary>
        public int LevelNoMultiplayer { get; set; }

        /// <summary>
        /// Gets or sets
        /// the number of the section to start.
        /// </summary>
        public int SectionNo { get; set; }

        /// <summary>
        /// Gets or sets
        /// the Number of heroes that should spawn in the entire level.
        /// </summary>
        public int HeroCount { get; set; }

        /// <summary>
        /// Gets or sets
        /// the Number of player for the mode.
        /// </summary>
        public int PlayerCount { get; set; }

        /// <summary>
        /// Gets or sets
        /// the Number of mercenaries that should spawn in the entire level.
        /// </summary>
        public int MercenaryCount { get; set; }

        /// <summary>
        /// Gets or sets
        /// the Number of mobs that should spawn in the entire level.
        /// </summary>
        public int MobCount { get; set; }

        /// <summary>
        /// Gets or sets
        /// the Number of teams, neccessary for dynamic modes.
        /// </summary>
        public int TeamCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether
        /// section 1 clearance is triggered in Play-Mode
        /// </summary>
        public bool PlayFirstTrigger { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether
        /// section 2 clearance is triggered in Play-Mode
        /// </summary>
        public bool PlaySecondTrigger { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether
        /// section 3 clearance is triggered in Play-Mode
        /// </summary>
        public bool PlayThirdTrigger { get; set; }

        #endregion

        #endregion

        #region "Methods"

        #region "public"

        /// <summary>
        /// Get an instance of this class (singleton).
        /// </summary>
        /// <returns>The only GameManager instance.</returns>
        public static GameManager GetInstance()
        {
            return _instance ?? (_instance = new GameManager());
        }

        /// <summary>
        /// Resets all Settings
        /// </summary>
        public void ResetSettings()
        {
            Awesomeness = 42;
            PlayFirstTrigger = false;
            PlaySecondTrigger = false;
            PlayThirdTrigger = false;
        }

        /// <summary>
        /// Resets the Teams
        /// </summary>
        public void ResetTeams()
        {
            _teams = new SortedDictionary<int, Team>();
            HeroCount = 0;
            PlayerCount = 1;
            MercenaryCount = 0;
            MobCount = 0;
            NextMobId = 2000;
        }

        /// <summary>
        /// Resets the Mercenaries
        /// </summary>
        public void ResetMercenaries()
        {
            Mercenaries = new List<Mercenary>();
            NextMercenaryId = 1000;
        }

        /// <summary>
        /// Resets the Itemlist
        /// </summary>
        public void ResetItems()
        {
            Items = new List<Item>();
        }

        /// <summary>
        /// Resets the level
        /// </summary>
        public void ResetLevel()
        {
            CurrentSection = null;
            CurrentLevel = null;
            SectionNo = 1;
            LevelNo = 0;
            Difficulty = 2;
            Checkpoints = new List<Checkpoint>();
            PlayFirstTrigger = false;
            PlaySecondTrigger = false;
            PlayThirdTrigger = false;
        }

        /// <summary>
        /// Resets everything
        /// </summary>
        public void Reset()
        {
            ResetSettings();
            ResetTeams();
            ResetMercenaries();
            ResetItems();
            ResetLevel();
        }

        /// <summary>
        /// Get the team with the specified team number.
        /// </summary>
        /// <param name="teamNo">Number of team to retrieve</param>
        /// <returns>The Team or NULL if it doesn't exist.</returns>
        public Team GetTeam(int teamNo)
        {
            Team team;
            _teams.TryGetValue(teamNo, out team);
            return team;
        }

        /// <summary>
        /// Add a team to the list of teams.
        /// If there already is a team with the same number, it
        /// is replaced.
        /// </summary>
        /// <param name="team">The Team to add.</param>
        /// <returns>The added team for convenience.</returns>
        public Team AddTeam(Team team)
        {
            _teams[team.TeamNo] = team;
            return team;
        }

        /// <summary>
        /// Remove a team from the list of teams.
        /// This method takes a Team and removes the Team with the same
        /// team number.
        /// </summary>
        /// <param name="team">Team to remove</param>
        /// <returns>
        ///     True if the team has been removed, false otherwise (i. e.
        ///     no such team).
        /// </returns>
        public bool RemoveTeam(Team team)
        {
            return RemoveTeam(team.TeamNo);
        }

        /// <summary>
        /// Remove a team from the list of teams.
        /// This method removes the team with the specified team number.
        /// </summary>
        /// <param name="teamNo">Number of team to remove</param>
        /// <returns>
        ///     True if the team has been removed, false otherwise (i. e.
        ///     no such team).
        /// </returns>
        public bool RemoveTeam(int teamNo)
        {
            return _teams.Remove(teamNo);
        }

        /// <summary>
        /// Get a List of teams, sorted by team number in ascending order.
        /// NB: The indices do not reflect the team numbers!
        /// </summary>
        /// <returns>All current teams</returns>
        public List<Team> GetAllTeams()
        {
            return new List<Team>(_teams.Values);
        }

        /// <summary>
        /// Get all heroes in the game, whether dead or alive.
        /// This includes AI-controlled and player-controlled heroes.
        /// </summary>
        /// <returns>An unsorted list of Heroes.</returns>
        public List<Hero> GetAllHeroes()
        {
            List<Hero> heroes = new List<Hero>();

            foreach (Team t in _teams.Values)
            {
                heroes.AddRange(t.GetHeroTeamMembers());
            }

            return heroes;
        }

        /// <summary>
        /// Clean up (destroy) all team members.
        /// This needs to be called at some point if MakeTeamMembersImmortal() has
        /// been used.
        /// This _does not_ reset the teams. After calling this method, you will probably
        /// end up with teams full of invalid object references. Reset the teams with
        /// ResetTeams().
        /// Also cleans the Bases in special mode
        /// </summary>
        public void CleanupTeamMembers()
        {
            if (GameMode == Mode.SPECIAL)
            {
                try
                {
                    Object.Destroy(GameObject.Find("Bases"));
                }
                catch (MissingReferenceException)
                {
                    // Irgnored. Can't destroy invalid objects.
                }
            }

            foreach (Team team in _teams.Values)
            {
                foreach (Character member in team.GetAllTeamMembers())
                {
                    try
                    {
                        Object.Destroy(member.gameObject);
                    }
                    catch (MissingReferenceException)
                    {
                        // Ignored. Can't destroy invalid objects.
                    }
                }
            }
        }

        /// <summary>
        /// Notify the GameManager that a team caused the game to end,
        /// losing the game.
        /// This loads the Statistic scene.
        /// </summary>
        /// <param name="team">The losing team.</param>
        public void NotifyGameOver(Team team)
        {
            if (team.TeamNo == MOBTEAMNO)
            {
                return;
            }

            Debug.Log("Team " + team.TeamNo + " LOSES! GAME OVER!");

            LevelTime = Time.timeSinceLevelLoad;
            LosingTeam = team;

            MakeTeamMembersImmortal();
            Application.LoadLevel("StatisticScreen");
        }

        /// <summary>
        /// Notify the GameManager when a Team won the special mode.
        /// </summary>
        /// <param name="team">the winning team</param>
        public void NotifySpecialWin(Team team)
        {
            Debug.Log("Team " + team.TeamNo + " WINS! GAME OVER!");
            LevelTime = Time.timeSinceLevelLoad;
            MakeTeamMembersImmortal();

            // TODO Statistic Screen: Show winning team
            if (NetworkController.IsServer || NetworkController.IsClient)
            {
                NetworkController.NetworkLoadLevel("StatisticScreen");
            }
            else
            {
                Application.LoadLevel("StatisticScreen");
            }
        }

        #endregion

        #region "private"

        /// <summary>
        /// Notify the GameManager that the game ends and that the statistics screen
        /// should be shown.
        /// </summary>
        public void NotifyGameEnd()
        {
            Debug.Log("GAME ENDS, showing stats screen");

            LevelTime = Time.timeSinceLevelLoad;

            MakeTeamMembersImmortal();
            Application.LoadLevel("StatisticScreen");
        }

        /// <summary>
        /// Prevent all members of all teams from being destroyed when a new scene is loaded.
        /// </summary>
        private void MakeTeamMembersImmortal()
        {
            if (GameMode == Mode.SPECIAL)
            {
                Object.DontDestroyOnLoad(GameObject.Find("Bases"));
            }

            foreach (Team team in _teams.Values)
            {
                foreach (Character member in team.GetAllTeamMembers())
                {
                    Object.DontDestroyOnLoad(member.gameObject);
                }
            }
        }

        #endregion

        #endregion
    }
}

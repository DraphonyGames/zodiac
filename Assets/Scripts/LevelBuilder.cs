namespace Assets.Scripts
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Can Instantiate a generated level.
    /// </summary>
    public class LevelBuilder : MonoBehaviour
    {
        /// <summary>
        /// Constant Sectionwidth
        /// </summary>
        public const int SECTIONWIDTH = 300;

        /// <summary>
        /// Gets or sets the SpawnerPrefab to be used
        /// </summary>
        public GameObject SpawnerPrefab
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or setshe GraphBukder to be used
        /// </summary>
        public GameObject GraphBuilder
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes the Spawner Prefab an the GraphBuilder
        /// </summary>
        public void Init()
        {
            SpawnerPrefab = (GameObject)Resources.Load("SpawnerPrefab");
            GraphBuilder = (GameObject)Instantiate((GameObject)Resources.Load("Pathfinding/GraphBuilderPrefab"));
        }

        /// <summary>
        /// Instantiate a generated Level
        /// </summary>
        /// <param name="level">the level number to build</param>
        public void BuildLevel(Level level)
        {
            if (GameManager.GetInstance().GameMode == GameManager.Mode.PLAY)
            {
                Debug.Log("[LevelBuilder.cs] BuildLevel");
                InitSections(level);
                SpawnHeroes(level);
                InitSpawners(level);
                StartLevel(level);

                GameObject checkpointPrefab = GameObject.Find("CheckpointPrefab");
                if (checkpointPrefab != null)
                {
                    Checkpoint c = checkpointPrefab.GetComponent<Checkpoint>();
                    c.Init();
                    GameManager.GetInstance().Checkpoints.Add(c);
                }
            }
            else
            {
                SpawnHeroes(null);
            }
        }

        private void InitSections(Level level)
        {
            for (int i = 0; i < level.Sections.Count; i++)
            {
                level.Sections[i] = SpawnSection(level.Sections[i], i * SECTIONWIDTH);
                level.Sections[i].InstantiateMobs();
                level.Sections[i].InstantiateMercs();
            }
        }

        private Section SpawnSection(Section section, int x)
        {
            List<Mob> mobs = section.GetMobs();
            List<Mercenary> mercs = section.GetMercs();
            int sectionNo = section.SectionNumber;
            Section instantiated = ((GameObject)Instantiate(section.SectionPrefab, new Vector3(x, 0, 0), Quaternion.identity)).GetComponent<Section>();
            instantiated.Init();
            foreach (Mob m in mobs)
            {
                instantiated.AddMob(m);
            }

            foreach (Mercenary m in mercs)
            {
                instantiated.AddMercenary(m);
            }

            instantiated.SectionNumber = sectionNo;
            return instantiated;
        }

        private void InitSpawners(Level level)
        {
            for (int i = 0; i < level.Sections.Count; i++)
            {
                level.Sections[i].MobSpawner = ((GameObject)Instantiate(SpawnerPrefab, level.Sections[i].transform.position, Quaternion.identity)).GetComponent<Spawner>();
                level.Sections[i].MobSpawner.OwnerSection = level.Sections[i];
                level.Sections[i].MobSpawner.SpawnerType = Spawner.Type.MOB;
                level.Sections[i].InitMobSpawner();
                level.Sections[i].MobSpawner.WaveSize = level.Sections[i].MobSpawner.Pool.Count / 4;

                level.Sections[i].MercSpawner = ((GameObject)Instantiate(SpawnerPrefab, level.Sections[i].transform.position, Quaternion.identity)).GetComponent<Spawner>();
                level.Sections[i].MercSpawner.OwnerSection = level.Sections[i];
                level.Sections[i].MercSpawner.SpawnerType = Spawner.Type.MERC;
                level.Sections[i].InitMercSpawner();
            }
        }

        private void SpawnHeroes(Level level)
        {
            int playerCounter = 1;
            List<Team> teamList = GameManager.GetInstance().GetAllTeams();
            int startSection = GameManager.GetInstance().SectionNo - 1;
            for (int i = 0; i < teamList.Count; i++)
            {
                if (teamList[i].TeamNo == 666)
                {
                    continue;
                }

                // Team newTeam = new Team(i+1);
                List<Hero> heroList = teamList[i].GetHeroTeamMembers();

                for (int j = 0; j < heroList.Count; j++)
                {
                    Hero h = heroList[j];
                    h = ((GameObject)Instantiate(h.gameObject)).GetComponent<Hero>();
                    h.PlayerNo = playerCounter;
                    playerCounter++;
                    h.Type = heroList[j].Type;
                    h.DisplayName = heroList[j].DisplayName;
                    h.name = "Hero" + h.PlayerNo;

                    if (level.Sections[startSection] != null)
                    {
                        // spawn on generated level
                        Debug.Log("Spawn: " + h.name);
                        level.Sections[startSection].RespawnCharacter(h);
                        Debug.Log("Hero is spawned in Section " + startSection + 1);
                    }
                    else
                    {
                        // spawn on fix points like on multiplayermaps
                        if (transform.parent.transform.Find("Spawn Player" + (playerCounter - 1)) == null)
                        {
                            Debug.Log("no spawningpoint found for player " + (playerCounter - 1));
                        }
                        else
                        {
                            h.transform.position = transform.parent.transform.Find("Spawn Player" + ((playerCounter - 1) + (4 * i))).transform.position;
                        }
                    }

                    teamList[i].RemoveMember(heroList[j]);
                    teamList[i].AddMember(h);
                }
            }

            if (level != null)
            {
                for (int k = 0; k < GameManager.GetInstance().HeroCount; k++)
                {
                    Debug.Log("[CharacterSelection.cs] Add AI hero");
                    Hero choosenCharacter = ((GameObject)Instantiate(Datasheet.Heroes()[Random.Range(0, 2)])).GetComponent<Hero>();
                    choosenCharacter.name = "Hero" + playerCounter + "KI";
                    playerCounter++;
                    choosenCharacter.Type = Hero.Types.AI;

                    Debug.Log("Spawn AI: " + choosenCharacter.name);
                    level.Sections[startSection].RespawnCharacter(choosenCharacter);

                    if (GameManager.GetInstance().TeamCount == 1)
                    {
                        teamList[0].AddMember(choosenCharacter);
                    }
                }
            }
        }

        private void StartLevel(Level level)
        {
            int startSection = GameManager.GetInstance().SectionNo - 1;
            GameManager.GetInstance().CurrentSection = level.Sections[startSection];
            GameManager.GetInstance().CurrentSection.MobSpawner.StartSpawnRoutine(GameManager.GetInstance().CurrentSection.MobSpawner.WaveSize * 3);
            GameManager.GetInstance().CurrentSection.MercSpawner.StartSpawnRoutine();
            GameManager.GetInstance().CurrentLevel = level;

            // Scan map for AI
            GraphBuilder.GetComponent<AIHelper.GraphBuilder>().TryMapScanning();
        }
    }
}

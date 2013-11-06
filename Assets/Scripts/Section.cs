namespace Assets.Scripts
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// Represents a Section that's part of a Level
    /// </summary>
    public class Section : MonoBehaviour
    {
        /// <summary>
        /// The MobSpawner
        /// </summary>
        public Spawner MobSpawner;

        /// <summary>
        /// The MercenarySpawner
        /// </summary>
        public Spawner MercSpawner;

        /// <summary>
        /// The GameObject that represents the section.
        /// </summary>
        public GameObject SectionPrefab;

        /// <summary>
        /// Contains all Mobs that will be spawned.
        /// </summary>
        private List<AbstractSpawnable> _mobPool;

        /// <summary>
        /// Contains all Items that will be spawned.
        /// </summary>
        private List<AbstractSpawnable> _itemPool;

        /// <summary>
        /// size of the section Terrain
        /// </summary>
        private Vector3 _terrainSize;

        private Terrain _terrain;

        /// <summary>
        /// Contains all Mercenaries that will be spawned.
        /// </summary>
        private List<AbstractSpawnable> _mercPool;

        /// <summary>
        /// Contains the count of the remaining mobs alive.
        /// </summary>
        private int _remainingMobs;

        /// <summary>
        /// Initializes a new instance of the <see cref="Section"/> class.
        /// </summary>
        /// <param name="sectionPrefab">The Section-Prefab</param>
        public Section(GameObject sectionPrefab)
        {
            SectionPrefab = sectionPrefab;
            Init();
        }

        /// <summary>
        /// Gets or sets the number of the Section ordered in the Level
        /// </summary>
        public int SectionNumber { get; set; }

        /// <summary>
        /// Gets the section terrain size
        /// </summary>
        public Vector3 TerrainSize
        {
            get
            {
                return _terrainSize;
            }
        }

        /// <summary>
        /// Inits the Section
        /// </summary>
        public void Init()
        {
            _mobPool = new List<AbstractSpawnable>();
            _mercPool = new List<AbstractSpawnable>();
            _itemPool = new List<AbstractSpawnable>();
            _remainingMobs = 0;

            Start();
        }

        /// <summary>
        /// Called when Instantiated (Gets the Terrain Size)
        /// </summary>
        public void Start()
        {
            try
            {
                if (_terrain == null)
                {
                    _terrain = transform.FindChild("Ground").GetComponent<Terrain>();
                }

                Debug.Log("[Section.cs]: not sure if got terrain :/");
                if (_terrain == null)
                {
                    Debug.Log("[Section.cs]: got not terrain really :/");
                }
                else
                {
                    _terrainSize = _terrain.terrainData.size;
                }
            }
            catch
            {
                Debug.Log("[Section.cs]: got not terrain with error :/");
            }
        }

        /// <summary>
        /// Adds new Mob to EnemyPool.
        /// </summary>
        /// <param name="mob">The new mob</param>
        public void AddMob(Mob mob)
        {
            _mobPool.Add(mob);
            _remainingMobs++;
        }

        /// <summary>
        /// Instantiates all Mobs for the Spawner
        /// </summary>
        public void InstantiateMobs()
        {
            Team mobTeam = new Team(GameManager.MOBTEAMNO);
            for (int i = 0; i < _mobPool.Count; i++)
            {
                // Create a new mob.
                Mob m = ((GameObject)Instantiate(_mobPool[i].gameObject, new Vector3(transform.position.x, transform.position.y + 110, transform.position.z), Quaternion.identity)).GetComponent<Mob>();
               
                // Add the mob to the pool.
                _mobPool[i] = m;
                Debug.Log("[Section.cs]: Mob x-value: " + m.transform.position.x);
                
                // Add the inactive mob to the mob team.
                m.gameObject.SetActive(false);
                m.Type = Character.Types.AI;
                mobTeam.AddMember(m);
            }

            GameManager.GetInstance().AddTeam(mobTeam);
        }

        /// <summary>
        /// Instantiates all Mercenaries for the Spawner
        /// </summary>
        public void InstantiateMercs()
        {
            for (int i = 0; i < _mercPool.Count; i++)
            {
                _mercPool[i] = ((GameObject)Instantiate(_mercPool[i].gameObject, new Vector3(transform.position.x, transform.position.y + 110, transform.position.z), Quaternion.identity)).GetComponent<Mercenary>();
                _mercPool[i].gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Gets all Mercenaries (free and caged)
        /// </summary>
        /// <returns>All Mercenaries (free and caged)</returns>
        public List<Mercenary> GetMercs()
        {
            return _mercPool.Cast<Mercenary>().ToList();
        }

        /// <summary>
        /// Returns all free Mercenaries
        /// </summary>
        /// <returns>All free Mercenaries</returns>
        public List<Mercenary> GetFreeMercs()
        {
            return _mercPool.Cast<Mercenary>().Where(merc => merc.Team != null).ToList();
        }

        /// <summary>
        /// Returns all caged Mercenaries
        /// </summary>
        /// <returns>All caged Mercenaries</returns>
        public List<Mercenary> GetCagedMercs()
        {
            return _mercPool.Cast<Mercenary>().Where(merc => merc.Team == null).ToList();
        }

        /// <summary>
        /// Returns all Mobs (spawned and not spawned)
        /// </summary>
        /// <returns>All Mobs (spawned and not spawned)</returns>
        public List<Mob> GetMobs()
        {
            return _mobPool.Cast<Mob>().ToList();
        }

        /// <summary>
        /// Returns all spawned Mobs
        /// </summary>
        /// <returns>All spawned Mobs</returns>
        public List<Mob> GetActiveMobs()
        {
            return _mobPool.Cast<Mob>().Where(mob => mob.enabled).ToList();
        }

        /// <summary>
        /// Returns all spawned Mobs and free Mercenaries
        /// </summary>
        /// <returns>All spawned Mobs and free Mercenaries</returns>
        public List<AbstractSpawnable> GetActiveMobsAndMercs()
        {
            List<Mob> mobs = GetActiveMobs();
            List<Mercenary> mercs = GetFreeMercs();
            List<AbstractSpawnable> output = mobs.Cast<AbstractSpawnable>().ToList();
            output.AddRange(mercs.Cast<AbstractSpawnable>());
            return output;
        }

        /// <summary>
        /// Adds Mercenary to Mercenary-Spawnpool
        /// </summary>
        /// <param name="merc">The new Mercenary</param>
        public void AddMercenary(Mercenary merc)
        {
            _mercPool.Add(merc);
        }

        /// <summary>
        /// Returns a list of all items in this section.
        /// </summary>
        /// <returns>List of BasicItems</returns>
        public List<Item> GetItems()
        {
            return _itemPool.Cast<Item>().ToList();
        }

        /// <summary>
        /// Add a item to this sections itemlist
        /// </summary>
        /// <param name="item">The new item</param>
        public void AddItem(Item item)
        {
            _itemPool.Add(item);
        }

        /// <summary>
        /// Remove an item in this sections itemlist.
        /// </summary>
        /// <param name="item">item to remove</param>
        /// <returns>returns if item found in this sections itemlist</returns>
        public bool RemoveItem(Item item)
        {
            return _itemPool.Remove(item);
        }

        /// <summary>
        /// Initialise the mobspawner of this section.
        /// </summary>
        public void InitMobSpawner()
        {
            MobSpawner.InitMobSpawner(_mobPool);
            MobSpawner.SpawnRate = 3;
        }

        /// <summary>
        /// Initialise the mercenaryspawner of this section.
        /// </summary>
        public void InitMercSpawner()
        {
            MercSpawner.Pool = _mercPool;
            MercSpawner.SpawnRate = 1;
        }

        /// <summary>
        /// Reduces the remaining Mob Count, checks if the Section is cleared.
        /// </summary>
        /// <param name="mob">The mob that was killed</param>
        public void KillMob(Mob mob)
        {
            Debug.Log("[Section.cs] Mob Kill requested");
            if (!_mobPool.Contains(mob))
            {
                return;
            }

            Debug.Log("[Section.cs] Mob Kill granted");
            _remainingMobs--;
            _mobPool.Remove(mob);
            if (_remainingMobs == 0)
            {
                OpenSectionExit();
                for (int i = 0; i < GameManager.GetInstance().CurrentLevel.Sections.Count; i++)
                {
                    if (GameManager.GetInstance().CurrentLevel.Sections[i] == this)
                    {
                        if (i + 1 < GameManager.GetInstance().CurrentLevel.Sections.Count)
                        {
                            GameManager.GetInstance().CurrentLevel.Sections[i + 1].OpenSectionEntrance();
                        }

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Open the entrance to the section.
        /// </summary>
        public void OpenSectionEntrance()
        {
            transform.FindChild("Entrance").Translate(Vector3.up * 10);
        }

        /// <summary>
        /// Open the entrance to the next section/ the exit of this section.
        /// </summary>
        public void OpenSectionExit()
        {
            transform.FindChild("Exit").Translate(Vector3.up * 10);
        }

        /// <summary>
        /// Amount of Mobs remaining in this section.
        /// </summary>
        /// <returns>Amount of mobs</returns>
        public int RemainingMobs()
        {
            return _remainingMobs;
        }

        /// <summary>
        /// Sets indices to the Waypoints
        /// </summary>
        public void SortWaypoints()
        {
            int i = 0;
            foreach (Map.Waypoint waypoint in transform.GetComponentsInChildren<Map.Waypoint>())
            {
                waypoint.Sort = i++;
            }
        }

        /// <summary>
        /// Respawns a Character at the beginning of this section.
        /// </summary>
        /// <param name="character">The Character to be respawned</param>
        public void RespawnCharacter(Character character)
        {
            float x = Random.Range(-2.5f, 2.5f);
            float z = Random.Range(-2.5f, 2.5f);

            Debug.Log("[Section.cs] " + character.name + " position: " + character.transform.position.x + " " + character.transform.position.y + " " + character.transform.position.z + " ");
            Debug.Log("[Section.cs] terrain position: " + _terrain.transform.position.y + " " + _terrain.transform.position.z + " " + _terrain.transform.position.z + " ");

            Vector3 pos = new Vector3(transform.position.x - (LevelBuilder.SECTIONWIDTH / 2) + (0.1f * LevelBuilder.SECTIONWIDTH) + x, transform.position.y + 5, transform.position.z + z);

            character.Spawn(pos);
        }
    }
}
namespace Assets.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    /// <summary>
    /// Spawner class which could spawn mercs, mobs and items.
    /// </summary>
    public class Spawner : MonoBehaviour
    {
        #region "global properties"
        #region "Fields"
        #region "public"
        /// <summary>
        /// The Type of the Spawner
        /// </summary>
        public Type SpawnerType;

        /// <summary>
        /// the current Countdown for the next Mob Wave
        /// </summary>
        public int Countdown;

        /// <summary>
        /// Size of one Enemy-Wave (only Mob-Spawner)
        /// </summary>
        public int WaveSize;

        /// <summary>
        /// Section the spawner is in.
        /// </summary>
        public Section OwnerSection;
        #endregion

        /// <summary>
        /// unit pool iterator
        /// </summary>
        protected int CurrentUnit = 0;

        #region "private"
        /// <summary>
        /// The Main-Hero Object the spawner is attached to
        /// </summary>
        private Hero _hero;

        /// <summary>
        /// Bool that shows if the Spawner is activated
        /// </summary>
        private bool _activated = false;

        /// <summary>
        /// Total amount of units that will be spawned
        /// </summary>
        private int _totalUnits = 0;

        /// <summary>
        /// Instance of the Game-Manager
        /// </summary>
        private GameManager _gameManager;
        #endregion
        #endregion

        /// <summary>
        /// Enum that describes the Spawnertype
        /// </summary>
        public enum Type
        {
            MOB, ITEM, MERC
        }

        /// <summary>
        /// Gets or sets the pool of BasicSpawnables which going to be spawend.
        /// </summary>
        public List<AbstractSpawnable> Pool { get; set; } // unit pool

        /// <summary>
        /// Gets or sets the rate after how many sec a BasicSpawnable spawns.
        /// </summary>
        public float SpawnRate { get; set; } // Time between spawning units/mobs
        #endregion

        #region "Methods"
        #region "unity triggered"
        /// <summary>
        /// Called when Instantiated
        /// </summary>
        public void Start()
        {
            _gameManager = GameManager.GetInstance();
        }

        /// <summary>
        /// Ceeps track of the heros position for moving the Mob-Spawner to the right lokation where the mobs should be spawned at.
        /// </summary>
        public void Update()
        {
            switch (SpawnerType)
            {
                case Type.MOB:
                    if (GameManager.GetInstance().CurrentSection == OwnerSection)
                    {
                        float x = _hero.transform.position.x + 40;
                        float length = _gameManager.CurrentSection.TerrainSize.x;
                        if (x > (OwnerSection.transform.position.x + (length / 2)))
                        {
                            x = _hero.transform.position.x - 40;
                        }

                        float y = _hero.transform.position.y + 10;
                        float z = this.OwnerSection.transform.position.z;

                        // (_gameManager.SectionNo*300)/2
                        transform.position = new Vector3(0 + ((_gameManager.SectionNo - 1) * _gameManager.CurrentSection.TerrainSize.x), y, z);
                    }

                    break;
                case Type.MERC:
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region "public"
        /// <summary>
        /// Inits the Spawner for being a Mobspawner 
        /// </summary>
        /// <param name="pool">list of mobs that will be spawned</param>
        public void InitMobSpawner(List<AbstractSpawnable> pool)
        {
            Pool = new List<AbstractSpawnable>(pool);
            _totalUnits = Pool.Count;
            CurrentUnit = 0;
            _hero = GameManager.GetInstance().GetAllTeams()[0].GetHeroTeamMembers()[0];
        }

        /// <summary>
        /// This method starts the spawn routine without a specific rate or an rate which got set before.
        /// </summary>
        public void StartSpawnRoutine()
        {
            _activated = true;
            StartCoroutine(Spawn()); // start spawning routine
        }

        /// <summary>
        /// This methode starts the spawn routine with an specific rate.
        /// </summary>
        /// <param name="spawnRate">Seconds to wait until the next BacisSpawnable should be spawned</param>
        public void StartSpawnRoutine(float spawnRate)
        {
            SpawnRate = spawnRate;
            StartSpawnRoutine();
        }

        /// <summary>
        /// Returns this instance.
        /// </summary>
        /// <returns>this Spawner</returns>
        public Spawner GetSpawner()
        {
            return this;
        }

        /// <summary>
        /// This function spawns spawnables of the spawnertype.
        /// </summary>
        /// <returns>waiting time until this routine starts again</returns>
        public IEnumerator Spawn()
        {
            switch (SpawnerType)
            {
                case Type.MOB:
                    while (CurrentUnit < _totalUnits && _activated)
                    {
                        while (((Mob)Pool[CurrentUnit]).IsDead() && CurrentUnit + 1 < Pool.Count)
                        {
                            CurrentUnit++;
                        }

                        int remaining = _totalUnits - CurrentUnit;
                        for (int i = 0; i < Math.Min(WaveSize, remaining); i++)
                        {
                            Mob currentMob = (Mob)Pool[CurrentUnit];

                            // Spawn them on a random position around the spawner.
                            currentMob.Spawn(NewPosition());

                            // Give the mob a new ID
                            currentMob.PlayerNo = GameManager.GetInstance().NextMobId++;

                            // Activate it
                            currentMob.gameObject.SetActive(true);

                            CurrentUnit++;

                            // Waiting for next spawning
                        }

                        // original version
                        // yield return new WaitForSeconds(SpawnRate);
                        // -----------------
                        for (Countdown = (int)SpawnRate; Countdown >= 0; Countdown--)
                        {
                            yield return new WaitForSeconds(1);
                        }
                    }

                    break;
                case Type.MERC:
                    while (CurrentUnit < Pool.Count)
                    {
                        yield return new WaitForSeconds(SpawnRate);

                        Mercenary currentMerc = (Mercenary)Pool[CurrentUnit];
                        List<MercenaryCage> mercList = new List<MercenaryCage>();
                        Vector3 spawnPosition = new Vector3();
                        bool loopBool = true;
                        while (loopBool)
                        {
                            loopBool = false;
                            mercList.Clear();
                            spawnPosition = NewPosition();
                            Collider[] hitColliders = Physics.OverlapSphere(new Vector3(spawnPosition.x, 0, spawnPosition.z), 7);
                            foreach (Collider hitCollider in hitColliders)
                            {
                                MercenaryCage mercCase = hitCollider.GetComponent<MercenaryCage>();
                                if (mercCase != null)
                                {
                                    loopBool = true;
                                    Debug.Log("Recalculating");
                                }

                                if (hitCollider.GetComponent<Character>() != null)
                                {
                                    loopBool = true;
                                    Debug.Log("Recalculating");
                                }
                            }
                        }

                        // Spawn them on a random position around the spawner.
                        currentMerc.Spawn(spawnPosition);

                        // Give it a new ID.
                        currentMerc.PlayerNo = GameManager.GetInstance().NextMercenaryId++;

                        // Activate them.
                        currentMerc.gameObject.SetActive(true);

                        CurrentUnit++;

                        // Waiting for next spawning
                    }

                    break;
                case Type.ITEM:
                    while (_activated && Pool.Count > 0)
                    {
                        yield return new WaitForSeconds(SpawnRate);
                        if (_gameManager.CurrentSection != null)
                        {
                            Item item = (Item)Pool[UnityEngine.Random.Range(0, Pool.Count)].Clone();
                            _gameManager.CurrentSection.AddItem(item);
                            item.Spawn(ItemSpawnPoint());
                        }
                    }

                    break;
            }
        }

        /// <summary>
        /// Deactivate the Spawner .. currently only the item/Mob spawner will be affected.
        /// </summary>
        public void Deactivate()
        {
            _activated = false;
        }

        /// <summary>
        /// returns if the spawner is active or inactive.
        /// </summary>
        /// <returns>active(= true) / inactive(= false)</returns>
        public bool IsActive()
        {
            return _activated;
        }
        #endregion

        #region "private"
        /// <summary>
        /// This methode returns a random position around the spawner.
        /// within x/-x: 120 y: 0 z/-z: 5
        /// </summary>
        /// <returns> the new position (as Vector3)</returns>
        private Vector3 NewPosition()
        {
            float x = UnityEngine.Random.Range(-120, 120);
            float z = UnityEngine.Random.Range(-5, 5);
            return new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
        }

        /// <summary>
        /// This method returns a specific point at the current section in front(play direction (storymode = right)) of the hero.
        /// </summary>
        /// <returns>Specific point to spawn the item at</returns>
        private Vector3 ItemSpawnPoint()
        {
            Section curSec = _gameManager.CurrentSection;

            // Section position
            float x = curSec.transform.position.x;
            float y = 10f; // spawn at hight of 10f
            float z = curSec.transform.position.z;

            // Section scale
            float xScale = curSec.TerrainSize.x / 2;  // float yScale = curSec.TerrainSize.y; (not used)
            float zScale = curSec.TerrainSize.z / 2;

            // beginning-/end-points (of area to spawn these items)
            float secEndX = x + xScale - 1;
            float secEndZ = z + zScale - 1;
            float secBeginZ = z - zScale + 1;

            return new Vector3(UnityEngine.Random.Range(gameObject.transform.position.x, secEndX), y, UnityEngine.Random.Range(secBeginZ, secEndZ));
        }
        #endregion
        #endregion
    }
}

namespace Assets.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    /// <summary>
    /// Basic class for heroes
    /// </summary>
    public abstract class Hero : Character
    {
        /// <summary>
        /// the item spawner spawns items around the hero
        /// </summary>
        private Spawner _itemSpawner;

        /// <summary>
        /// Gets or sets the projectile
        /// </summary>
        public GameObject Projectile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the hero is doing fork damage
        /// </summary>
        public bool DoForkDamage { get; set; }

        /// <summary>
        /// Gets or sets the radius of Area damage
        /// </summary>
        public float ForkArcRadius { get; set; }

        /// <summary>
        /// Gets or sets the currents time to respawn in seconds
        /// </summary>
        public int RespawnCountdown { get; set; }

        /// <summary>
        /// called on awake
        /// </summary>
        public override void Awake()
        {
            RespawnCountdown = 0;
            base.Awake();
        }

        /// <summary>
        /// called once an start
        /// </summary>
        public override void Start()
        {
            base.Start();

            // setting the itemspawner of this hero
            // TODO: one setting still left... spawn right or left of this hero?? --atm always right
            GameObject prefab = (GameObject)Resources.Load("SpawnerPrefab");
            _itemSpawner = ((GameObject)Instantiate(prefab, transform.position, Quaternion.identity)).GetComponent<Spawner>();
            _itemSpawner.Pool = Datasheet.Items();
            _itemSpawner.SpawnerType = Spawner.Type.ITEM;
            _itemSpawner.transform.parent = transform;

            if (GameManager.GetInstance().GameMode == GameManager.Mode.PLAY)
            {
                _itemSpawner.StartSpawnRoutine(UnityEngine.Random.Range(20 + (GameManager.GetInstance().Difficulty * 5), 40 + (GameManager.GetInstance().Difficulty * 5)));
            }
            else
            {
                // between 30s and 1min
                _itemSpawner.StartSpawnRoutine(UnityEngine.Random.Range(30, 60));
            }
        }

        /// <summary>
        /// Check if an object equals to this one.
        /// </summary>
        /// <param name="obj">Object to compare with.</param>
        /// <returns>True if both objects are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            Hero other = obj as Hero;

            if (other == null)
            {   // It's not a team -- no equality.
                return false;
            }

            return Equals(other);
        }

        /// <summary>
        /// Check if another Hero equals to this one.
        /// Two heroes are equal if they have the same player number.
        /// </summary>
        /// <param name="other">Team to compare with.</param>
        /// <returns>True if both teams are equal, false otherwise.</returns>
        public bool Equals(Hero other)
        {
            return PlayerNo == other.PlayerNo;
        }

        /// <summary>
        /// Warning fix
        /// </summary>
        /// <returns>the hash code</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Let the character die
        /// if in special mode starting respawn timer
        /// </summary>
        /// <param name="killer">The character that killed this entity (optional).</param>
        public override void Die(Character killer)
        {
            base.Die(killer);

            if (GameManager.GetInstance().GameMode == GameManager.Mode.SPECIAL)
            {
                StartCoroutine(SpecialRespawnTimer(5));
            }
        }

        /// <summary>
        /// Respawns a Hero in his Base (Special Mode)
        /// </summary>
        public void SpecialRespawn()
        {
            Instantiate(DieSmokePrefab, transform.position, DieSmokePrefab.transform.rotation);
            Vector3 pos = GameObject.Find("Base" + this.TeamHidden.TeamNo).transform.position;
            this.Spawn(pos);
            CanMove = true;
        }

        /// <summary>
        /// Respawn Offset
        /// </summary>
        /// <param name="time">the time offset</param>
        /// <returns>waiting time for coroutine in seconds</returns>
        private IEnumerator SpecialRespawnTimer(float time)
        {
            for (RespawnCountdown = (int)time; RespawnCountdown > 0; RespawnCountdown--)
            {
                yield return new WaitForSeconds(1);
            }

            RespawnCountdown = 0;
            SpecialRespawn();
        }
    }
}
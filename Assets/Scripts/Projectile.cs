namespace Assets.Scripts
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Projectile class which holds all nessecary information for a character which could be hit.
    /// i.e. Damage done by this projectile, character which send this projectile, ...
    /// </summary>
    public class Projectile : MonoBehaviour
    { // TODO: Vererbung.....
        #region "global properties"
        #region "Fields"
        #region "public"
        /// <summary>
        /// Hitcounter to prevent early destruction
        /// </summary>
        public int HitCounter;

        /// <summary>
        /// direction of the Knockback
        /// If you don't want knockback use Vector3.zero
        /// </summary>
        public Vector3 Knockback;

        /// <summary>
        /// Gets or sets the flying direction of the projectile. 
        /// </summary>
        public Vector3 ProjectileDirection;

        /// <summary>
        /// time to life for stationary projectiles
        /// </summary>
        public float TimeToLive;

        /// <summary>
        /// Prefab to Instantiate
        /// </summary>
        public GameObject ActivePrefab;

        /// <summary>
        /// Size of Prefab
        /// </summary>
        public Vector3 PrefabSize;

        /// <summary>
        /// Size of AoEPrefab 
        /// </summary>
        public Vector3 AoEPrefabSize;

        /// <summary>
        /// Rotation of AoEPrefab 
        /// </summary>
        public Vector3 AoEPrefabRotation;

        /// <summary>
        /// Damage of AoE Projectile 
        /// </summary>
        public int AoEDamageValue;

        /// <summary>
        /// rotation of Prefab
        /// </summary>
        public Vector3 PrefabRotation;

        /// <summary>
        /// AoEPrefab to Instantiate
        /// </summary>
        public GameObject AoEPrefab;

        /// <summary>
        /// ForkPrefab to Instantiate
        /// </summary>
        public GameObject ForkPrefab;

        /// <summary>
        /// enemy that triggered fork effect
        /// </summary>
        public AbstractSpawnable ForkTriggerEnemy;

        /// <summary>
        /// indicates if the projectile is effected by gravity
        /// </summary>
        public bool HasGravity;

        /// <summary>
        /// whether the projectile is attached to the player
        /// </summary>
        public bool Attached;
        #endregion

        #region "protected"
        /// <summary>
        /// time the object is instantiated
        /// </summary>
        protected float SpawnTime;

        /// <summary>
        /// variable to store the amout moved already 
        /// </summary>
        protected float Moved;
        #endregion

        #region "private"
        /// <summary>
        /// NetworkController which holds the connection logic and information
        /// </summary>
        private NetworkController _net;

        /// <summary>
        /// stores the enemytrigger of the main projectile
        /// </summary>
        private AbstractSpawnable _forkTriggerEnemy;
        #endregion
        #endregion

        #region "Properties"
        #region "public"
        /// <summary>
        /// Gets or sets the range of the projectile.
        /// </summary>
        public float Range { get; set; }

        /// <summary>
        /// Gets or sets the speed of the projectile.
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// Gets or sets the damage of the projectile.
        /// If Area of Effekt damage is enabled, it is the damage of the triggered projectiles.
        /// </summary>
        public int Damage { get; set; }

        /// <summary>
        /// Gets or sets the direction of the player is facing.
        /// </summary>
        public Vector3 PlayerDirection { get; set; }

        /// <summary>
        /// Gets or sets the reference to the obj. of the owner.
        /// </summary>
        public GameObject PlayerObj { get; set; }

        /// <summary>
        /// Gets or sets the offset of the owners position the projectile is flying from
        /// </summary>
        public Vector3 StartPosition { get; set; }

        /// <summary>
        /// Gets or sets attack startup
        /// </summary>
        public float StartUp { get; set; }

        /// <summary>
        /// Gets or sets the stunning time in seconds
        /// 0 means disabled
        /// </summary>
        public float Hitstun { get; set; }

        /// <summary>
        /// Gets or sets the projectile owner
        /// </summary>
        public Character Player { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the projectile is piercing through enemies.
        /// </summary>
        public bool DoPiercing { get; set; }

        /// <summary>
        /// Gets or sets the loss of damage strength of this projectile (piercing)
        /// </summary>
        public float DamageLoss { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Area of Effect Damage is enabled.
        /// </summary>
        public bool AoEDamage { get; set; }

        /// <summary>
        /// Gets or sets the time of AoE
        /// </summary>
        public float AoETime { get; set; }

        /// <summary>
        /// Gets or sets the radius of AoE.
        /// </summary>
        public Vector3 AoESize { get; set; }

        /// <summary>
        /// Gets or sets Area of Effect Damage enabled any vert. knockback (and how much)
        /// </summary>
        public Vector3 AoEKnockbackVert { get; set; }

        /// <summary>
        /// Gets or sets Area of Effect Damage enabled any horizontal knockback (and how much)
        /// </summary>
        public float AoEKnockbackHor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Fork Damage is enabled.
        /// </summary>
        public bool ForkDamage { get; set; }

        /// <summary>
        /// Gets or sets the radius of Fork Damage.
        /// </summary>
        public float ForkArcRadius { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether multi fork damage in an row is enabled.
        /// </summary>
        public bool DoMultiForkDamage { get; set; }

        /// <summary>
        /// Gets or sets a delay after the main projectile starts.
        /// </summary>
        public float ForkMultiDamageDelay { get; set; }

        /// <summary>
        /// Gets or sets the trigger rate for multi Fork damage.
        /// </summary>
        public float ForkMultiDamageRate { get; set; }
        #endregion

        #region "protected"
        /// <summary>
        /// Gets or sets a value indicating whether projectiles has to be disabled for the projectilepool or has to be destroyed.
        /// </summary>
        protected bool Inactive { get; set; }

        /// <summary>
        /// Gets or sets a variable to store Fork Damage for the main projectile.
        /// </summary>
        protected int ForkDamageValue { get; set; }
        #endregion
        #endregion
        #endregion

        #region "Methods"
        #region "unity triggered"
        /// <summary>
        /// called by unity on the first game loop
        /// </summary>
        public void Start()
        {
            // default values for projectile
            Reset();
            SpawnTime = Time.time;
        }

        /// <summary>
        /// Unity Callback event on initialising or scene changes
        /// </summary>
        public void Awake()
        {
            _net = GameObject.Find("Network").GetComponent<NetworkController>();
        }

        /// <summary>
        /// called on each frame
        /// do things like moving the projectile
        /// </summary>
        public void Update()
        {
            if (!_net.IsClient)
            {
                // don't do anything if inactive
                if (!Inactive)
                {
                    // if Range is reached trigger
                    if (Range <= Moved)
                    {
                        if (ForkDamage)
                        {
                            DoForkDamage();
                        }
                        else if (AoEDamage)
                        {
                            DoAoEDamage();
                        }

                        ProjectilePool.RemoveProjectile(this);
                    }

                    if ((!DoPiercing && HitCounter > 0) || (Time.time - SpawnTime > TimeToLive && Speed <= 0.0f))
                    { // Destroy on impact
                        if (AoEDamage)
                        {
                            DoAoEDamage();
                        }

                        ProjectilePool.RemoveProjectile(this);
                    }

                    Move();
                }
            }
        }

        /// <summary>
        /// Listen for collisions
        /// </summary>
        /// <param name="other">the object we hit</param>
        public void OnTriggerEnter(Collider other)
        {
            if (!_net.IsClient)
            {
                // TODO: There needs to be a better way of doing this.
                //       Trying all possible classes is really ugly --
                //       there's this great concept called "polymorphism"...
                var s = other.GetComponent<Character>();

                if (s == null)
                {   // Not a Character.
                    return;
                }

                if (Player == null || Player == s)
                {
                    return;
                }

                if (s.Team != null && Player.Team != null && s.Team.Equals(Player.Team))
                {
                    // Friendly fire.
                    return;
                }

                if (s is Mercenary && (s as Mercenary).Cage != null)
                {
                    // It's a mercenary in its cage. It shouldn't take any damage.
                    return;
                }

                if (s != ForkTriggerEnemy)
                {
                    HitCounter++;
                }

                // start the AE Effect
                if (ForkDamage)
                {
                    _forkTriggerEnemy = s;
                    DoForkDamage();
                }
            }
        }
        #endregion

        #region "public"

        /// <summary>
        /// resets the projectile to default values and inactive state
        /// [RPC]
        /// </summary>
        [RPC]
        public void NetworkReset()
        {
            networkView.RPC("Reset", RPCMode.All);
        }

        /// <summary>
        /// resets the projectile to default values and inactive state
        /// [RPC]
        /// </summary>
        [RPC]
        public void Reset()
        {
            if (ActivePrefab != null)
            {
                Destroy(ActivePrefab.gameObject);
                ActivePrefab = null;
            }

            HitCounter = 0;
            Moved = 0;
            Inactive = true;
            Attached = false;
            HasGravity = false;
            Range = 0f;
            Speed = 0;
            Damage = 0;
            StartUp = 0;
            TimeToLive = 0;
            StartPosition = Vector3.zero;
            PrefabSize = new Vector3(1f, 1f, 1f);
            AoEPrefabSize = Vector3.zero;
            PrefabRotation = Vector3.zero;
            AoEPrefabRotation = Vector3.zero;
            AoEPrefab = null;
            ActivePrefab = null;
            ForkPrefab = null;
            gameObject.renderer.enabled = false;
            transform.localScale = new Vector3(1f, 1f, 1f);
            ((BoxCollider)collider).size = new Vector3(1f, 1f, 1f);
            ProjectileDirection = Vector3.forward;
            PlayerDirection = Vector3.forward;
            Knockback = Vector3.zero;
            Hitstun = 0;
            Player = null;
            PlayerObj = null;
            ForkTriggerEnemy = null;
            _forkTriggerEnemy = null;
            DoPiercing = false;
            DamageLoss = 0;
            ForkDamage = false;
            AoEDamage = false;
            ForkArcRadius = 0;
            AoESize = Vector3.zero;
            AoEKnockbackVert = Vector3.zero;
            AoEKnockbackHor = 0;
            AoEDamageValue = 0;
            AoETime = 0;
            DoMultiForkDamage = false;
            ForkMultiDamageDelay = 0.0f;
            ForkMultiDamageRate = 0.0f;
        }

        /// <summary>
        /// activate the projectile
        /// </summary>
        public void Apply()
        {
            SpawnTime = Time.time;
            if (ActivePrefab != null)
            {
                GameObject prefab;
                if (_net.IsServer)
                {
                    prefab = (GameObject)Network.Instantiate(ActivePrefab, transform.position, Quaternion.Euler(PrefabRotation), 0);
                    string oldName = prefab.name;
                    prefab.name = prefab.name + prefab.GetInstanceID();
                    networkView.RPC("NetworkAttachPrefab", RPCMode.Others, oldName, prefab.name);
                }
                else
                {
                    prefab = (GameObject)Instantiate(ActivePrefab, transform.position, Quaternion.Euler(PrefabRotation));
                }

                ActivePrefab = prefab;
                prefab.transform.localScale = PrefabSize;
                prefab.transform.parent = transform;
                prefab.renderer.enabled = true;
                if (ActivePrefab.animation != null && TimeToLive == 0)
                {
                    ActivePrefab.animation["Flying"].wrapMode = WrapMode.Loop;
                    ActivePrefab.animation.Play("Flying");
                    if (_net.IsServer)
                    {
                        networkView.RPC("NetworkPlayAnimation", RPCMode.Others, "Flying");
                    }
                }
            }

            if (StartUp > 0 && !_net.IsClient)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - 10, transform.position.z);
                StartCoroutine(Projectiledelay(StartUp));
            }
            else
            {
                if (!_net.IsClient)
                {
                    Inactive = false;
                    transform.position = new Vector3(
                        transform.position.x + StartPosition.x,
                        transform.position.y + collider.bounds.extents.y + StartPosition.y,
                        transform.position.z + StartPosition.z);
                    if (Attached)
                    {
                        transform.parent = PlayerObj.transform;
                    }
                }

                if (DoMultiForkDamage)
                {
                    StartCoroutine(MultiForkDamage());
                }
            }
        }

        /// <summary>
        /// Attach a Projectile to this gameobject (into ActiveProjectile)
        /// [RPC] Methode is visible over Network.
        /// </summary>
        /// <param name="oldName">projectile prefab to attach (old)</param>
        /// <param name="prefabName">projectile prefab to attach</param>
        [RPC]
        public void NetworkAttachPrefab(string oldName, string prefabName)
        {
            ActivePrefab = GameObject.Find(oldName);
            Debug.Log(ActivePrefab);
            ActivePrefab.name = prefabName;
            ActivePrefab.transform.localScale = PrefabSize;
            ActivePrefab.transform.parent = transform;
            ActivePrefab.renderer.enabled = true;
        }

        /// <summary>
        /// Syncs the current Projectile Pool position
        /// </summary>
        public void SyncProjectilePool()
        {
            networkView.RPC("SyncProjectilePoolPosition", RPCMode.Others, ProjectilePool.CurrentPosition);
        }

        #endregion

        #region "protected"
        /// <summary>
        /// setmove direction
        /// </summary>
        protected void SetKnockback()
        {
            PlayerDirection = Vector3.back;
        }

        /// <summary>
        /// triggers the Fork Effect
        /// </summary>
        protected void DoForkDamage()
        {
            if (ActivePrefab != null)
            {
                if (_net.IsServer)
                {
                    Network.Destroy(ActivePrefab.gameObject);
                }
                else if (!_net.IsClient)
                {
                    Destroy(ActivePrefab.gameObject);
                }
            }

            // fire 10 projectiles around the main projectile
            for (int i = 0; i < 20; i++)
            {
                Projectile proj = ProjectilePool.GetProjectile(transform.position, Quaternion.Euler(Vector3.up * 360 / 20 * i));
                if (ForkPrefab != null)
                {
                    proj.ActivePrefab = ForkPrefab;
                    proj.PrefabSize = new Vector3(1, 1, 1);
                }

                proj.ForkTriggerEnemy = ForkTriggerEnemy;
                proj.transform.localScale = transform.localScale;
                proj.SetKnockback();
                proj.ForkTriggerEnemy = _forkTriggerEnemy;
                proj.Knockback = proj.transform.forward.normalized * 10;
                proj.Knockback.y = 10;
                proj.Range = ForkArcRadius;
                proj.Speed = 17;
                proj.Player = Player;
                proj.Damage = Damage / 2;
                proj.ForkDamage = false;
                proj.Apply();
            }
        }

        /// <summary>
        /// triggers the AE Effect
        /// </summary>
        protected void DoAoEDamage()
        {
            if (ActivePrefab != null)
            {
                if (_net.IsServer)
                {
                    Network.Destroy(ActivePrefab.gameObject);
                }
                else if (!_net.IsClient)
                {
                    Destroy(ActivePrefab.gameObject);
                }
            }

            Projectile proj = ProjectilePool.GetProjectile(transform.position, Quaternion.Euler(Vector3.zero));
            if (AoEPrefab != null)
            {
                proj.ActivePrefab = AoEPrefab;
                proj.PrefabSize = AoEPrefabSize;
                proj.PrefabRotation = AoEPrefabRotation;
            }

            proj.transform.localScale = AoESize;
            ((BoxCollider)collider).size = AoESize;
            proj.Knockback = AoEKnockbackVert;
            proj.AoEKnockbackHor = AoEKnockbackHor;
            proj.TimeToLive = AoETime;
            proj.Range = 1;
            proj.Speed = 0;
            proj.Player = Player;
            proj.DoPiercing = true;
            proj.Damage = AoEDamageValue;
            proj.Apply();
        }
        #endregion

        #region "private"
        /// <summary>
        /// triggers multi ae damage
        /// </summary>
        /// <returns>rate to spread the ae</returns>
        private IEnumerator MultiForkDamage()
        {
            yield return new WaitForSeconds(ForkMultiDamageDelay);
            while (!Inactive && ForkDamage)
            {
                DoForkDamage();
                yield return new WaitForSeconds(ForkMultiDamageRate);
            }
        }

        /// <summary>
        /// main movement of the projectile
        /// </summary>
        private void Move()
        {
            float amtToMove = Speed * Time.deltaTime;
            Moved += amtToMove;
            if (HasGravity)
            {
                ProjectileDirection.y -= 0.5f * Time.deltaTime;
            }

            gameObject.transform.Translate(ProjectileDirection * amtToMove);
        }

        [RPC]
        private void NetworkPlayAnimation(string animation)
        {
            ActivePrefab.animation[animation].wrapMode = WrapMode.Loop;
            ActivePrefab.animation.Play(animation);
        }

        /// <summary>
        /// delays the projectile
        /// </summary>
        /// <param name="time">startup time</param>
        /// <returns>delay to set</returns>
        private IEnumerator Projectiledelay(float time)
        {
            yield return new WaitForSeconds(time);
            if (!Player.IsKnockedBack)
            {
                Inactive = false;
                transform.position = new Vector3(
                    Player.transform.position.x + StartPosition.x,
                    Player.transform.position.y + collider.bounds.extents.y + StartPosition.y,
                    Player.transform.position.z + StartPosition.z);
                if (Attached)
                {
                    transform.parent = PlayerObj.transform;
                }
            }
        }

        [RPC]
        private void SyncProjectilePoolPosition(int pos)
        {
            ProjectilePool.CurrentPosition = pos;
        }
        #endregion
        #endregion
    }
}

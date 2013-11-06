namespace Assets.Scripts.Heroes
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Represents a Cockman hero
    /// </summary>
    public class CockMan : Hero
    {
        /// <summary>
        /// Variable for character sounds. Default 0=Attack 1=Dying
        /// </summary>
        private AudioSource[] _clip;

        /// <summary>
        /// Gets or sets Prefab for ranged attack
        /// </summary>
        public GameObject EggPartPrefab
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Prefab for ranged attack
        /// </summary>
        public GameObject EggsplosionPrefab
        {
            get;
            set;
        }

        /// <summary>
        /// Unity's start method. Sets up animation speeds.
        /// </summary>
        public override void Start()
        {
            base.Start();

            if (animation != null)
            {
                animation["Walking"].speed = 1.5f;
                animation["Running"].speed = 1;
                animation["Jumping"].speed = 2.5f;
                animation["Falling"].speed = 0.2f;
                animation["Landing"].speed = 1;
                animation["Attacking"].speed = 1.5f;
                animation["Standing"].speed = 0.1f;
                animation["BeingHit"].speed = 1;
                animation["Dying"].speed = 1;
                animation["Special MultiShot"].speed = 2;
                animation["Special Eggsplosion"].speed = 0.5f;
                animation["ItemInteraction"].speed = 2.5f;
                animation["ItemConsumption"].speed = 2.5f;
            }
        }

        /// <summary>
        /// called on awake
        /// </summary>
        public override void Awake()
        {
            if (GameManager.GetInstance().GameMode == GameManager.Mode.PLAY)
            {
                MaxStamina = 20 - (3 * GameManager.GetInstance().Difficulty);
                StaminaUseRate = 1 + (0.5f * GameManager.GetInstance().Difficulty);
                MaxMP = 550 - (100 * GameManager.GetInstance().Difficulty);
            }
            else
            {
                MaxStamina = 10;
                StaminaUseRate = 1;
                MaxMP = 250;
            }

            ManaRegenerationAmount = 5;

            BasicDamage = 1;
            MeeleRange = 17f;

            DefendDamagePercentage = 0.15f;

            AttackDamage = 65;
            SpecialDamageMultiplier = 1.5f;

            MaxHP = 500;

            StaminaRegRate = 1;

            MovementSpeed = 4.2f;
            JumpSpeed = 30f;
            RunningFactor = 2.5f;

            DisplayName = "Cock Man";

            _clip = GetComponents<AudioSource>();
            _clip[0].volume = ConfigManager.GetInstance().SoundLevel;
            _clip[1].volume = ConfigManager.GetInstance().SoundLevel;
            DyingSound = _clip[1];

            EggPartPrefab = Resources.Load("Attacks/EggPartPrefab") as GameObject;
            EggsplosionPrefab = Resources.Load("Attacks/EggsplosionPrefab") as GameObject;
            base.Awake();
        }

        /// <summary>
        /// Add the key combos for the special attacks of this Character.
        /// </summary>
        protected override void AddSpecialAttackKeyCombos()
        {
            base.AddSpecialAttackKeyCombos();

            // Eggsplosion
            StateMachine.AddSpecialAttack(
                new KeyCode[] 
                {
                    PlayerControlKeys.DefendKey, 
                    PlayerControlKeys.DefendKey,
                    PlayerControlKeys.DefendKey,
                },
                PerformEggsplosion);

            // SPLASH
            StateMachine.AddSpecialAttack(
                new KeyCode[] 
                {
                    PlayerControlKeys.DefendKey, 
                    PlayerControlKeys.DefendKey,
                    PlayerControlKeys.AttackKey,
                },
                PerformSplashAttack);

            // MULTI SHOT
            StateMachine.AddSpecialAttack(
                new KeyCode[]
                {
                    PlayerControlKeys.DefendKey, 
                    PlayerControlKeys.JumpKey, 
                    PlayerControlKeys.AttackKey
                },
                PerformMultiShotAttack);
        }

        /// <summary>
        /// starts normal attack
        /// </summary>
        [RPC]
        protected override void PerformAttack()
        {
            if (Projectile == null)
            {
                Projectile = Resources.Load("Attacks/EggPrefab") as GameObject;
            }

            if (_net.IsClient)
            {
                networkView.RPC("PerformAttack", RPCMode.Server);
            }
            else
            {
                Projectile proj;
                proj = ProjectilePool.GetProjectile(new Vector3(0, -1000, 0), transform.rotation);
                proj.transform.localScale = new Vector3(1f, 1f, 1f);
                proj.StartPosition = new Vector3(0, collider.bounds.extents.y * 1.5f, 0);
                proj.ActivePrefab = Projectile;
                Debug.Log(proj.ActivePrefab);
                proj.PrefabSize = new Vector3(0.5f, 0.5f, 0.5f);
                proj.PrefabRotation = new Vector3(0, 270, 0);
                proj.ProjectileDirection.y = 0.1f;
                proj.Range = MeeleRange + 1.2f;
                proj.Speed = 20;
                proj.StartUp = 0.35f;
                proj.Player = this;
                proj.HasGravity = true;
                proj.Damage = AttackDamage;
                proj.Knockback = transform.forward.normalized * 10f;
                proj.Knockback.y = 10.0f;
                proj.Apply();

                StartCoroutine(AttackDelay("Attacking"));
            }

            // _clip[0].volume = ConfigManager.GetInstance().SoundLevel;
            // _clip[0].Play();
        }

        /// <summary>
        /// performs a devasting explosion
        /// </summary>
        [RPC]
        protected void PerformEggsplosion()
        {
            Debug.Log("Special Attack: Eggsplosion");

            if (EggsplosionPrefab == null)
            {
                EggsplosionPrefab = Resources.Load("Attacks/EggsplosionPrefab") as GameObject;
            }

            if (Projectile == null)
            {
                Projectile = Resources.Load("Attacks/EggPrefab") as GameObject;
            }

            if (MP < 50)
            {
                return;
            }

            if (_net.IsClient)
            {
                networkView.RPC("PerformEggsplosion", RPCMode.Server);
            }
            else
            {
                Projectile proj;
                proj = ProjectilePool.GetProjectile(new Vector3(0, 0, 0), transform.rotation);
                proj.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                proj.StartPosition = new Vector3(0, collider.bounds.extents.y * 0.1f, 0);
                proj.ActivePrefab = Projectile;
                proj.PrefabSize = new Vector3(2f, 2f, 2f);
                proj.PrefabRotation = new Vector3(0, 270, 0);
                proj.AoEPrefab = EggsplosionPrefab;
                proj.AoEPrefabSize = new Vector3(6, 6, 6);
                proj.Range = 1;
                proj.Speed = 0f;
                proj.TimeToLive = 1.5f;
                proj.StartUp = 0.45f;
                proj.Player = this;
                proj.Damage = 0;
                proj.AoEDamage = true;
                proj.AoEKnockbackVert = Vector3.up * 30;
                proj.AoEKnockbackHor = 20;
                proj.AoEDamageValue = AttackDamage + 20;
                proj.AoESize = new Vector3(10, 10, 10);
                proj.AoETime = 1;
                proj.Knockback.y = 8.0f;
                proj.Apply();

                IsKnockbackImmune = true;
                IsDamageImmune = true;

                MP -= 60;

                StartCoroutine(AttackDelay("Special Eggsplosion"));
            }
        }

        /// <summary>
        /// performs 3 attacks in row
        /// </summary>
        [RPC]
        protected void PerformMultiShotAttack()
        {
            Debug.Log("Special Attack: MultiShot");

            if (Projectile == null)
            {
                Projectile = Resources.Load("Attacks/EggPrefab") as GameObject;
            }

            if (MP < 25)
            {
                return;
            }

            if (_net.IsClient)
            {
                networkView.RPC("PerformMultiShotAttack", RPCMode.Server);
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    Projectile proj = ProjectilePool.GetProjectile(ProjectilePos, transform.rotation);
                    proj.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                    proj.PrefabSize = new Vector3(0.8f, 0.8f, 0.8f);
                    proj.StartPosition = new Vector3(0, collider.bounds.extents.y * 1.5f, 0);
                    proj.ActivePrefab = Projectile;
                    proj.PrefabRotation = new Vector3(0, 270, 0);
                    proj.Range = 20;
                    proj.Speed = 20;
                    proj.StartUp = 0.25f + (0.6f * i);
                    proj.Player = this;
                    proj.Damage = AttackDamage - 10;
                    proj.Knockback = Vector3.zero;
                    proj.Knockback.y = 19.0f;
                    proj.Apply();

                    IsKnockbackImmune = true;

                    MP -= 40;

                    StartCoroutine(AttackDelay("Special MultiShot"));
                }
            }
        }

        /// <summary>
        /// shoots a projectile with fork effect
        /// </summary>
        [RPC]
        protected void PerformSplashAttack()
        {
            Debug.Log("Special Attack: Splash");

            if (Projectile == null)
            {
                Projectile = Resources.Load("Attacks/EggPrefab") as GameObject;
            }

            if (EggPartPrefab == null)
            {
                EggPartPrefab = Resources.Load("Attacks/EggPartPrefab") as GameObject;
            }

            if (MP < 30)
            {
                return;
            }

            if (_net.IsClient)
            {
                networkView.RPC("PerformSplashAttack", RPCMode.Server);
            }
            else
            {
                Projectile proj = ProjectilePool.GetProjectile(ProjectilePos, transform.rotation);
                proj.transform.localScale = new Vector3(1f, 1f, 1f);
                proj.PrefabSize = new Vector3(0.6f, 0.6f, 0.6f);
                proj.StartPosition = new Vector3(0, collider.bounds.extents.y * 1.5f, 0);
                proj.ActivePrefab = Projectile;
                proj.PrefabRotation = new Vector3(0, 270, 0);
                proj.Range = MeeleRange - 0.8f;
                proj.Speed = 20;
                proj.StartUp = 0.25f;
                proj.Player = this;
                proj.Damage = AttackDamage;
                proj.HasGravity = true;
                proj.ProjectileDirection.y = 0.1f;
                proj.Knockback = transform.forward.normalized * 10;
                proj.Knockback.y = 8.0f;
                proj.ForkDamage = true;
                proj.ForkArcRadius = 7.0f;
                proj.ForkPrefab = EggPartPrefab;
                proj.Apply();

                MP -= 40;

                StartCoroutine(AttackDelay("Attacking"));
            }
        }

        /// <summary>
        /// block movement while attacking
        /// </summary>
        /// <param name="time">Duration of the attack</param>
        /// <returns>An IEnumerator indicating how long to block all movement.</returns>
        private IEnumerator MultiShotDelay(float time)
        {
            yield return new WaitForSeconds(time);
            animation["Attacking"].speed = 2f;
        }
    }
}

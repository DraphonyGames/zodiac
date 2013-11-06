namespace Assets.Scripts.Heroes
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// represents a Tigerman Hero
    /// </summary>
    public class TigerMan : Hero
    {
        #region "Fields"
        /// <summary>
        /// Gets or sets Prefab for ranged attack
        /// </summary>
        public GameObject EarthquakePrefab;

        /// <summary>
        /// Variable for attack sound
        /// </summary>
        private AudioSource[] _clip;
        #endregion

        /// <summary>
        /// Called on Startup
        /// </summary>
        public override void Start()
        {
            base.Start();

            if (animation != null)
            {
                animation["Walking"].speed = 3;
                animation["Running"].speed = 3;
                animation["Jumping"].speed = 2;
                animation["Falling"].speed = 0.2f;
                animation["Landing"].speed = 1;
                animation["Attacking"].speed = 3;
                animation["Standing"].speed = 0.1f;
                animation["BeingHit"].speed = 4;
                animation["Dying"].speed = 1;
                animation["ItemInteraction"].speed = 2.5f;
                animation["ItemConsumption"].speed = 1f;
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
            MeeleRange = 2f;

            DefendDamagePercentage = 0.15f;

            AttackDamage = 80;
            SpecialDamageMultiplier = 1.5f;

            MaxHP = 500;            

            StaminaRegRate = 1;

            MovementSpeed = 5f;
            JumpSpeed = 25f;
            RunningFactor = 2.8f;

            DisplayName = "Tiger Man";

            _clip = GetComponents<AudioSource>();
            _clip[0].volume = ConfigManager.GetInstance().SoundLevel;
            _clip[1].volume = ConfigManager.GetInstance().SoundLevel;
            DyingSound = _clip[1];

            EarthquakePrefab = Resources.Load("Attacks/EarthquakePrefab") as GameObject;
            
            base.Awake();
        }

        /// <summary>
        /// Add the key combos for the special attacks of this Character.
        /// </summary>
        protected override void AddSpecialAttackKeyCombos()
        {
            base.AddSpecialAttackKeyCombos();

            // Hammer
            StateMachine.AddSpecialAttack(
                new KeyCode[] 
                {
                PlayerControlKeys.DefendKey, 
                PlayerControlKeys.DefendKey,
                PlayerControlKeys.DefendKey,
                },
                PerformHammer);

            // DropKick
            StateMachine.AddSpecialAttack(
               new KeyCode[] 
                {
                PlayerControlKeys.DefendKey, 
                PlayerControlKeys.JumpKey,
                PlayerControlKeys.AttackKey,
                },
               PerformDropKick);

            // UpperCut
            StateMachine.AddSpecialAttack(
              new KeyCode[] 
                {
                PlayerControlKeys.DefendKey, 
                PlayerControlKeys.DefendKey,
                PlayerControlKeys.AttackKey,
                },
              PerformUpperCut);
        }

        /// <summary>
        /// starts normal attack
        /// </summary>
        [RPC]
        protected override void PerformAttack()
        {
            if (_net.IsClient)
            {
                networkView.RPC("PerformAttack", RPCMode.Server);
            }
            else
            {
                Projectile proj;
                proj = ProjectilePool.GetProjectile(ProjectilePos, transform.rotation);
                proj.transform.localScale = new Vector3(2f, 2f, 2f);
                proj.Range = MeeleRange + 0.2f;
                proj.DoPiercing = true;
                proj.Speed = 13;
                proj.Player = this;
                proj.Damage = AttackDamage;
                proj.Knockback = transform.forward.normalized * 8;
                proj.Knockback.y += 17.0f;
                proj.Apply();

                StartCoroutine(AttackDelay("Attacking"));
            }

            _clip[0].volume = ConfigManager.GetInstance().SoundLevel;
            _clip[0].Play();
        }

        /// <summary>
        /// performs a devasting close ranged  AoE effect
        /// </summary>
        [RPC]
        protected void PerformHammer()
        {
            if (EarthquakePrefab == null)
            {
                EarthquakePrefab = Resources.Load("Attacks/EarthquakePrefab") as GameObject;
            }

            if (MP < 60)
            {
                return;
            }

            if (_net.IsClient)
            {
                networkView.RPC("PerformHammer", RPCMode.Server);
            }
            else
            {
                Projectile proj = ProjectilePool.GetProjectile(ProjectilePos, transform.rotation);
                proj.StartPosition = Vector3.up * 3;
                proj.transform.localScale = new Vector3(2f, 2f, 2f);
                proj.ProjectileDirection = new Vector3(proj.ProjectileDirection.x, -1.5f, proj.ProjectileDirection.z);
                proj.AoEPrefab = EarthquakePrefab;
                proj.AoEPrefabSize = new Vector3(1.0f, 1.0f, 1.0f);
                proj.AoEPrefabRotation = new Vector3(270f, 0, 0);
                proj.Range = 2.5f;
                proj.Speed = 9f;
                proj.StartUp = 0.35f;
                proj.Player = this;
                proj.DoPiercing = true;
                proj.Damage = AttackDamage + 40;
                proj.Knockback = transform.forward.normalized * 25;
                proj.Knockback.y = 30.0f;
                proj.AoEDamage = true;
                proj.AoEKnockbackVert = Vector3.up * 20;
                proj.AoESize = new Vector3(10.0f, 1f, 10.0f);
                proj.AoEDamageValue = AttackDamage / 4;
                proj.AoETime = 0.5f;
                proj.Apply();

                MP -= 60;

                StartCoroutine(AttackDelay("Special Hammer"));
            }
        }

        /// <summary>
        /// performs a jumpattack
        /// </summary>
        [RPC]
        protected void PerformDropKick()
        {
            if (MP < 80)
            {
                return;
            }

            if (_net.IsClient)
            {
                networkView.RPC("PerformDropKick", RPCMode.Server);
            }
            else
            {
                Projectile proj = ProjectilePool.GetProjectile(ProjectilePos, transform.rotation);
                proj.transform.localScale = new Vector3(3f, 3f, 3f);
                proj.StartPosition = transform.forward.normalized * 2f;
                proj.ProjectileDirection = new Vector3(0, 0, 0);
                proj.Attached = true;
                proj.Range = MeeleRange;
                proj.Speed = 0;
                proj.TimeToLive = 0.55f;
                proj.Player = this;
                proj.PlayerObj = this.gameObject;
                proj.DoPiercing = true;
                proj.Damage = AttackDamage - 20;
                proj.Knockback = transform.forward.normalized * 25;
                proj.Knockback.y = 15.0f;
                proj.Apply();
                CanMove = true;
                IsDefending = false;

                IsAttacking = true;
                IsJumping = true;

                IsKnockbackImmune = true;

                MP -= 80;

                animation.Play("Special DropKick");
                if (_net.IsServer)
                {
                    networkView.RPC("NetworkPlayAnimation",
                        RPCMode.Others,
                        "Special DropKick",
                        -1f,
                        false);
                }
            }

            if (IsMine())
            {
                Vector3 dir = transform.forward.normalized * 25;
                dir.y = 15;
                Direction = dir;
                Move();
            }
        }

        /// <summary>
        /// performs a jumpattack
        /// </summary>
        [RPC]
        protected void PerformUpperCut()
        {
            if (MP < 50)
            {
                return;
            }

            if (_net.IsClient)
            {
                networkView.RPC("PerformUpperCut", RPCMode.Server);
            }
            else
            {
                Projectile proj = ProjectilePool.GetProjectile(ProjectilePos, transform.rotation);

                // proj.StartPosition = ProjectilePos + Vector3.up * -0.6f;
                proj.transform.localScale = new Vector3(2f, 2f, 2f);
                proj.StartPosition = transform.forward.normalized * 1.5f;
                proj.ProjectileDirection = new Vector3(0, 1f, 0);
                proj.Range = MeeleRange + 2;
                proj.StartUp = 0.25f;
                proj.Speed = 13;
                proj.Player = this;
                proj.Damage = AttackDamage;
                proj.Knockback = transform.forward.normalized * -6;
                proj.Knockback.y = 40.0f;
                proj.Apply();

                MP -= 50;

                StartCoroutine(AttackDelay("Special UpperCut"));
            }
        }
    }
}

namespace Assets.Scripts.Mobs
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// represents a ranged mob
    /// </summary>
    public class Ranged : Mob
    {
        /// <summary>
        /// Variable for attack sound
        /// </summary>
        private AudioSource[] _clip;

        /// <summary>
        /// Gets or sets Prefab for ranged attack
        /// </summary>
        public GameObject QuarryPrefab
        {
            get;
            set;
        }

        /// <summary>
        /// Executed on Instatiation of Mob
        /// </summary>
        public override void Awake()
        {
            if (GameManager.GetInstance().GameMode == GameManager.Mode.PLAY)
            {
                AttackDamage = 10 + (GameManager.GetInstance().Difficulty * 20 * GameManager.GetInstance().Difficulty);
                MaxHP = 90 + (40 * GameManager.GetInstance().Difficulty);
                AttackRechargeTime = 4f - (0.5f * GameManager.GetInstance().Difficulty);
            }
            else
            {
                AttackDamage = 50;
                MaxHP = 50;
                AttackRechargeTime = 1.5f;
            }

            BasicDamage = 1;
            MeeleRange = 20f;
            DefendDamagePercentage = 0.15f;
            DisplayName = "Ranger";
            SpecialDamageMultiplier = 1.0f;

            MaxMP = 215;

            MovementSpeed = 5f;
            JumpSpeed = 18f;           

            if (animation != null)
            {
                animation["Walking"].speed = 1.5f;
                animation["Jumping"].speed = 2;
                animation["Falling"].speed = 0.2f;
                animation["Landing"].speed = 1;
                animation["Attacking"].speed = 1.5f;
                animation["Standing"].speed = 0.1f;
            }

            _clip = GetComponents<AudioSource>();
            _clip[0].volume = ConfigManager.GetInstance().SoundLevel;
            _clip[1].volume = ConfigManager.GetInstance().SoundLevel;
            DyingSound = _clip[1];

            base.Awake();
        }

        /// <summary>
        /// called on start
        /// </summary>
        public override void Start()
        {
            // Always executing base.Start() firsts
            base.Start();
            QuarryPrefab = Resources.Load("Attacks/QuarryPrefab") as GameObject;
        }

        /// <summary>
        /// performs a standart attack
        /// </summary>
        [RPC]
        protected override void PerformAttack()
        {
            if (QuarryPrefab == null)
            {
                QuarryPrefab = Resources.Load("Attacks/QuarryPrefab") as GameObject;
            }

            if (_net.IsClient)
            {
                networkView.RPC("PerformAttack", RPCMode.Server);
            }
            else
            {
                Projectile proj = ProjectilePool.GetProjectile(new Vector3(0, -111, 0), transform.rotation);
                proj.StartPosition = new Vector3(0, collider.bounds.extents.y, 0);
                proj.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
                proj.ActivePrefab = QuarryPrefab;
                proj.PrefabSize = new Vector3(6f, 50f, 6f);
                proj.PrefabRotation = new Vector3(90f, transform.localEulerAngles.y, 0);
                proj.Range = MeeleRange + 0.2f;
                proj.Speed = 20;
                proj.StartUp = 0.3f;
                proj.Player = this;
                proj.Damage = AttackDamage;
                proj.Knockback = Vector3.zero;
                proj.Knockback = transform.forward.normalized * 10;
                proj.Knockback.y = 7.0f;
                proj.Apply();
            }

            _clip[0].volume = ConfigManager.GetInstance().SoundLevel;
            _clip[0].Play();
            StartCoroutine(AttackDelay("Attacking"));
        }
    }
}

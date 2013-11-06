namespace Assets.Scripts.Mercenaries
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// represents a Kroko merc
    /// </summary>
    public class Kroko : Mercenary
    {
        /// <summary>
        /// Variable for attack sound
        /// </summary>
        private AudioSource[] _clip;

        /// <summary>
        /// called on awake
        /// </summary>
        public override void Awake()
        {
            BasicDamage = 1;
            MeeleRange = 2.5f;

            DefendDamagePercentage = 0.15f;

            AttackDamage = 80;
            SpecialDamageMultiplier = 1.0f;

            MaxHP = 265;
            MaxMP = 0;

            MovementSpeed = 6f;
            JumpSpeed = 10f;
            RunningFactor = 2.7f;

            AttackRechargeTime = 0.75f;

            if (animation != null)
            {
                animation["Walking"].speed = 1.5f;
                animation["Running"].speed = 3;
                animation["Jumping"].speed = 2;
                animation["Falling"].speed = 0.2f;
                animation["Landing"].speed = 1;
                animation["Attacking"].speed = 1;
                animation["Standing"].speed = 0.1f;
                animation["BeingHit"].speed = 2;
                animation["Dying"].speed = 1;
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
        }

        /// <summary>
        /// performs standart attack
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
                Projectile proj = ProjectilePool.GetProjectile(transform.position, transform.rotation);
                proj.StartPosition = this.transform.forward * 1.5f;
                proj.Range = MeeleRange + 0.2f;
                proj.Speed = 7;
                proj.Player = this;
                proj.Damage = AttackDamage;
                proj.Knockback = Vector3.zero;
                proj.Knockback = transform.forward.normalized * 15;
                proj.Knockback.y = 15.0f;
                proj.DoPiercing = true;
                proj.Apply();
            }

            _clip[0].volume = ConfigManager.GetInstance().SoundLevel;
            _clip[0].Play();
            StartCoroutine(AttackDelay("Attacking"));
        }
    }
}

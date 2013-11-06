namespace Assets.Scripts.Mercenaries
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// represents a chamelion merc
    /// </summary>
    public class Chamelion : Mercenary
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
            MeeleRange = 15f;

            DefendDamagePercentage = 0.15f;

            AttackDamage = 50;
            SpecialDamageMultiplier = 1.0f;

            MaxHP = 215;
            MaxMP = 0;

            MovementSpeed = 5f;
            JumpSpeed = 10f;
            RunningFactor = 2.7f;

            AttackRechargeTime = 1.5f;

            if (animation != null)
            {
                animation["Walking"].speed = 1.5f;
                animation["Running"].speed = 3;
                animation["Jumping"].speed = 2;
                animation["Falling"].speed = 0.2f;
                animation["Landing"].speed = 1;
                animation["Attacking"].speed = 0.7f;
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
                proj.StartPosition = new Vector3(0, 0, 0);
                proj.Range = MeeleRange + 0.2f;
                proj.Speed = 20;
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

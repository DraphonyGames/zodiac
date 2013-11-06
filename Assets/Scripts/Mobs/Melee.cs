namespace Assets.Scripts.Mobs
{
    using UnityEngine;

    /// <summary>
    /// represents a melee mob
    /// </summary>
    public class Melee : Mob
    {
        /// <summary>
        /// Variable for sword attack sound
        /// </summary>
        private AudioSource[] _clip;

        /// <summary>
        /// Executed on Instatiation of Mob
        /// </summary>
        public override void Awake()
        {
            if (GameManager.GetInstance().GameMode == GameManager.Mode.PLAY)
            {
                AttackDamage = 15 + (GameManager.GetInstance().Difficulty * 20 * GameManager.GetInstance().Difficulty);
                MaxHP = 135 + (40 * GameManager.GetInstance().Difficulty);
                AttackRechargeTime = 3.75f - (0.5f * GameManager.GetInstance().Difficulty);
            }
            else
            {
                AttackDamage = 80;
                MaxHP = 265;
                AttackRechargeTime = 0.25f;
            }

            BasicDamage = 1;
            MeeleRange = 2.6f;
            DefendDamagePercentage = 0.15f;
            DisplayName = "Bully";
            SpecialDamageMultiplier = 1.0f;

            MaxMP = 0;

            MovementSpeed = 6f;
            JumpSpeed = 18f;

            if (animation != null)
            {
                animation["Walking"].speed = 1.5f;
                animation["Jumping"].speed = 2;
                animation["Falling"].speed = 0.2f;
                animation["Landing"].speed = 1;
                animation["Attacking"].speed = 1;
                animation["Standing"].speed = 0.1f;
            }

            _clip = GetComponents<AudioSource>();
            _clip[0].volume = ConfigManager.GetInstance().SoundLevel;
            _clip[1].volume = ConfigManager.GetInstance().SoundLevel;
            DyingSound = _clip[1];

            base.Awake();
        }

        /// <summary>
        /// performs a standart attack
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
                Projectile proj = ProjectilePool.GetProjectile(new Vector3(0, -111, 0), transform.rotation);
                proj.StartPosition = new Vector3(0, collider.bounds.extents.y * 0.5f, 0);
                proj.transform.localScale = new Vector3(2, 2, 2);
                proj.Range = MeeleRange + 0.2f;
                proj.Speed = 15;
                proj.StartUp = 0.3f;
                proj.Player = this;
                proj.Damage = AttackDamage;
                proj.Knockback = Vector3.zero;
                proj.Knockback = transform.forward.normalized * 15;
                proj.Knockback.y = 12.0f;
                proj.Apply();
            }

            _clip[0].volume = ConfigManager.GetInstance().SoundLevel;
            _clip[0].Play();
            StartCoroutine(AttackDelay("Attacking"));
        }
    }
}
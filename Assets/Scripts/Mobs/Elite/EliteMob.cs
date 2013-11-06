namespace Assets.Scripts.Mobs.Elite
{
    using UnityEngine;

    /// <summary>
    /// Elite Mob character class
    /// </summary>
    public class EliteMob : Mob
    {
        /// <summary>
        /// Executed on Instatiation of Mob
        /// </summary>
        public override void Awake()
        {
            BasicDamage = 1;
            MeeleRange = 5f;

            DefendDamagePercentage = 0.30f;

            AttackDamage = 100;
            SpecialDamageMultiplier = 1.0f;

            MaxHP = 1000;
            MaxMP = 0;

            MovementSpeed = 13f;
            JumpSpeed = 18f;

            AttackRechargeTime = 0.4f;

            if (animation != null)
            {
                animation["Walking"].speed = 3;
                animation["Jumping"].speed = 2;
                animation["Falling"].speed = 0.2f;
                animation["Landing"].speed = 1;
                animation["Attacking"].speed = 3;
                animation["Standing"].speed = 0.1f;
            }

            base.Awake();
        }

        /// <summary>
        /// Called at first frame
        /// Sets the Particle system for the right team
        /// </summary>
        public override void Start()
        {
            if (Team != null)
            {
                transform.FindChild(Team.TeamNo == 1 ? "BlueParticles" : "RedParticles").particleSystem.Play();
            }

            base.Start();

            float factor = Mathf.Clamp((_net.NumOfConnections / 2) - 1, 0, 10) * 0.5f;

            AttackDamage = AttackDamage + (int)(AttackDamage * factor);
            MaxHP = MaxHP + (int)(MaxHP * factor);

            if (!_net.IsClient)
            {
                AIComponent.IsElite = true;
            }
        }

        /// <summary>
        /// performs a standart attack
        /// </summary>
        protected override void PerformAttack()
        {
            Projectile proj = ProjectilePool.GetProjectile(new Vector3(0, -111, 0), transform.rotation);
            proj.StartPosition = new Vector3(0, collider.bounds.extents.y * 0.5f, 0);
            proj.transform.localScale = new Vector3(2, 2, 2);
            proj.Range = MeeleRange + 2.5f;
            proj.Speed = 20;
            proj.StartUp = 0.3f;
            proj.Player = this;
            proj.Damage = AttackDamage + BasicDamage;
            proj.Knockback = transform.forward.normalized * 5;
            proj.Knockback.y = 15.0f;
            proj.Apply();

            StartCoroutine(AttackDelay("Attacking"));
        }
    }
}

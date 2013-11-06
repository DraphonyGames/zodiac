namespace Assets.Scripts
{
    using UnityEngine;

    /// <summary>
    /// The class defines a basic Mob
    /// </summary>
    public class Mob : Character
    {
        /// <summary>
        /// Kill this mob.
        /// This also destroys the health bar.
        /// </summary>
        /// <param name="killer">The character that killed this entity (optional).</param>
        public override void Die(Character killer)
        {
            if (GameManager.GetInstance().CurrentSection != null)
            {
                GameManager.GetInstance().CurrentSection.KillMob(this);
            }

            // Destroy the health bar as well.
            Destroy(Healthbar);
            Healthbar = null;

            // Make sure we call the parent's method.
            base.Die(killer);
        }

        /// <summary>
        /// Performs an attack and calls this method on the server
        /// if it's executed by a client.
        /// </summary>
        [RPC]
        protected override void PerformAttack()
        {
            /*if (_net.IsClient)
            {
                networkView.RPC("PerformAttack", RPCMode.Server);
            }
            else
            {*/
                Projectile proj = ProjectilePool.GetProjectile(transform.position, transform.rotation);
                proj.StartPosition = transform.position;
                proj.Range = MeeleRange;
                proj.renderer.enabled = false;
                proj.Speed = 10;
                proj.Player = this;
                proj.Damage = AttackDamage + BasicDamage;
                proj.Knockback = Vector3.zero;
                proj.ForkDamage = false;
                proj.ForkArcRadius = 0;
                proj.Apply();

            // }
            StartCoroutine(AttackDelay("Attacking"));
        }
    }
}
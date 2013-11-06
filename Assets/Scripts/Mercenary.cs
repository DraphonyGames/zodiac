namespace Assets.Scripts
{
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// This class represents the character mercenaries, which are animals
    /// In the JungleLevel they fight for the rescuer
    /// In the Multiplayer Level they fight against humans
    /// </summary>
    public class Mercenary : Character
    {
        /// <summary>
        /// Gets or sets the cage, where the mercenary will be contained
        /// </summary>
        public MercenaryCage Cage { get; set; }

        /// <summary>
        /// Gets or sets the player that freed this mercenary.
        /// </summary>
        public Character FreeingPlayer { get; set; }

        /// <summary>
        /// When the object is created in memory
        /// </summary>
        public override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        /// When the gameobject is in the game
        /// </summary>
        public override void Start()
        {
            base.Start();
            if (GameManager.GetInstance().GameMode != GameManager.Mode.SPECIAL)
            {
                CreateCage();
            }
        }

        /// <summary>
        /// Kill this mercenary
        /// </summary>
        /// <param name="killer">The character that killed this entity (optional).</param>
        public override void Die(Character killer)
        {
            if (FreeingPlayer != null)
            {
                FreeingPlayer.StatsMercenariesLost++;
            }

            base.Die(killer);
        }

        /// <summary>
        /// This method will be executed when the mercenary attacks
        /// and defines the attack
        /// </summary>
        protected override void PerformAttack()
        {
        }

        /// <summary>
        /// Called when we get hit by a projectile.
        /// </summary>
        /// <param name="projectile">The projectile that is hitting us.</param>
        protected override void GetHitByProjectile(Projectile projectile)
        {
            if (Cage != null)
            {   // We are still inside our cage and are invulnerable to damage.
                return;
            }

            base.GetHitByProjectile(projectile);
        }

        /// <summary>
        /// Instantiates a MercenaryCage at the own position and links himself to this cage
        /// </summary>
        private void CreateCage()
        {
            GameObject mercenaryCagePrefab = (GameObject)Resources.Load("MercenaryCagePrefab");
            Vector3 cagePosition = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
            
            Cage = ((GameObject)UnityEngine.Object.Instantiate(mercenaryCagePrefab, cagePosition, Quaternion.identity)).GetComponent<MercenaryCage>();
            Cage.MyMercenary = this;

            // Prevent collision of Merc and Merc cage (except for the floor).
            foreach (Transform child in
                Cage.transform.Cast<Transform>().Where(
                    child => child.name != "ColliderBox5" && child.name.StartsWith("ColliderBox")))
            {
                Physics.IgnoreCollision(collider, child.collider);
            }
        }
    }
}

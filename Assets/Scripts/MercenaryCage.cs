namespace Assets.Scripts
{
    using AIHelper;
    using UnityEngine;

    /// <summary>
    /// Represents the cage, where the Mercenarys are locked in before freeing
    /// </summary>
    public class MercenaryCage : MonoBehaviour
    {
        /// <summary>
        /// the current GraphBuilder to rerun the scanning
        /// </summary>
        private GraphBuilder _graph;

        /// <summary>
        /// Gets or sets the mercenary which is in this cage
        /// </summary>
        public Mercenary MyMercenary { get; set; }

        /// <summary>
        /// When the cage collides with a collider
        /// If the collider belongs to a Projectile, cage destroys himself sets active of mercenary
        /// and insert the mercenary into the team of the owner Player of the Projectile
        /// </summary>
        /// <param name="other">The Collider of the GameObject which collides with the cage</param>
        public void OnTriggerEnter(Collider other)
        {
            Projectile p = other.GetComponent<Projectile>();
            if (p != null && MyMercenary.Team == null)
            {
                // Create a broken mercenary cage.
                GameObject brokenMercenaryCagePrefab = (GameObject)Resources.Load("BrokenMercenaryCagePrefab");
                Instantiate(brokenMercenaryCagePrefab, transform.position, Quaternion.identity);

                // Destroy cage.
                Destroy(gameObject);
                MyMercenary.Cage = null;
                _graph.TryMapScanning();

                // Add this mercenary to the player's team.
                GameManager.GetInstance().Mercenaries.Add(MyMercenary);
                p.Player.Team.AddMember(MyMercenary);
                MyMercenary.FreeingPlayer = p.Player;

                MyMercenary.AIComponent.StartAi();

                // Keep track of statistics.
                p.Player.StatsMercenariesFreed++;
                p.HitCounter++;
            }
        }

        /// <summary>
        /// is called when a collision happens 
        /// </summary>
        /// <param name="col">the colision that was detected</param>
        public void OnCollisionEnter(Collision col)
        {
            if (col.gameObject.name == "Ground")
            {
                GameObject graphObject = GameObject.Find("GraphBuilderPrefab(Clone)");
               _graph = graphObject.GetComponent<GraphBuilder>();
               _graph.TryMapScanning();
            }
        }
    }
}

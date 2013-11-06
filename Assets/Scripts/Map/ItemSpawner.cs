namespace Assets.Scripts.Map
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Item spawner for special mode
    /// </summary>
    public class ItemSpawner : MonoBehaviour
    {
        /// <summary>
        /// radius of spawner
        /// </summary>
        private float _radius;

        /// <summary>
        /// Gets or sets the prefab list for items
        /// </summary>
        protected List<AbstractSpawnable> ItemPrefabs { get; set; }

        /// <summary>
        /// called at first frame
        /// </summary>
        public void Start()
        {
            _radius = gameObject.renderer.bounds.size.x / 2;
            gameObject.renderer.enabled = false;

            ItemPrefabs = Datasheet.Items();

            if (!GameManager.GetInstance().NetworkController.IsClient)
            {
                StartCoroutine(Spawn());
            }
        }

        /// <summary>
        /// Spawning coroutine
        /// </summary>
        /// <returns>waiting time in seconds</returns>
        protected IEnumerator Spawn()
        {
            while (true)
            {
                Vector2 randomPoint = Random.insideUnitCircle * _radius;
                Vector3 randomPoint3D = new Vector3(randomPoint.x, 0, randomPoint.y);
                AbstractSpawnable item = ItemPrefabs[Random.Range(0, ItemPrefabs.Count)];
                if (GameManager.GetInstance().NetworkController.IsServer)
                {
                    Network.Instantiate(item, transform.position + randomPoint3D, item.transform.rotation, 0);
                }
                else
                {
                    Instantiate(item, transform.position + randomPoint3D, item.transform.rotation);
                }

                yield return new WaitForSeconds(Random.Range(30f, 80f));
            }
        }
    }
}

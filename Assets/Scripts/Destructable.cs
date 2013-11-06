namespace Assets.Scripts
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Represents the Objects which can be destroyed
    /// </summary>
    public class Destructable : MonoBehaviour
    {
        /// <summary>
        /// The Object which will be instantiated, when this object is destroyed
        /// </summary>
        public GameObject DestroyedVersion;

        /// <summary>
        /// Will be called, when something collides with the destructable, specially Projectiles
        /// </summary>
        /// <param name="other">The other object which collides with the destructable</param>
        public void OnTriggerEnter(Collider other)
        {
            Projectile projectile = other.GetComponent<Projectile>();

            if (projectile != null)
            {
                Debug.Log("Destructable collide");
                if (gameObject.name.Contains("Barrel"))
                {
                    StartCoroutine(DestroyBarrel());
                }
                else
                {
                    Destroy(gameObject);
                    Instantiate(DestroyedVersion, transform.position, Quaternion.identity);
                }
            }
        }

        /// <summary>
        /// Waits 2 Seconds and destroys the barrel
        /// </summary>
        /// <returns>Only waits 2 seconds</returns>
        private IEnumerator DestroyBarrel()
        {
            GameObject barrelSmokePrefab = (GameObject)Resources.Load("Destructables/BarrelSmoke");
            Instantiate(barrelSmokePrefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(2);
            Destroy(gameObject);
            Instantiate(DestroyedVersion, transform.position, Quaternion.identity);
            ApplyAreaDamage();
        }

        /// <summary>
        /// Gives all the Characters in the area damage and knockback
        /// </summary>
        private void ApplyAreaDamage()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f);

            foreach (Collider collider in hitColliders)
            {
                Character character = collider.GetComponent<Character>();
                if (character != null)
                {
                    Vector3 knockback = character.transform.position - transform.position;
                    knockback.y = 0;
                    knockback = knockback.normalized * 7;
                    knockback.y = 35;

                    character.ReceiveDamage(100, character);
                    character.ReceiveKnockback(knockback, Quaternion.identity, Vector3.zero, 0);
                }
            }
        }
    }
}

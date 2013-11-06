namespace Assets.Scripts.Camera
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// The camera script to do the raycast and
    /// start to make the objects hitted transparent
    /// </summary>
    public class SightClearance : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets the distance of the raycast
        /// </summary>
        public float DistanceToPlayer { get; set; }

        /// <summary>
        /// Make an object transparent. If the object has no AutoTransparent component,
        /// it is added.
        /// </summary>
        /// <param name="renderer">The renderer of the object to make transparent.</param>
        public static void MakeTransparent(Renderer renderer)
        {
            AutoTransparent at = renderer.GetComponent<AutoTransparent>();
            if (at == null)
            {
                at = renderer.gameObject.AddComponent<AutoTransparent>();
            }

            at.BeTransparent();
        }

        /// <summary>
        /// Called at instanciate of the obejct
        /// </summary>
        public void Awake()
        {
            DistanceToPlayer = 20.0f;
        }

        /// <summary>
        /// called every frame
        /// </summary>
        public void Update()
        {
            RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, DistanceToPlayer);
            foreach (RaycastHit hit in hits)
            {
                Renderer r = hit.collider.renderer;
                if (r == null)
                {
                    continue;
                }

                // If it is a base wall, make it transparent (since only the walls have colliders).
                GameObject obj = r.gameObject;
                if (obj.name.StartsWith("Wall") && obj.transform.parent != null &&
                    obj.transform.parent.gameObject.name.StartsWith("Base"))
                {
                    // The bases don't have unique names, so look for the first matching one. :/
                    foreach (Transform child in r.transform.parent)
                    {
                        GameObject childObj = child.gameObject;
                        if (childObj.name.StartsWith("Base") && childObj.renderer != null)
                        {
                            MakeTransparent(childObj.renderer);
                            break;
                        }
                    }
                }
                else
                {
                    // It's not a base wall, but a normal object.
                    MakeTransparent(r);
                }
            }
        }
    }
}

namespace Assets.Scripts
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// This class defines a broken cage, which will spawned, when a normal mercenary
    /// cage gets damage
    /// </summary>
    public class BrokenMercenaryCage : MonoBehaviour
    {
        /// <summary>
        /// The y Position of the Object at the start
        /// </summary>
        private float _startPositionY;

        /// <summary>
        /// Starting Method of the Object, sets the startposition y value
        /// </summary>
        public void Start()
        {
            _startPositionY = transform.position.y;
        }

        /// <summary>
        /// Is called once per frame, checks, if object is moved under the terrain
        /// When the cage is under the terrain, it destroys himself
        /// </summary>
        public void Update()
        {
            if (transform.position.y - 9 > _startPositionY)
            {
                Destroy(gameObject);
            }
        }
    }
}

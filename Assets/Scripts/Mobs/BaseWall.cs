namespace Assets.Scripts.Mobs
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Represents a wall of a player base.
    /// </summary>
    public class BaseWall : MonoBehaviour
    {
        #region public methods
        /// <summary>
        /// Unity's OnTriggerEnter callback.
        /// </summary>
        /// <param name="other">The collider we collided with.</param>
        public void OnTriggerEnter(Collider other)
        {
            this.transform.parent.GetComponent<Base>().OnTriggerEnter(other);
        }
        #endregion
    }
}
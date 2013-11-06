namespace Assets.Scripts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    /// <summary>
    /// Basic component for all spawnable things in zodiac
    /// </summary>
    public abstract class AbstractSpawnable : MonoBehaviour
    {
        #region "properties"
        /// <summary>
        /// Gets or sets the ingame name of Spawnable, e.g. Flasche des Todes, The Cockest Cock of all Cocks, ...
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets gravity value for this spawnable
        /// </summary>
        public float Gravity { get; set; }

        /// <summary>
        /// Gets or sets the current network controller
        /// </summary>
        protected NetworkController _net { get; set; }
        #endregion

        #region "methods"
        /// <summary>
        /// Move spawnable (apply gravity)
        /// </summary>
        public abstract void Move();

        /// <summary>
        /// Unity's Awake() method.
        /// </summary>
        public virtual void Awake()
        {
            _net = GameObject.Find("Network").GetComponent<NetworkController>();
            Gravity = 77f;
        }

        /// <summary>
        /// Spawn spawnable at given position
        /// </summary>
        /// <param name="position">the position where to spawn</param>
        public virtual void Spawn(Vector3 position)
        {
            gameObject.transform.position = position;
        }

        /// <summary>
        /// Returns a Clone of this item.
        /// </summary>
        /// <returns>the ISpawnable component of instantiated clone</returns>
        public AbstractSpawnable Clone()
        {
            return (AbstractSpawnable)Instantiate(this, new Vector3(-1000f, -1000f, -1000f), transform.rotation);
        }
        #endregion
    }
}

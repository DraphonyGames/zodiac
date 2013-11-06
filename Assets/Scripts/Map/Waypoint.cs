namespace Assets.Scripts.Map
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// The waypoint in sections for ai
    /// </summary>
    public class Waypoint : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets the team no fo the ai
        /// </summary>
        public int TeamNo { get; set; }

        /// <summary>
        /// Gets or sets the sorting in the section
        /// </summary>
        public int Sort { get; set; }
    }
}

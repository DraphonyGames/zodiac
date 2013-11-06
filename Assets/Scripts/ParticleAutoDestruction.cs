namespace Assets.Scripts
{
    using UnityEngine;

    /// <summary>
    /// Script for destroying particle effects when they're finished.
    /// </summary>
    public class ParticleAutoDestruction : MonoBehaviour
    {
        /// <summary>
        /// the particle system of the gameobject
        /// </summary>
        private ParticleSystem _ps;

        /// <summary>
        /// called on instantiation, sets the particle system
        /// </summary>
        public void Start()
        {
            _ps = GetComponent<ParticleSystem>();
        }

        /// <summary>
        /// check if the particle effect is finished, then destroy
        /// </summary>
        public void Update()
        {
            if (_ps != null)
            {
                if (!_ps.IsAlive())
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
namespace Assets.Scripts.AIHelper
{
    using UnityEngine;

    /// <summary>
    /// The DistanceHolder is used to hold the right distance between AI and target.
    /// This object always moves between the two points (AI and target position)
    /// </summary>
    public class DistanceHolder : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets the distance the AI is to keep to target
        /// </summary>
        public float DistanceToTarget;

        /// <summary>
        /// Gets or sets the AI to calculate the target for
        /// </summary>
        public Transform AI;

        /// <summary>
        /// Gets or sets the target the AI is going to aim
        /// </summary>
        public Transform Target;

        /// <summary>
        /// Gets or sets a value indicating whether target position is randomised
        /// </summary>
        public bool Randomize;

        /// <summary>
        /// Gets or sets distance bevore Update
        /// </summary>
        private float _oldDistance;

        /// <summary>
        /// random vector for randomOption
        /// </summary>
        private Vector3 _randomVector;

        #region "methods"
        /// <summary>
        /// Returns the point as vector between a and b with an max distance to point b.
        /// </summary>
        /// <param name="a">first vector</param>
        /// <param name="b">second vector</param>
        /// <param name="d">maximum distance to second vector</param>
        /// <returns>Vector point between a and b</returns>
        public static Vector3 Between(Vector3 a, Vector3 b, float d)
        {
            // distance between a and b
            float l = Vector3.Distance(a, b);

            // scale factor for vector a - b
            float x;

            // distance to b can't be bigger then the hole distance
            // the maximum distance would be l
            if (d >= l)
            {
                return a;
            }
            
            x = 1 - (d / l);

            // normal vector addition with scale factor x
            // if d would be half of l, x would be 0.5
            // if x is 0 this would be a, so we have the short cut above
            return a + ((b - a) * x);
        }

        /// <summary>
        /// initialized the values for the distance holder
        /// </summary>
        public void Start()
        {
            _randomVector = Random.insideUnitSphere;
            _randomVector.y = 0;
            _randomVector.Normalize();
        }

        /// <summary>
        /// Recalcutate position.
        /// </summary>
        public void Update()
        {
            RemoveOnDeath();

            CalculatePosition();
        }

        /// <summary>
        /// Moves the distance holder 
        /// </summary>
        /// <returns>Chaining object this</returns>
        public DistanceHolder CalculatePosition()
        {
            if (Target != null && AI != null)
            {
                transform.rotation.SetLookRotation(AI.position);

                if (Randomize)
                {
                    transform.position = Target.position + (_randomVector * DistanceToTarget);
                }
                else
                {
                    transform.position = Between(AI.position, Target.position, DistanceToTarget);
                }
            }

            return this;
        }

        /// <summary>
        /// Destroys this game object if either the ai or the target is dead
        /// </summary>
        public void RemoveOnDeath()
        {
            if (AI == null || AI.GetComponent<Character>().IsDead())
            {
                Destroy(gameObject);
            }
        }
        #endregion
    }
}

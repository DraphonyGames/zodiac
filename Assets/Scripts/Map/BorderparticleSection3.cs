namespace Assets.Scripts.Map
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Class to change the color of the particles.
    /// </summary>
    public class BorderparticleSection3 : MonoBehaviour
    {
        /// <summary>
        /// Variable for own GameManager instance
        /// </summary>
        private GameManager _gameManager;

        /// <summary>
        /// Loads the GameManager singleton instance.
        /// </summary>
        public void Start()
        {
            _gameManager = GameManager.GetInstance();
        }

        /// <summary>
        /// Unity called Update function.
        /// </summary>
        /// <remarks>
        /// Checks whether the particles should be red or green.
        /// </remarks>
        public void Update()
        {
            if (_gameManager.CurrentSection.RemainingMobs() == 0)
            {
                if (_gameManager.SectionNo == 1)
                {
                    _gameManager.PlayFirstTrigger = true;
                }

                if (_gameManager.SectionNo == 2)
                {
                    _gameManager.PlaySecondTrigger = true;
                }

                if (_gameManager.SectionNo == 3)
                {
                    _gameManager.PlayThirdTrigger = true;
                }

                if (_gameManager.PlayThirdTrigger)
                {
                    this.particleSystem.startColor = Color.green;
                }
            }
        }
    }
}

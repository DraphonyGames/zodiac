namespace Assets.Scripts
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Represents the destroyed version of a destructable
    /// </summary>
    public class DestroyedDestructable : MonoBehaviour
    {
        /// <summary>
        /// The Clip which will be played
        /// </summary>
        public AudioClip Clip;

        /// <summary>
        /// The Audio Control Component, which plays the sound
        /// </summary>
        private AudioSource _audioSource;

        /// <summary>
        /// Will be called, when the object is in the game
        /// </summary>
        public void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = Clip;
            _audioSource.Play();
            _audioSource.volume = ConfigManager.GetInstance().SoundLevel / 100;
        }
    }
}

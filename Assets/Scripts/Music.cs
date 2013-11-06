namespace Assets.Scripts
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Audiosound setting class.
    /// </summary>
    public class Music : MonoBehaviour
    {
        /// <summary>
        /// Stores the audiocomponent.
        /// </summary>
        private AudioSource _audio;

        /// <summary>
        /// Initializes the defaultvalues and loads the audio component.
        /// </summary>
        public void Start()
        {
            _audio = GetComponent<AudioSource>();
            _audio.volume = 0.4f * (ConfigManager.GetInstance().MusicLevel / 100);
        }
    }
}
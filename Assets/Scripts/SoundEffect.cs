namespace Assets.Scripts
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Script-Class for adding a soundeffect to an asset in scenes.
    /// </summary>
    public class SoundEffect : MonoBehaviour
    {
        private AudioClip _clip;
        private AudioSource _audio;
        private bool _ifPlay;

        /// <summary>
        /// Gets or sets AudioClip attached to soundeffect script 
        /// </summary>
        public AudioClip Clip
        {
            get
            {
                return _clip;
            }

            set
            {
                _clip = value;
            }
        }

        /// <summary>
        ///  Use this for initialization
        /// </summary>
        public void Start()
        {
            _audio = GetComponent<AudioSource>();
        }

        /// <summary>
        ///  Update is called once per frame
        /// </summary>
        public void Update()
        {
            if (_ifPlay)
            {
                _audio.clip = Clip;
                _audio.Play();
                _audio.volume = 0.7f * (ConfigManager.GetInstance().SoundLevel / 100);
                _ifPlay = false;
            }
        }

        /// <summary>
        ///  Called when the collider of the gameobject, to which the script is attached, is entered
        /// </summary>
        /// <param name="other">
        /// collider which entered
        /// </param>
        public void OnTriggerEnter(Collider other)
        {
            _ifPlay = true;
        }
    }
}

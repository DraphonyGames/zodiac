namespace Assets.Scripts.Sounds
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Manager for backgroundmusic. It's stays alive when the sections get changed.
    /// </summary>
    public sealed class MusicManager : MonoBehaviour
    {
        /// <summary>
        /// The only instance of this singleton.
        /// </summary>
        private static MusicManager _instance;

        /// <summary>
        /// Get an instance of this class (singleton).
        /// </summary>
        /// <returns>The only GameManager instance.</returns>
        public static MusicManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new MusicManager();
            }

            return _instance;
        }

        /// <summary>
        /// Prevent it's destroying on load.
        /// </summary>
        public void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            else
            {
                _instance = this;
            }

            DontDestroyOnLoad(this.gameObject);
        }
    }
}

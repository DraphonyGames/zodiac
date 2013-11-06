namespace Assets.Scripts.Sounds
{
    using System.Collections;
    using UnityEngine;
    
    /// <summary>
    /// Class to play our imported music.
    /// </summary>
    public class MusicPlayer : MonoBehaviour
    {
        /// <summary>
        /// The audiosource dragged to.
        /// </summary>
        public AudioClip Clip;

        /// <summary>
        /// Plays the Music dragged to
        /// </summary>
        public void Awake()
        {
            GameObject go = GameObject.Find("GameMusic"); // Finds the game object called Game Music, if it goes by a different name, change this.
            go.audio.clip = Clip; // Replaces the old audio with the new one set in the inspector.
            go.audio.volume = ConfigManager.GetInstance().MusicLevel / 100.0f;
            if (!go.audio.isPlaying)
            {
                go.audio.Play(); // Plays the audio.
            }

            go.audio.loop = true;
        }
    }
}

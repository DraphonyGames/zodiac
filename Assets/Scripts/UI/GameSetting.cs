namespace Assets.Scripts.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;    
    using UnityEngine;

    /// <summary>
    /// Class to set the playmode preferences.
    /// </summary>
    public class GameSetting : MonoBehaviour
    {
        /// <summary>
        /// Variable for custom GUI skin
        /// </summary>
        public GUISkin Myskin;

        /// <summary>
        /// Variable for custom alternate GUI skin
        /// </summary>
        public GUISkin Myskin2;

        private AudioSource _aSource;

        private float _sfxLevelOld;
        private float _sfxLevelNew;
        private float _musicLevel;
        private float _resolution;
        private float _quality;
        private float _awesomeness;
        private float _awesomenessMin;
        private List<string> _awesomeList;
        private bool _mouseClickedInPast;

        private enum Quality
        {
            Performance, Balanced, Awesome
        }

        private enum Resolution
        {
            _1280X720, _1366X768, _1600X900, _1920X1080
        }

        /// <summary>
        /// Initialising and set the default values.
        /// </summary>
        public void Start()
        {
            _awesomeList = new List<string>();
            TextAsset data = Resources.Load("Other/awesome") as TextAsset;

            StringReader reader = new StringReader(data.text);
            if (reader == null)
            {
                Debug.Log("puzzles.txt not found or not readable");
            }
            else
            {
                string text;
                while ((text = reader.ReadLine()) != null)
                {
                    _awesomeList.Add(text);
                }
            }

            _awesomeness = _awesomenessMin = GameManager.GetInstance().Awesomeness;
            _sfxLevelOld = _sfxLevelNew = ConfigManager.GetInstance().SoundLevel;
            _musicLevel = ConfigManager.GetInstance().MusicLevel;
            _resolution = 3.0f;
            _quality = 1.0f;
        }

        /// <summary>
        /// Prints the Play setting menu and the connected screen from 
        /// </summary>
        public void OnGUI()
        {
            GUI.skin = Myskin;
            float boxHeight = Screen.height / 10;
            float boxWidth = Screen.width / 6;
            float buttonWidth = Screen.width / 9;
            float sliderWidth = Screen.width / 3;
            float buttonHeight = Screen.height / 10;
            float offsetAudioHeight = Screen.height / 8;
            float offsetAudioWidth = Screen.width / 3;
            float offsetVideo = 3 * Screen.height / 5;
            float offsetVideoWidth = Screen.width / 6;

            ////// Audio Options //////
            GUI.Box(new Rect(offsetAudioWidth, offsetAudioHeight, 2 * boxWidth, boxHeight), "Audio and Stuff");

            // Sound Effect box
            GUI.Button(new Rect(offsetAudioWidth, offsetAudioHeight + (boxHeight / 2), boxWidth, boxHeight), "Sound Effects");

            // Sound Effect slider
            _sfxLevelNew = Mathf.Round(GUIOperations.LabeledSlider(new Rect(offsetAudioWidth + boxWidth, offsetAudioHeight + (boxHeight / 2), sliderWidth, boxHeight), string.Empty, _sfxLevelOld, 0, 100));

            // Sound Effect info box
            GUI.Button(new Rect(offsetAudioWidth + (2 * boxWidth / 3) + sliderWidth, offsetAudioHeight + (boxHeight / 2), boxWidth, boxHeight), _sfxLevelNew + "%");

            if (_sfxLevelNew != _sfxLevelOld)
            {
                ConfigManager.GetInstance().SoundLevel = _sfxLevelNew;
                _sfxLevelOld = _sfxLevelNew;
                _aSource = GetComponents<AudioSource>()[0];
                _aSource.volume = ConfigManager.GetInstance().SoundLevel / 100;
                if (!_aSource.isPlaying)
                {
                    _aSource.Play();
                }
            }

            // Music box
            GUI.Button(new Rect(offsetAudioWidth, offsetAudioHeight + (5 * boxHeight / 4), boxWidth, boxHeight), "Music");

            // Music slider
            _musicLevel = Mathf.Round(GUIOperations.LabeledSlider(new Rect(offsetAudioWidth + boxWidth, offsetAudioHeight + (5 * boxHeight / 4), sliderWidth, boxHeight), string.Empty, _musicLevel, 0, 100));
            ConfigManager.GetInstance().MusicLevel = _musicLevel;
            GameObject go = GameObject.Find("GameMusic"); // Finds the game object called Game Music, if it goes by a different name, change this.
            go.audio.volume = ConfigManager.GetInstance().MusicLevel / 100.0f;

            // Sound Effect info box
            GUI.Button(new Rect(offsetAudioWidth + (2 * boxWidth / 3) + sliderWidth, offsetAudioHeight + (5 * boxHeight / 4), boxWidth, boxHeight), _musicLevel + "%");

            // Awesomeness box
            GUI.Button(new Rect(offsetAudioWidth, offsetAudioHeight + (8 * boxHeight / 4), boxWidth, boxHeight), "Awesomeness");

            // Awesomeness slider
            _awesomeness = Mathf.Round(GUIOperations.LabeledSlider(new Rect(offsetAudioWidth + boxWidth, offsetAudioHeight + (8 * boxHeight / 4), sliderWidth, boxHeight), string.Empty, _awesomenessMin, 42, 10042));
            if (_awesomenessMin > _awesomeness)
            {
                _awesomeness = _awesomenessMin;
            }
            else
            {
                _awesomenessMin = _awesomeness;
            }

            GameManager.GetInstance().Awesomeness = _awesomeness;

            // Awesomeness info box
            GUI.Button(new Rect(offsetAudioWidth + (3 * boxWidth / 4) + sliderWidth, offsetAudioHeight + (8 * boxHeight / 4), boxWidth, boxHeight), _awesomeList[(int)((_awesomeness / 10042.0f) * (_awesomeList.Count - 1))]);

            /*
            GUIVideoOptions(offsetVideoWidth, offsetVideo, boxWidth, boxHeight);

            //Buttons for applying video settings, switching to key bindings or main menu
            

            if (GUI.Button(new Rect(offsetVideoWidth + 3 * boxWidth / 2, offsetVideo + 12 * boxHeight / 5, 3 * boxWidth / 4, 3 * boxHeight / 4), "Apply"))
            {
                //Change Resolutions
                GameManager.GetInstance().VideoQuality = (int)_quality;
                GameManager.GetInstance().Resolution = (int)_resolution;
            }
            */
            GUI.skin = Myskin2;
            float fontSize = 0.03f * Screen.height;
            GUI.skin.button.fontSize = (int)fontSize;
            if (GUI.Button(new Rect((Screen.width / 6) - (7 * buttonWidth / 5), Screen.height - (Screen.height * (3.5f / 14)), 7 * buttonWidth / 5, buttonHeight), "Main Menu"))
            {
                // TODO: Error checking!
                ConfigManager.GetInstance().SaveConfig();
                Application.LoadLevel("MainMenu");
            }

            if (GUI.Button(new Rect(Screen.width - (Screen.width / 6), Screen.height - (Screen.height * (3.5f / 14)), 7 * buttonWidth / 5, buttonHeight), "Key Bindings"))
            {
                Application.LoadLevel("KeySettings");
            }
        }

        /// <summary>
        /// Draw all Video options on GUI Screen
        /// </summary>
        /// <param name="offsetVideoWidth">Top point of the Video box</param>
        /// <param name="offsetVideo">Left point of the Video box</param>
        /// <param name="boxWidth">width of the video box</param>
        /// <param name="boxHeight">height of the video box</param>
        protected void GUIVideoOptions(float offsetVideoWidth, float offsetVideo, float boxWidth, float boxHeight)
        {
            ////// Video Options //////
            GUI.Box(new Rect(offsetVideoWidth, offsetVideo, boxWidth, boxHeight), "Video");

            // Resolution box
            GUI.Button(new Rect(offsetVideoWidth, offsetVideo + (boxHeight / 2), boxWidth, boxHeight), "Resolution");

            // Resolution slider
            _resolution = Mathf.Round(GUIOperations.LabeledSlider(new Rect(offsetVideoWidth + boxWidth, offsetVideo + (boxHeight / 2), boxWidth, boxHeight), string.Empty, _resolution, 0, 3));
            GUI.Button(new Rect(offsetVideoWidth + (19 * boxWidth / 10), offsetVideo + (boxHeight / 2), boxWidth, boxHeight), Enum.GetNames(typeof(Resolution))[(int)_resolution].Substring(1));

            // Quality box
            GUI.Button(new Rect(offsetVideoWidth, offsetVideo + (5 * boxHeight / 4), boxWidth, boxHeight), "Quality");

            // Quality slider
            _quality = Mathf.Round(GUIOperations.LabeledSlider(new Rect(offsetVideoWidth + boxWidth, offsetVideo + (5 * boxHeight / 4), boxWidth, boxHeight), string.Empty, _quality, 0, 2));
            GUI.Button(new Rect(offsetVideoWidth + (19 * boxWidth / 10), offsetVideo + (5 * boxHeight / 4), boxWidth, boxHeight), Enum.GetNames(typeof(Quality))[(int)_quality]);
            //////////////////////////
        }
    }
}

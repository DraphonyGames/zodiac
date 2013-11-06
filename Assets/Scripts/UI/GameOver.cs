namespace Assets.Scripts.UI
{
    using UnityEngine;

    /// <summary>
    /// Class for Game Over Screen which shows that you lost the Game.
    /// </summary>
    public class GameOver : MonoBehaviour
    {
        private GUISkin _mySkin;

        /// <summary>
        /// Called on Instantiation
        /// Skin is assigned.
        /// </summary>
        public void Start()
        {
            _mySkin = Resources.Load("GUI/zodiac") as GUISkin;
        }

        /// <summary>
        /// Draws the GUI elements
        /// </summary>
        public void OnGUI()
        {
            GUI.skin = _mySkin;
            if (GUIOperations.DrawButtonCenteredAt(11 * Screen.width / 16, 7 * Screen.height / 16, Screen.width / 6, Screen.height / 15, 1f, 0.7f, "Play again"))
            {
                Application.LoadLevel("GameSelection");
            }

            if (GUIOperations.DrawButtonCenteredAt(13 * Screen.width / 16, 9 * Screen.height / 16, Screen.width / 6, Screen.height / 15, 1f, 0.7f, "Main Menu"))
            {
                Application.LoadLevel("MainMenu");
            }

            if (GUIOperations.DrawButtonCenteredAt(35 * Screen.width / 64, 13 * Screen.height / 16, Screen.width / 6, Screen.height / 15, 1f, 0.7f, "Quit"))
            {
                Application.Quit();
            }
        }

        /// <summary>
        /// Called when this GameObject is destroyed by Unity.
        /// </summary>
        public void OnDestroy()
        {
            // Make sure we destroy all the immortal team members (we needed them on this screen
            // to display stats etc.).
            GameManager.GetInstance().CleanupTeamMembers();
        }
    }
}

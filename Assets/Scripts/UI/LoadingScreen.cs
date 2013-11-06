namespace Assets.Scripts.UI
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Scenescript that shows a Loading Screen.
    /// </summary>
    public class LoadingScreen : MonoBehaviour
    {
        /// <summary>
        /// Used Gui skin
        /// </summary>
        private GUISkin _zodiacStyle;

        private string _sceneToBeLoaded;

        /// <summary>
        /// Executed on start, sets the GUIStyle.
        /// </summary>
        public void Start()
        {
            _zodiacStyle = Resources.Load("GUI/zodiac") as GUISkin;
            _sceneToBeLoaded = GameManager.GetInstance().SceneToBeLoaded;
            StartCoroutine(LoadLevel());
        }

        /// <summary>
        /// Draws the Loading Screen
        /// </summary>
        public void OnGUI()
        {
            GUI.skin = _zodiacStyle;
            GUIOperations.DrawLabelCenteredAt(Screen.width / 2, Screen.height * 5 / 6, (int)(0.1f * Screen.width), "Loading");
            int top = (int)(Screen.height * 0.15f);
            int topoffset = (int)(0.05f * Screen.height);
            int left;
            int leftoffset = Screen.width / 3;
            if (GameManager.GetInstance().PlayerCount == 1)
            {
                left = Screen.width / 2;
            }
            else
            {
                left = Screen.width / 3;
            }

            for (int i = 1; i <= GameManager.GetInstance().PlayerCount; i++)
            {
                ControlKeysManager c = ConfigManager.GetInstance().GetControlKeysForPlayer(i);
                GUIOperations.DrawLabelCenteredAt(left, top - topoffset, (int)(0.04f * Screen.width), "Player " + i);
                GUIOperations.DrawLabelCenteredAt(left, top + topoffset, (int)(0.02f * Screen.width), "Walk up: " + ConfigManager.KeyToString(c.ForwardKey));
                GUIOperations.DrawLabelCenteredAt(left, top + (2 * topoffset), (int)(0.02f * Screen.width), "Walk down: " + ConfigManager.KeyToString(c.BackwardKey));
                GUIOperations.DrawLabelCenteredAt(left, top + (3 * topoffset), (int)(0.02f * Screen.width), "Walk left: " + ConfigManager.KeyToString(c.LeftKey));
                GUIOperations.DrawLabelCenteredAt(left, top + (4 * topoffset), (int)(0.02f * Screen.width), "Walk right: " + ConfigManager.KeyToString(c.RightKey));
                GUIOperations.DrawLabelCenteredAt(left, top + (5 * topoffset), (int)(0.02f * Screen.width), "Attack: " + ConfigManager.KeyToString(c.AttackKey));
                GUIOperations.DrawLabelCenteredAt(left, top + (6 * topoffset), (int)(0.02f * Screen.width), "Defend: " + ConfigManager.KeyToString(c.DefendKey));
                GUIOperations.DrawLabelCenteredAt(left, top + (7 * topoffset), (int)(0.02f * Screen.width), "Jump: " + ConfigManager.KeyToString(c.JumpKey));
                GUIOperations.DrawLabelCenteredAt(left, top + (8 * topoffset), (int)(0.02f * Screen.width), "Pickup & Use: " + ConfigManager.KeyToString(c.PickupKey));
                GUIOperations.DrawLabelCenteredAt(left, top + (9 * topoffset), (int)(0.02f * Screen.width), "Drop: " + ConfigManager.KeyToString(c.DropItemKey));
                left += leftoffset;
            }
        }

        private IEnumerator LoadLevel()
        {
            yield return new WaitForSeconds(0);
            Application.LoadLevel(_sceneToBeLoaded);
        }
    }
}
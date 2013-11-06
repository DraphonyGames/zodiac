namespace Assets.Scripts
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Implements custom input handling.
    /// </summary>
    public class CustomInput
    {
        #region public static methods
        /// <summary>
        /// Get the value of a player's input axis.
        /// </summary>
        /// <param name="playerNo">Player number</param>
        /// <param name="axisName">Name of axis ("horizontal" or "vertical")</param>
        /// <returns>Value of requested axis (0 means currently no input or unknown axis specified)</returns>
        public static float GetAxis(int playerNo, string axisName)
        {
            float value = 0;
            switch (axisName.ToLower())
            {
                case "vertical":
                    value += Input.GetKey(ConfigManager.GetInstance().GetControlKeysForPlayer(playerNo).ForwardKey) ? 1 : 0;
                    value -= Input.GetKey(ConfigManager.GetInstance().GetControlKeysForPlayer(playerNo).BackwardKey) ? 1 : 0;
                    if (value == 0)
                    {
                        value = Input.GetAxis("VerticalJoystick" + playerNo);
                    }

                    break;

                case "horizontal":
                    value += Input.GetKey(ConfigManager.GetInstance().GetControlKeysForPlayer(playerNo).RightKey) ? 1 : 0;
                    value -= Input.GetKey(ConfigManager.GetInstance().GetControlKeysForPlayer(playerNo).LeftKey) ? 1 : 0;
                    if (value == 0)
                    {
                        value = Input.GetAxis("HorizontalJoystick" + playerNo);
                    }

                    break;
            }

            return value;
        }
        #endregion
    }
}

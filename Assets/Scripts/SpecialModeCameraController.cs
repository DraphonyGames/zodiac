namespace Assets.Scripts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    /// <summary>
    /// Controls the position of the camera.
    /// </summary>
    public class SpecialModeCameraController
    {
        // public static float MinX = float.MaxValue;
        // public static float MaxX = float.MinValue;

        /// <summary>
        /// If i'm right there always a MAXIMUM of 16 Players playing zodiac together.
        /// </summary>
        public static int MAXPLAYER = 16;

        private static NetworkController _net = GameManager.GetInstance().NetworkController;

        /// <summary>
        /// Calculate Cameraposition
        /// </summary>
        /// <returns>new calculated position</returns>
        public static Vector3 CameraPosition()
        {
            List<GameObject> playerList = new List<GameObject>();
            GameObject player;

            if (_net.IsServer)
            {
                player = GameObject.Find("HeroServer");
            }
            else if (_net.IsClient)
            {
                player = GameObject.Find("HeroClient" + _net.ClientNumber);
            }
            else
            {
                player = GameObject.Find("Hero1");
            }

            if (player != null)
            {
                playerList.Add(player);
            }

            if (playerList.Count == 0)
            {
                return Vector3.zero;
            }

            float x, y, z; // new camera position

            // calculate position
            float minX = float.MaxValue, maxX = float.MinValue;
            float minY = float.MaxValue, maxY = float.MinValue;
            float minZ = float.MaxValue;
            foreach (GameObject obj in playerList)
            {
                minX = Math.Min(minX, obj.transform.position.x);
                minY = Math.Min(minY, obj.transform.position.y);
                minZ = Math.Min(minZ, obj.transform.position.z);

                maxX = Math.Max(maxX, obj.transform.position.x);
                maxY = Math.Max(maxY, obj.transform.position.y);
            }

            // calculate zoom
            // float zoom = ((maxX - minX) > 10) ? (maxX / minX) : 0;
            float zoom = (maxX - minX) / 2;

            // new camera position values
            x = (minX + maxX) / 2;
            y = (minY + maxY + zoom) / 2;
            z = minZ - zoom;

            return new Vector3(x, y, z);
        }
    }
}

namespace Assets.Scripts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    /// <summary>
    /// Calculate the Camera Position
    /// </summary>
    public class CameraController
    {
        /// <summary>
        /// If i'm right there always a MAXIMUM of 16 Players playing zodiac together.
        /// </summary>
        private static int MAXPLAYER = 16;

        /// <summary>
        /// return the new CameraPosition
        /// </summary>
        /// <returns>the new Camera position</returns>
        public static Vector3 CameraPosition()
        {
            List<GameObject> playerList = new List<GameObject>();
            GameObject player;
            for (int i = 0; i < MAXPLAYER; i++)
            {
                player = GameObject.Find("Hero" + (i + 1));
                if (player != null)
                {
                    playerList.Add(player);
                }
            }

            if (playerList.Count == 0)
            {
                // no heros? fokus on mercTeamMembers
                if (GameManager.GetInstance().GameMode == GameManager.Mode.PLAY)
                {
                    foreach (Character c in GameManager.GetInstance().CurrentSection.GetFreeMercs())
                    {
                        if (c.Team.TeamNo != GameManager.MOBTEAMNO)
                        {
                            playerList.Add(c.gameObject);
                        }
                    }
                }

                // no one left? retun zero
                if (playerList.Count == 0)
                {
                    return Vector3.zero;
                }
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
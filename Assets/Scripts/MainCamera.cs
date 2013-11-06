namespace Assets.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    /// <summary>
    /// This Class moves the MainCamera
    /// </summary>
    public class MainCamera : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets the Speed the Camera moves with
        /// </summary>
        public float CameraSpeed;
        /* {
             get;
             set;
         }*/

        /// <summary>
        /// Gets or sets the Z Distance to the player
        /// </summary>
        public float CameraDistance;
        /* {
             get;
             set;
         */

        /// <summary>
        /// is called on Startup, initializes the ProjectilePool and calls the levelBuilder
        /// </summary>
        public void Start()
        {
            ProjectilePool.PoolSize = 100;
            ProjectilePool.GeneratePool();
            LevelGenerator generator = new LevelGenerator();

            GameObject levelBuilderGameObj = GameObject.Find("LevelBuilder");
            if (levelBuilderGameObj != null)
            {
                levelBuilderGameObj.GetComponent<LevelBuilder>().Init();
                levelBuilderGameObj.GetComponent<LevelBuilder>().BuildLevel(generator.GenerateNextLevel(2));
            }
        }

        /// <summary>
        /// Moves the Camera According to the CameraControllers returnValue, closes the game when ESP is pressed
        /// </summary>
        public void Update()
        {
            Movement(CameraController.CameraPosition());

            if (ControlKeysManager.GetKeyDown(KeyCode.Escape))
            {
                Application.LoadLevel("MainMenu");
            }
        }

        /// <summary>
        /// Moves the Camera in relation to Players
        /// </summary>
        /// <param name="position">Object camera moves towards</param>
        private void Movement(Vector3 position)
        {
            double angle = transform.rotation.eulerAngles.x / 180 * Math.PI;
            float y = (float)Math.Sin(angle) * CameraDistance;
            float z = (float)Math.Cos(angle) * CameraDistance;
            transform.position = new Vector3(position.x, position.y + y, position.z - z);
        }
    }
}

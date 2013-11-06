namespace Assets.Scripts
{
    using UnityEngine;

    /// <summary>
    /// Preloaded pool of Projectiles to minimize instantiations in gametime.
    /// </summary>
    public class ProjectilePool
    {
        #region "global properties"
        /// <summary>
        /// Pool size for projectiles
        /// minimun size is 5
        /// </summary>
        public static int PoolSize;

        /// <summary>
        /// all projectiles (cached)
        /// </summary>
        private static Projectile[] _pool;

        /// <summary>
        /// where to hide inactive projectiles
        /// </summary>
        private static Vector3 _invisiblePoint = new Vector3(-1000f, -1000f, -1000f);

        /// <summary>
        /// NetworkController which holds connections logics and information
        /// </summary>
        private static NetworkController _net = GameObject.Find("Network").GetComponent<NetworkController>();

        /// <summary>
        /// Gets or sets the current position inside the pool
        /// </summary>
        public static int CurrentPosition { get; set; }
        #endregion

        #region "Methods"
        /// <summary>
        /// instatiate all projectiles in pool
        /// </summary>
        public static void GeneratePool()
        {
            if (!_net.IsClient)
            {
                GameObject projectilePrefab = (GameObject)Resources.Load("ProjectilePrefab");

                if (PoolSize < 5)
                {
                    PoolSize = 5;
                }

                _pool = new Projectile[PoolSize];
                CurrentPosition = 0;
                for (int i = 0; i < PoolSize; i++)
                {
                    // using modified _invisiblePoint to prevent OnTriggerEnter get triggered 'n!' often.
                    if (_net.IsServer)
                    {
                        _pool[i] = ((GameObject)Network.Instantiate(projectilePrefab, new Vector3(_invisiblePoint.x * i, _invisiblePoint.y, _invisiblePoint.z), Quaternion.identity, 0)).GetComponent<Projectile>();
                    }
                    else
                    {
                        _pool[i] = ((GameObject)UnityEngine.Object.Instantiate(projectilePrefab, new Vector3(_invisiblePoint.x * i, _invisiblePoint.y, _invisiblePoint.z), Quaternion.identity)).GetComponent<Projectile>();
                    }
                }
            }
        }

        /// <summary>
        /// get the next projectile
        /// spawns at 0, 0, 0 with no rotation
        /// </summary>
        /// <returns>one projectile of this projectilepool</returns>
        public static Projectile GetProjectile()
        {
            return GetProjectile(Vector3.zero, Quaternion.identity);
        }

        /// <summary>
        /// spawn projectile at an specific point with a given rotation
        /// </summary>
        /// <param name="position">spawn point</param>
        /// <param name="rotation">spawn direction</param>
        /// <returns> one projectile of this projectilepool with an given rotation ans startpoint</returns>
        public static Projectile GetProjectile(Vector3 position, Quaternion rotation)
        {
            Projectile projectile = _pool[CurrentPosition++ % PoolSize];
            if (_net.IsServer || _net.IsClient)
            {
                projectile.SyncProjectilePool();
            }

            projectile.gameObject.transform.position = position;
            projectile.gameObject.transform.rotation = rotation;
            return projectile;
        }

        /// <summary>
        /// move projectile back to the invisible point
        /// remember: this function always reset the projectile
        /// </summary>
        /// <param name="projectile">the projectile to "remove"</param>
        public static void RemoveProjectile(Projectile projectile)
        {
            if (_net.IsServer || _net.IsClient)
            {
                projectile.NetworkReset();
            }
            else
            {
                projectile.Reset();
            }

            projectile.gameObject.transform.position = _invisiblePoint;
        }
        #endregion
    }
}
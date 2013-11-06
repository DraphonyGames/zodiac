namespace Assets.Scripts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    /// <summary>
    /// This Class keeps all Heroes, Mobs, Mercaneries and Items that are in the Resources Folder
    /// </summary>
    public sealed class Datasheet
    {
        /// <summary>
        /// the list of all Heros in the Resources/Heroes Folder
        /// </summary>
        private static List<GameObject> _heroList;

        /// <summary>
        /// the list of all Mobs in the Resources/Mobs Folder
        /// </summary>
        private static List<GameObject> _mobList;

        /// <summary>
        /// the list of all Mercenaries in the Resources/Mercs Folder
        /// </summary>
        private static List<GameObject> _mercenaryList;

        /// <summary>
        /// the list of all Items in the Resources/Items Folder
        /// </summary>
        private static List<AbstractSpawnable> _itemList;

        /// <summary>
        /// Finds all Heroes in the Resources Folder
        /// </summary>
        /// <returns>all Heros in the Resources Folder</returns>
        public static List<GameObject> Heroes()
        {
            if (_heroList == null)
            {
                _heroList = new List<GameObject>();

                foreach (UnityEngine.Object obj in Resources.LoadAll("Heroes"))
                {
                    if (obj is GameObject)
                    {
                        _heroList.Add((GameObject)obj);
                    }
                }
            }

            return _heroList;
        }

        /// <summary>
        /// Finds all Mobss in the Resources Folder
        /// </summary>
        /// <returns>all Mobs in the Resources Folder</returns>
        public static List<GameObject> Mobs()
        {
            if (_mobList == null)
            {
                _mobList = new List<GameObject>();

                foreach (UnityEngine.Object obj in Resources.LoadAll("Mobs"))
                {
                    if (obj is GameObject)
                    {
                        _mobList.Add((GameObject)obj);
                    }
                }
            }

            return _mobList;
        }

        /// <summary>
        /// Finds all Mercs in the Resources Folder
        /// </summary>
        /// <returns>all Mercs in the Resources Folder</returns>
        public static List<GameObject> Mercenaries()
        {
            if (_mercenaryList == null)
            {
                _mercenaryList = new List<GameObject>();

                foreach (UnityEngine.Object obj in Resources.LoadAll("Mercenaries"))
                {
                    if (obj is GameObject)
                    {
                        _mercenaryList.Add((GameObject)obj);
                    }
                }
            }

            return _mercenaryList;
        }

        /// <summary>
        /// Finds all Items in the Resources Folder
        /// </summary>
        /// <returns>all Items in the Resources Folder</returns>
        public static List<AbstractSpawnable> Items()
        {
            if (_itemList == null)
            {
                _itemList = new List<AbstractSpawnable>();
                foreach (UnityEngine.Object obj in Resources.LoadAll("Items"))
                {
                    if (obj is GameObject)
                    {
                        _itemList.Add(((GameObject)obj).GetComponent<Item>());
                    }
                }
            }

            return _itemList;
        }
    }
}

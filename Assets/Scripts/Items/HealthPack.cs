namespace Assets.Scripts.Items
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    /// <summary>
    /// An item that adds an fixed amount of HP
    /// </summary>
    public class HealthPack : Item
    {
        /// <summary>
        /// the HPs to be added
        /// </summary>
        private int _addHP = 250;

        /// <summary>
        /// The HPs are added when Use() is called
        /// </summary>
        /// <returns> the Time that the item is in use</returns>
        public override int Use()
        {
            base.Use();
            ItemInUse = true;
            Owner.AddHealth(_addHP, true);
            if (_net.IsServer || _net.IsClient)
            {
                Network.Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            return Time;
        }
    }
}

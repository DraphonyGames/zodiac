namespace Assets.Scripts.Items
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    /// <summary>
    /// An Item that adds a fixed amount of MP
    /// </summary>
    public class ManaPack : Item
    {
        /// <summary>
        /// the amount of MP that is added
        /// </summary>
        private float _addMP = 100;

        /// <summary>
        /// The MPs are added here
        /// </summary>
        /// <returns>Time the item was in use</returns>
        public override int Use()
        {
            base.Use();
            ItemInUse = true;
            Owner.MP += _addMP;

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

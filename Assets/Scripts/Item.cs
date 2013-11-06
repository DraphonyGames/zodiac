namespace Assets.Scripts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    /// <summary>
    /// represents an Item
    /// </summary>
    public abstract class Item : AbstractSpawnable
    {
        /// <summary>
        /// The TextureAssocianted with the Item
        /// </summary>
        public Texture2D ItemTexture;

        /// <summary>
        /// Is this item in use?
        /// </summary>
        public bool ItemInUse;

        /// <summary>
        /// Gets or sets the time the Item is active
        /// </summary>
        public int Time
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Character that holds the item
        /// </summary>
        public Character Owner
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Character that has reserved this item for pickup.
        /// </summary>
        /// <remarks>
        /// This is currently used in <see cref="Character.GetHitByItem"/> to prevent
        /// AIs from trying to pick up the same item.
        /// </remarks>
        public Character ReservingPlayer
        {
            get;
            set;
        }

        /// <summary>
        /// Starts this class with ItemInUse = false
        /// </summary>
        public void Start()
        {
            ItemInUse = false;
        }

        /// <summary>
        /// Move this entity.
        /// This is a no-op. Items cannot and should not move by their own.
        /// </summary>
        public override void Move()
        {
            // Do not do anything. Items cannot move. They are dead.
        }

        /// <summary>
        /// here comes the Effect of the Item
        /// </summary>
        /// <returns>the time teh item</returns>      
        public virtual int Use()
        {
            if (!ItemInUse)
            {
                if (GameManager.GetInstance().CurrentSection != null)
                {
                    bool itemRemoved = GameManager.GetInstance().CurrentSection.RemoveItem(this);
                    Debug.Log("Item removed from section after use: " + itemRemoved);
                }
            }

            return Time;
        }

        /// <summary>
        /// Pick up this item.
        /// Hides the item as well.
        /// </summary>
        /// <param name="owner">The new owner of this item.</param>
        public virtual void Pickup(Character owner)
        {
            // We have a new owner.
            Owner = owner;
            if (_net.IsServer || _net.IsClient)
            {
                networkView.RPC("SetOwner", RPCMode.Others, owner.name);
            }

            ReservingPlayer = null;

            // Make us invisible!
            ChangeVisibilityTo(false);
            if (_net.IsClient || _net.IsServer)
            {
                networkView.RPC("ChangeVisibilityTo", RPCMode.Others, false);
            }
        }

        /// <summary>
        /// Change the visibility of this Item.
        /// </summary>
        /// <param name="visible">Whether the item should be visible or not.</param>
        /// <remarks>[RPC] Callable and visible via network.</remarks>
        [RPC]
        public virtual void ChangeVisibilityTo(bool visible)
        {
            // Make us invisible!
            gameObject.renderer.enabled = visible;

            // and all of our Children
            foreach (Renderer r in gameObject.transform.GetComponentsInChildren<Renderer>())
            {
                r.enabled = visible;
            }
        }

        /// <summary>
        /// Called when this item should be dropped.
        /// </summary>
        [RPC]
        public virtual void Drop()
        {
            // Move us to the player.
            if (_net.IsClient)
            { // 'cause Client couldn't move items owned by the server (all items are owned by the server)
                networkView.RPC("Drop", RPCMode.Server);
                return;
            }

            transform.position = new Vector3(Owner.transform.position.x,
                Owner.transform.position.y + renderer.bounds.extents.y - 0.1f,
                Owner.transform.position.z);

            // We don't have an owner anymore.
            Owner = null;
            if (_net.IsServer || _net.IsClient)
            {
                networkView.RPC("SetOwner", RPCMode.Others, string.Empty);
            }

            // Finally, make us visible again.
            ChangeVisibilityTo(true);
            if (_net.IsClient || _net.IsServer)
            {
                networkView.RPC("ChangeVisibilityTo", RPCMode.Others, true);
            }
        }

        /// <summary>
        /// Sets the owner of this item.
        /// </summary>
        /// <remarks>[RPC]Visible via network.</remarks>
        /// <param name="owner">owner name to be find in the scene</param>
        [RPC]
        public virtual void SetOwner(string owner)
        {
            if (owner.Equals(string.Empty))
            { // do that to prevent unity is searching for an gameobject with empty string
                Owner = null;
            }
            else
            {
                Owner = GameObject.Find(owner).GetComponent<Character>();
            }
        }

        /// <summary>
        /// Deactivate the gravity of its rigidbody and set velocity to zero.
        /// </summary>
        /// <param name="other">The terrain or something else it collides with</param>
        public void OnTriggerEnter(Collider other)
        {
            Terrain t = other.gameObject.GetComponent<Terrain>();
            Transform obs = other.gameObject.transform.Find("Obstacles");
            if (t != null || obs != null)
            {
                Debug.Log("[BasicItem.cs] Is terrain");
                Rigidbody rig = gameObject.GetComponent<Rigidbody>();
                rig.useGravity = false;
                rig.velocity = Vector3.zero;
                transform.Translate(new Vector3(0f, 0.3f, 0f));
            }
        }

        /// <summary>
        /// Returns a value indicating whether this item currently picked up by a player.
        /// </summary>
        /// <returns>True if this item is picked up; false otherwise.</returns>
        public bool IsPickedUp()
        {
            return Owner != null;
        }
    }
}

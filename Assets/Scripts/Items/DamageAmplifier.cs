namespace Assets.Scripts.Items
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    /// <summary>
    /// An item that adds damage for a specific time
    /// </summary>
    public class DamageAmplifier : Item
    {
        private int _damageMult = 2;

        /// <summary>
        /// the fire that is instatiated
        /// </summary>
        private GameObject _fire;

        /// <summary>
        /// Gets or sets the prefab for the fire effects
        /// </summary>
        public GameObject FirePrefab
        {
            get;
            set;
        }

        /// <summary>
        /// is called when the Item is instanitated
        /// </summary>
        public override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        /// is called when the item is used (the effect happens here)
        /// </summary>
        /// <returns>the time the item was in use</returns>
        public override int Use()
        {
            base.Use();
            if (!ItemInUse)
            {
                ItemInUse = true;
                Time = 5;
                Owner.BasicDamage *= _damageMult;
                FirePrefab = Resources.Load("Particles/FirePrefab") as GameObject;
                if (_net.IsClient || _net.IsServer)
                {
                    _fire = (GameObject)Network.Instantiate(FirePrefab, Owner.transform.position, FirePrefab.transform.rotation, 0);
                    networkView.RPC("SetParentTransform", RPCMode.Others);
                }
                else
                {
                    _fire = (GameObject)Instantiate(FirePrefab, Owner.transform.position, FirePrefab.transform.rotation);
                    ////_fire.transform.parent = Owner.transform;
                }

                SetParentTransform();                
                Renderer mainR = Owner.GetComponent<Renderer>();
                Color oldColor = Color.black;
                if (_net.IsServer || _net.IsClient)
                {
                    networkView.RPC("SetRendererColor", RPCMode.Others, 1f, 0f, 0f);
                }

                if (mainR != null)
                {
                    oldColor = mainR.material.color;
                    mainR.material.color = new Color(1f, 0, 0, 1f);
                }

                Dictionary<Material, Color> oldColors = new Dictionary<Material, Color>();
                foreach (Renderer r in Owner.gameObject.GetComponentsInChildren<Renderer>())
                {
                    foreach (Material m in r.materials)
                    {
                        if (m.HasProperty("_Color"))
                        {
                            oldColors[m] = m.color;
                            m.color = new Color(1f, 0, 0, 1f);
                        }
                    }
                }

                StartCoroutine(CountAndRevert(Time, oldColors, oldColor));
            }

            return Time;
        }

        [RPC]
        private void SetRendererColor(float r, float g, float b)
        {
            Renderer mainR = Owner.GetComponent<Renderer>();
            if (mainR != null)
            {
                mainR.material.color = new Color(r, g, b);
            }

            foreach (Renderer rend in Owner.gameObject.GetComponentsInChildren<Renderer>())
            {
                foreach (Material m in rend.materials)
                {
                    if (m.HasProperty("_Color"))
                    {
                        m.color = new Color(r, g, b, 1f); // sry but i don't like to transmitt all color data .. so i used the parents default for all children.
                    }
                }
            }
        }

        [RPC]
        private void SetParentTransform()
        {
            _fire = GameObject.Find("FirePrefab(Clone)");
            _fire.name = _fire.name + "."; // next time this shouldn't be found by the line above ;)
            _fire.transform.parent = Owner.transform;
        }

        /// <summary>
        /// Waits the given amount in seconds and then reverts the effect
        /// </summary>
        /// <param name="seconds">time the Effect is used</param>
        /// <param name="oldColors">old colors of the materials</param>
        /// <param name="oldColor">old color of main material</param>
        /// <returns>IEnumerator for the timer</returns>
        private IEnumerator CountAndRevert(int seconds, Dictionary<Material, Color> oldColors, Color oldColor)
        {
            yield return new WaitForSeconds(seconds);
            Owner.BasicDamage /= _damageMult;
            Renderer mainR = Owner.GetComponent<Renderer>();
            if (_net.IsServer || _net.IsClient)
            {
                networkView.RPC("SetRendererColor", RPCMode.Others, 1f, 1f, 1f); // oldColor.r, oldColor.g, oldColor.b);
            }

            if (mainR != null)
            {
                mainR.material.color = oldColor;
            }

            foreach (Renderer r in Owner.gameObject.GetComponentsInChildren<Renderer>())
            {
                foreach (Material m in r.materials)
                {
                    if (m.HasProperty("_Color"))
                    {
                        m.color = oldColors[m];
                    }
                }
            }

            if (_net.IsServer || _net.IsClient)
            {
                Network.Destroy(_fire);
                Network.Destroy(gameObject);
            }
            else
            {
                Destroy(_fire);
                Destroy(gameObject);
            }
        }
    }
}

namespace Assets.Scripts
{
    using System.Collections;
    using UnityEngine;
    
    /// <summary>
    /// Area around the bases in specialmode in which the heroes could regenerate life.
    /// </summary>
    public class RegenerationArea : MonoBehaviour
    {
        private Mobs.Base _base;
        private float _rate, _time, _percent;
        private NetworkController _net;

        /// <summary>
        /// Sets the parented base
        /// </summary>
        public void Start()
        {
            _base = transform.parent.GetComponent<Mobs.Base>();
            _rate = 0.1f;
            _time = 0f;
            _percent = 0.01f;
            _net = GameManager.GetInstance().NetworkController;
        }

        /// <summary>
        /// Controlls which hero in the area regenerates life.
        /// </summary>
        /// <param name="other">The Object this Object collides with</param>
        public void OnTriggerStay(Collider other)
        {
            Hero hero = other.GetComponent<Hero>();
            if (hero != null && !hero.IsDead() && (hero.Team == _base.Team))
            {
                int maxHp = hero.MaxHP;
                _time += Time.deltaTime;
                if (_time >= _rate)
                {
                    _time = 0; // reset rate timer
                    if (_net.IsServer)
                    {
                        networkView.RPC("Regenerate", RPCMode.All, hero.name);
                    }
                    else if (!_net.IsClient)
                    {
                        hero.HP += (int)(maxHp * _percent); // need no check whether greater than 'hp + value' because it's set in propertie of charachter.cs
                    }
                }
            }
        }

        /// <summary>
        /// Networkregeneration triggered via Network.
        /// </summary>
        /// <param name="name">name of the hero who schould regenerate life</param>
        [RPC]
        private void Regenerate(string name)
        {
            Hero hero = GameObject.Find(name).gameObject.GetComponent<Hero>();
            hero.HP += (int)(hero.MaxHP * _percent);
        }
    }
}
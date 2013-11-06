namespace Assets.Scripts
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Class that describes the Wind, which can blow Characters off the bridge.
    /// </summary>
    public class SpecialModeWind : MonoBehaviour
    {
        private const int MINBREAK = 40;
        private const int MAXBREAK = 120;
        private const int MINDURATION = 5;
        private const int MAXDURATION = 10;

        private float _remainingBreak;
        private float _remainingDuration;

        private bool _windEnabled;
        private bool _windAnimationEnabled;
        private bool _bubblesFired;

        private ParticleSystem _windPs;
        private GameObject _bubblePs;

        private NetworkController _net;

        /// <summary>
        /// Called on Instantiation
        /// Sets the Initial Break
        /// Assigns the Particle Systems
        /// </summary>
        public void Start()
        {
            _net = GameObject.Find("Network").GetComponent<NetworkController>();
            _bubblePs = Resources.Load("SMObjects/BubblesPrefab") as GameObject;
            _windPs = transform.FindChild("WindAnimation").GetComponent<ParticleSystem>();
            if (_net.IsClient)
            {
                return;
            }

            CalculateBreak();
        }

        /// <summary>
        /// Called on every Frame
        /// Starts wind when break is finished
        /// stops wind when duration is over
        /// </summary>
        public void Update()
        {
            if (_net.IsClient)
            {
                return;
            }

            if (_remainingBreak > 0)
            {
                _remainingBreak -= Time.deltaTime;
            }

            if (_remainingBreak <= 0.7f && !_windAnimationEnabled)
            {
                CalculateDuration();
            }

            if (_remainingBreak <= 0 && !_windEnabled)
            {
                _windEnabled = true;
                Projectile p = ProjectilePool.GetProjectile(transform.position, transform.rotation);
                p.DoPiercing = true;
                p.Damage = 0;
                p.Speed = 0;
                p.Range = 1;
                p.TimeToLive = _remainingDuration;
                p.transform.localScale = new Vector3(transform.parent.localScale.x - 1, 50, 100);
                p.transform.Translate(Vector3.down * 25);
                p.collider.transform.localScale = p.transform.localScale;
                p.Knockback = p.transform.forward * 50;
                p.Knockback.y += 50;
                p.Apply();
            }

            if (_remainingBreak - 6 <= 0 && !_bubblesFired)
            {
                _bubblesFired = true;
                if (_net.IsServer)
                {
                    networkView.RPC("InstantiateBubbles", RPCMode.All);
                }
                else
                {
                    InstantiateBubbles();
                }
            }

            if (_remainingDuration > 0)
            {
                _remainingDuration -= Time.deltaTime;
            }

            if (_remainingDuration <= 0 && _windEnabled)
            {
                CalculateBreak();
            }
        }

        /// <summary>
        /// Sets a random Break length and stops all effects.
        /// </summary>
        private void CalculateBreak()
        {
            _remainingBreak = Random.Range(MINBREAK, MAXBREAK);
            _windEnabled = false;
            _bubblesFired = false;
            _windAnimationEnabled = false;
            if (_net.IsServer)
            {
                networkView.RPC("StopWindAnimation", RPCMode.All);
            }
            else
            {
                StopWindAnimation();
            }
        }

        /// <summary>
        /// Sets a random Duration of the wind effect and starts it.
        /// </summary>
        private void CalculateDuration()
        {
            _remainingDuration = Random.Range(MINDURATION, MAXDURATION);
            _windAnimationEnabled = true;
            if (_net.IsServer)
            {
                networkView.RPC("StartWindAnimation", RPCMode.All);
            }
            else
            {
                StartWindAnimation();
            }
        }

        /// <summary>
        /// Start the Wind Particle Effect
        /// </summary>
        [RPC]
        private void StartWindAnimation()
        {
            _windPs.Play();
        }

        /// <summary>
        /// Stops the Wind Particle Effect
        /// </summary>
        [RPC]
        private void StopWindAnimation()
        {
            _windPs.Stop();
        }

        /// <summary>
        /// Instantiates the Bubble Particle Effect
        /// </summary>
        [RPC]
        private void InstantiateBubbles()
        {
            Debug.Log(_net.IsServer + " BUBBLES!!!!");
            Vector3 pos = transform.position + (transform.forward * (-15));
            Instantiate(_bubblePs, pos, transform.rotation);
        }
    }
}
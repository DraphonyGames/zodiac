namespace Assets.Scripts
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Class that represents a conquerable Checkpoint
    /// </summary>
    public class Checkpoint : MonoBehaviour
    {
        #region "Global"

        #region "Constants

        /// <summary>
        /// how often the checkpoint state is refreshed (in seconds)
        /// </summary>
        private const float REFRESHRATE = 1f;

        /// <summary>
        /// how many percents a hero does per RefreshRate
        /// </summary>
        private const float HEROVALUE = 5;

        /// <summary>
        /// how many percents a mob/merc does per RefreshRate
        /// </summary>
        private const float MOBVALUE = 2.5f;

        /// <summary>
        /// how many points the Checkpoint generates per RefreshRate
        /// </summary>
        private const int POINTVALUE = 1;

        #endregion

        #region "Fields"

        /// <summary>
        /// The used NetworkController instance.
        /// </summary>
        private NetworkController _net;

        /// <summary>
        /// teamNo of the Team currently owning this Checkpoint
        /// </summary>
        private int _conqueror;

        /// <summary>
        /// status of the conquer (procents of the teamNo's)
        /// 50% is neutral
        /// 100% is conquered
        /// </summary>
        private Dictionary<int, float> _status;

        /// <summary>
        /// All Characters that are currently in the Checkpoints radius
        /// </summary>
        private List<Character> _conquering;

        /// <summary>
        /// variable counts when the checkpoint state is changed
        /// </summary>
        private float _passedTime;

        #endregion

        #region "Enums"

        /// <summary>
        /// Enum that describes if the status of the checkpoint
        /// relative to a character
        /// </summary>
        public enum Type
        {
            FRIENDLY,
            HOSTILE,
            NEUTRAL
        }

        #endregion

        #endregion

        #region "Methods"

        #region "public"

        /// <summary>
        /// Is called when a Character dies,
        /// so he stops to conquer.
        /// </summary>
        /// <param name="c">the dead character</param>
        public void NotifyDeath(Character c)
        {
            _conquering.Remove(c);
        }

        /// <summary>
        /// Initialises the Checkpoint
        /// </summary>
        public void Init()
        {
            _net = GameObject.Find("Network").GetComponent<NetworkController>();
            _status = new Dictionary<int, float>();
            List<Team> teams = GameManager.GetInstance().GetAllTeams();
            foreach (Team t in teams)
            {
                _status[t.TeamNo] = 50;
            }

            _conqueror = -1;
            _conquering = new List<Character>();
            _passedTime = 0;
        }

        /// <summary>
        /// Called when a Character enters the Checkpoint radius
        /// </summary>
        /// <param name="c">the entering character</param>
        public void Enter(Character c)
        {
            if (c == null)
            {
                return;
            }

            if (!_conquering.Contains(c))
            {
                _conquering.Add(c);
            }
        }

        /// <summary>
        /// called when a character leaves the checkpoint radius
        /// </summary>
        /// <param name="c">the leaving character</param>
        public void Exit(Character c)
        {
            if (c == null)
            {
                return;
            }

            _conquering.Remove(c);
        }

        /// <summary>
        /// Is called once per frame.
        /// Checks the current status of the Checkpoint and
        /// adjusts it for each character standing in the radius.
        /// Gives Points to the conqueror.
        /// </summary>
        public void Update()
        {
            _passedTime += Time.deltaTime;
            if (_passedTime >= REFRESHRATE)
            {
                _passedTime = 0;
                float oldstatus = _status[1];
                foreach (Character c in _conquering)
                {
                    if (c is Hero)
                    {
                        ChangeStatus(c.Team.TeamNo, HEROVALUE);
                    }

                    if (c is Mob || c is Mercenary)
                    {
                        if (c != null && !c.IsDead())
                        {
                            ChangeStatus(c.Team.TeamNo, MOBVALUE);
                        }
                    }
                }

                CheckStatusBounds();

                if (oldstatus > _status[1])
                {
                    transform.FindChild("Bottom").renderer.material.color = new Color(1, 0, 0, 0.5f);
                }
                else if (oldstatus < _status[1])
                {
                    transform.FindChild("Bottom").renderer.material.color = new Color(0, 0, 1, 0.5f);
                }
                else
                {
                    transform.FindChild("Bottom").renderer.material.color = new Color(1, 1, 1, 0.5f);
                }

                if (_conqueror != -1)
                {
                    if (_status[_conqueror] <= 50)
                    {
                        if (_net.IsServer)
                        {
                            networkView.RPC("SetNeutral", RPCMode.Others);
                        }

                        if (!_net.IsClient)
                        {
                            SetNeutral();
                        }

                        // Checkpoint is now neutral
                    }
                }
                else if (_conqueror == -1)
                {
                    foreach (Team t in GameManager.GetInstance().GetAllTeams())
                    {
                        if (_status[t.TeamNo] >= 100)
                        {
                            if (_net.IsServer)
                            {
                                networkView.RPC("SetConqueror", RPCMode.Others, t.TeamNo);
                            }

                            if (!_net.IsClient)
                            {
                                SetConqueror(t.TeamNo);
                            }

                            // Checkpoint is now conquered
                        }
                    }
                }

                if (_conqueror != -1)
                {
                    Team conqueror = GameManager.GetInstance().GetTeam(_conqueror);
                    if (_net.IsServer)
                    {
                        networkView.RPC("GeneratePoints", RPCMode.Others, _conqueror);
                    }

                    if (!_net.IsClient)
                    {
                        GeneratePoints(_conqueror);
                    }

                    if (conqueror.Points >= GameManager.SPECIALWINNINGPOINTS)
                    {
                        GameManager.GetInstance().NotifySpecialWin(conqueror);
                    }
                }
            }
        }

        /// <summary>
        /// checks if the checkpoint has the given status relative to the player
        /// </summary>
        /// <param name="player">the player</param>
        /// <param name="type">status to be checked</param>
        /// <returns>if the checkpoint has the given type</returns>
        public bool CheckType(Character player, Type type)
        {
            switch (type)
            {
                case Type.FRIENDLY:
                    if (player.Team == null)
                    {
                        return false;
                    }

                    return _conqueror == player.Team.TeamNo;
                case Type.HOSTILE:
                    return _conqueror != -1 && _conqueror != player.Team.TeamNo;
                default:
                    return _conqueror == -1;
            }
        }

        #endregion

        #region "private"

        #region "RPC"

        [RPC]
        private void GeneratePoints(int team)
        {
            Team conqueror = GameManager.GetInstance().GetTeam(team);
            conqueror.Points += POINTVALUE;
        }

        [RPC]
        private void SetNeutral()
        {
            _conqueror = -1;
            GetComponentInChildren<ParticleSystem>().startColor = new Color(1, 1, 1);
        }

        [RPC]
        private void SetConqueror(int team)
        {
            _conqueror = team;
            GetComponentInChildren<ParticleSystem>().startColor = _conqueror == 2 ? new Color(1, 0, 0) : new Color(0, 0, 1);
        }
        #endregion

        /// <summary>
        /// adds a value to the status of the team and substracts
        /// this value from all other statuses
        /// </summary>
        /// <param name="team">the adding team</param>
        /// <param name="value">the value to add</param>
        private void ChangeStatus(int team, float value)
        {
            _status[team] += value;

            foreach (Team t in GameManager.GetInstance().GetAllTeams())
            {
                if (t.TeamNo != team)
                {
                    _status[t.TeamNo] -= value;
                }
            }
        }

        /// <summary>
        /// Checks if the status is out of Bounds (0 - 100)
        /// and adjusts it.
        /// </summary>
        private void CheckStatusBounds()
        {
            foreach (Team t in GameManager.GetInstance().GetAllTeams())
            {
                if (_status[t.TeamNo] < 0)
                {
                    _status[t.TeamNo] = 0;
                }
                else if (_status[t.TeamNo] > 100)
                {
                    _status[t.TeamNo] = 100;
                }
            }
        }

        #endregion

        #endregion
    }
}
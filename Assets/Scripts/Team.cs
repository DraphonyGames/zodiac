namespace Assets.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    /// <summary>
    /// Represents a team of players/Heroes/Mobs/etc.
    /// </summary>
    public class Team
    {
        #region public read-only fields
        /// <summary>
        /// The number of this team.
        /// </summary>
        public readonly int TeamNo;
        #endregion

        #region private fields
        /// <summary>
        /// List of current team members, excluding the team leader.
        /// </summary>
        private List<Character> _teamMembers;

        /// <summary>
        /// The current points (special mode)
        /// </summary>
        private int _points;
        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Team"/> class.
        /// Sets the team number to the one specified.
        /// </summary>
        /// <remarks>
        /// This does not set a team leader.
        /// </remarks>
        /// <param name="teamNo">Team number</param>
        public Team(int teamNo)
            : this(teamNo, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Team"/> class.
        /// Sets the team number to the one specified; also sets a team leader.
        /// </summary>
        /// <param name="teamNo">Team number</param>
        /// <param name="teamLeader">Team leader</param>
        public Team(int teamNo, Hero teamLeader)
        {
            TeamNo = teamNo;
            TeamLeader = teamLeader;
            _teamMembers = new List<Character>();
            Points = 0;

            if (teamLeader != null)
            {
                TeamLeader.Team = this;

                if (!teamLeader.IsDead())
                {
                    MembersAlive++;
                }
            }
        }
        #endregion

        #region public properties
        /// <summary>
        /// Gets or sets the current team leader.
        /// </summary>
        /// <remarks>TODO: Do we really need team leaders?</remarks>
        public Hero TeamLeader { get; protected set; }

        /// <summary>
        /// Gets or sets the number of team members that are alive.
        /// </summary>
        public int MembersAlive { get; protected set; }

        /// <summary>
        /// Gets or sets the current points (special mode)
        /// </summary>
        public int Points
        {
            get
            {
                return _points;
            }

            set
            {
                if (value > _points)
                {
                    _points = value;
                }
            }
        }

        /// <summary>
        /// Gets the base of this team (where team members spawn).
        /// </summary>
        public Assets.Scripts.Mobs.Base Base
        {
            get
            {
                return TeamNo == 1 ? GameObject.Find("Base1").GetComponent<Assets.Scripts.Mobs.Base>() : GameObject.Find("Base2").GetComponent<Assets.Scripts.Mobs.Base>();
            }
        }

        /// <summary>
        /// Gets a list of all checkpoints captured by this team.
        /// </summary>
        public List<Checkpoint> CapturedCheckpoints
        {
            get
            {
                List<Checkpoint> checks = new List<Checkpoint>();
                foreach (Checkpoint c in GameManager.GetInstance().Checkpoints)
                {
                    if (c.CheckType(this.Base, Checkpoint.Type.FRIENDLY))
                    {
                        checks.Add(c);
                    }
                }

                return checks;
            }
        }
        #endregion

        #region public methods
        /// <summary>
        /// Add a non-leading member to the team.
        /// </summary>
        /// <remarks>
        /// If the specified member already belongs to the team, he is added _twice_.
        /// </remarks>
        /// <param name="member">Member to add.</param>
        public void AddMember(Character member)
        {
            AddMember(member, false);
        }

        /// <summary>
        /// Add a member to the team.
        /// </summary>
        /// <remarks>
        /// If the specified member already belongs to the team, he is added _twice_.
        /// </remarks>
        /// <param name="member">Member to add.</param>
        /// <param name="isTeamLeader">Should this member be the new team leader?</param>
        public void AddMember(Character member, bool isTeamLeader)
        {
            Debug.Log("[Team.cs] Adding member " + member.GetType().Name +
                " #" + member.PlayerNo + " (team leader: " + isTeamLeader + ")" +
                " to team " + TeamNo);

            if (isTeamLeader)
            {
                RemoveMember(TeamLeader);
                TeamLeader = (Hero)member;
            }
            else
            {
                _teamMembers.Add(member);
            }

            member.Team = this;
            if (!member.IsDead())
            {
                MembersAlive++;
            }
        }

        /// <summary>
        /// Remove a member from the team.
        /// </summary>
        /// <param name="member">Member to remove.</param>
        /// <returns>
        ///     True if the team member has been removed, false
        ///     if it didn't exist in the team.
        /// </returns>
        public bool RemoveMember(Character member)
        {
            bool removed = false;

            if (member == null)
            {
                return false;
            }

            if (member is Hero && TeamLeader != null && TeamLeader.Equals((Hero)member))
            {   // This is the team leader.
                removed = true;
                TeamLeader = null;
            }
            else
            {   // It's (probably) a normal member.
                removed = _teamMembers.Remove(member);
            }

            if (removed)
            {
                member.Team = null;

                if (!member.IsDead())
                {   // This member does not count towards the alive members in
                    // this team anymore. If he is dead, MembersAlive was decremented
                    // already.
                    MembersAlive--;
                }
            }

            return removed;
        }

        /// <summary>
        /// Get all team members, including the team leader.
        /// </summary>
        /// <remarks>
        /// This includes _all_ team members, regardless of their type (Hero,
        /// Mob...).
        /// </remarks>
        /// <returns>All team members.</returns>
        public List<Character> GetAllTeamMembers()
        {
            List<Character> list = new List<Character>();

            if (TeamLeader != null)
            {
                list.Add(TeamLeader);
            }

            list.AddRange(_teamMembers);
            return list;
        }

        /// <summary>
        /// Get all Hero team members, including the team leader.
        /// </summary>
        /// <returns>All hero team members.</returns>
        public List<Hero> GetHeroTeamMembers()
        {
            List<Hero> list = new List<Hero>();

            if (TeamLeader != null)
            {
                list.Add(TeamLeader);
            }

            _teamMembers.ForEach(
                delegate(Character item)
                {
                    if (item is Hero)
                    {
                        list.Add((Hero)item);
                    }
                });

            return list;
        }

        /// <summary>
        /// Get all Mob team members.
        /// </summary>
        /// <returns>All mob team members.</returns>
        public List<Mob> GetMobTeamMembers()
        {
            List<Mob> list = new List<Mob>();

            _teamMembers.ForEach(
                delegate(Character item)
                {
                    if (item is Mob)
                    {
                        list.Add((Mob)item);
                    }
                });

            return list;
        }

        /// <summary>
        /// Get all playable team members.
        /// </summary>
        /// <remarks>
        /// This includes Mobs and Heroes.
        /// </remarks>
        /// <returns>All playable team members.</returns>
        public List<Character> GetPlayableTeamMembers()
        {
            List<Character> list = new List<Character>();

            GetHeroTeamMembers().ForEach(
                delegate(Hero item)
                {
                    list.Add(item);
                });
            GetMobTeamMembers().ForEach(
                delegate(Mob item)
                {
                    list.Add(item);
                });

            return list;
        }

        /// <summary>
        /// Get all Mercenary team members.
        /// </summary>
        /// <returns>All mercenary team members.</returns>
        public List<Mercenary> GetMercenaryTeamMembers()
        {
            List<Mercenary> list = new List<Mercenary>();

            _teamMembers.ForEach(
                delegate(Character item)
                {
                    if (item is Mercenary)
                    {
                        list.Add((Mercenary)item);
                    }
                });

            return list;
        }

        /// <summary>
        /// returns the sum of all teammembers life
        /// </summary>
        /// <returns>all hp</returns>
        public int GetTotalHP()
        {
            return _teamMembers.Sum(c => c.HP);
        }

        /// <summary>
        /// Notify the team that a member died.
        /// </summary>
        /// <remarks>
        /// This ends the game if all members are dead!
        /// </remarks>
        /// <param name="deadMember">The dead member.</param>
        public void NotifyMemberDead(Character deadMember)
        {
            if (GameManager.GetInstance().GameMode == GameManager.Mode.SPECIAL)
            {
                return;
            }

            if (MembersAlive > 0 && --MembersAlive == 0)
            {
                // TODO: Should call NotifyGameEnd() in multiplayer mode.
                GameManager.GetInstance().NotifyGameOver(this);
            }
        }

        /// <summary>
        /// Check if an object equals to this one.
        /// </summary>
        /// <param name="obj">Object to compare with.</param>
        /// <returns>True if both objects are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            Team other = obj as Team;
            if (other == null)
            {   // It's not a team -- no equality.
                return false;
            }

            return this.Equals(other);
        }

        /// <summary>
        /// Check if another Team equals to this one.
        /// </summary>
        /// <remarks>
        /// Two teams are equal if they have the same team number.
        /// </remarks>
        /// <param name="other">Team to compare with.</param>
        /// <returns>True if both teams are equal, false otherwise.</returns>
        public bool Equals(Team other)
        {
            return TeamNo == other.TeamNo;
        }

        /// <summary>
        /// Returns the Hashcode created for this Object.
        /// </summary>
        /// <returns>Integer Hashcode</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Respawns all dead team members at the beginning of the current section.
        /// </summary>
        public void RespawnDeadMember()
        {
            foreach (var character in GetHeroTeamMembers().Where(character => character.IsDead()))
            {
                GameManager.GetInstance().CurrentSection.RespawnCharacter(character);
            }
        }
        #endregion
    }
}

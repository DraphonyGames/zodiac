namespace Assets.Scripts.Mobs
{
    using System.Collections;
    using System.Collections.Generic;
    using Assets.Scripts.Mobs.Elite;
    using UnityEngine;

    /// <summary>
    /// Represents a player base.
    /// </summary>
    public class Base : Character
    {
        #region private constants
        /// <summary>
        /// How often (in seconds) should new mobs spawn in the base?
        /// </summary>
        private const float SPAWNRATE = 15.0f;
        #endregion

        #region private fields
        #endregion

        #region public methods
        /// <summary>
        /// Executed on Instantiation of a Base.
        /// </summary>
        public override void Awake()
        {
            BasicDamage = 0;
            MeeleRange = 0;

            AttackDamage = 0;
            SpecialDamageMultiplier = 0;

            MaxHP = 5000;
            MaxMP = 0;

            MovementSpeed = 0;
            JumpSpeed = 0;

            base.Awake();
            if (_net.IsServer || _net.Status == NetworkPeerType.Disconnected)
            {
                StartCoroutine(SpawnMobs());
            }
        }

        /// <summary>
        /// Add a mob to this base's team.
        /// </summary>
        /// <remarks>[RPC] Visible via network. Id is send to prevent a wrong allocation in runtime.</remarks>
        /// <param name="name">GameObject name of the mob to add.</param>
        /// <param name="id">The NetworkViewId of the Instantiated GameObject</param>
        [RPC]
        public void AddMobToTeam(string name, NetworkViewID id)
        {
            Character character = GameObject.Find(name).GetComponent<Character>();
            character.networkView.viewID = id;
            character.Type = Character.Types.AI;
            character.name = character.name + character.GetInstanceID().ToString();
            Team.AddMember(character);
        }

        /// <summary>
        /// Unity's Update callback.
        /// </summary>
        public new void Update()
        {
        }

        /// <summary>
        /// Unity's Start callback. Called before the first call to Update().
        /// </summary>
        public override void Start()
        {
            // Always executing base.Start() firsts
            base.Start();
        }

        /// <summary>
        /// Override base.Move(): Bases cannot move.
        /// </summary>
        public override void Move()
        {
        }

        /// <summary>
        /// Override base.PerformJumo(): Bases cannot jump.
        /// </summary>
        public override void PerformJump()
        {
        }

        /// <summary>
        /// Override base.ReceiveKnockback(): Bases cannot receive any knockback.
        /// </summary>
        /// <param name="knockback">Knockback direction</param>
        /// <param name="damageDirection">Direction the damage comes from</param>
        /// <param name="projectilepos">center of the projectile that hit</param>
        /// <param name="knockbackHorizontal">value of the knockback from center to character</param>
        [RPC]
        public override void ReceiveKnockback(Vector3 knockback, Quaternion damageDirection, Vector3 projectilepos, float knockbackHorizontal)
        { 
        }

        /// <summary>
        /// Kills this player base.
        /// </summary>
        /// <param name="killer">The character that killed this entity (optional).</param>
        public override void Die(Character killer)
        {
            // Destroying a base should not increase the attacker's kill count.
            base.Die(null);

            List<Team> teams = GameManager.GetInstance().GetAllTeams();
            if (teams.Count > 2)
            {
                Debug.Log("More than two teams in special mode is not implemented yet!");
                return;
            }

            foreach (Team t in teams)
            {
                if (t != this.TeamHidden)
                {
                    GameManager.GetInstance().NotifySpecialWin(t);
                    return;
                }
            }
        }
        #endregion

        #region protected methods
        /// <summary>
        /// Override base.PerformAttack(): Bases cannot attack.
        /// </summary>
        protected override void PerformAttack()
        {
        }

        /// <summary>
        /// Override base.PerformDefend(): Bases cannot defend.
        /// </summary>
        protected override void PerformDefend()
        {
        }

        /// <summary>
        /// Override base.PerformSpecialAttack(): Bases have no special attacks.
        /// </summary>
        protected override void PerformSpecialAttack()
        {
        }

        /// <summary>
        /// Override base.AddSpecialAttackKeyCombos(): Bases have no special attacks.
        /// </summary>
        protected override void AddSpecialAttackKeyCombos()
        {
        }
        #endregion

        #region private methods
        private IEnumerator SpawnMobs()
        {
            const int MAXMOBS = 10;
            List<Checkpoint> checkpoints = (List<Checkpoint>)GameManager.GetInstance().Checkpoints;

            while (true)
            {
                int wronglyCountedEliteMobs = 0;
                foreach (Mob mob in GameManager.GetInstance().GetTeam(2).GetMobTeamMembers())
                {
                    if (mob is EliteMob)
                    {
                        ++wronglyCountedEliteMobs;
                    }
                }

                float checkpointCounter = checkpoints.Count / 2;

                foreach (Checkpoint checkpoint in checkpoints)
                {
                    if (checkpoint.CheckType(this, Checkpoint.Type.FRIENDLY))
                    {
                        checkpointCounter -= 0.5f;
                    }
                    else if (checkpoint.CheckType(this, Checkpoint.Type.HOSTILE))
                    {
                        checkpointCounter += 0.5f;
                    }
                }

                int meleeMobs = Mathf.RoundToInt(checkpointCounter * 2 / 3);
                for (int i = 0; i < meleeMobs; i++)
                {
                    if (Team.TeamNo == 1)
                    {
                        if (GameManager.GetInstance().GetTeam(1).GetMercenaryTeamMembers().Count >= MAXMOBS)
                        {                            
                            break;
                        }
                    }
                    else
                    {
                        if (GameManager.GetInstance().GetTeam(2).GetMobTeamMembers().Count >= MAXMOBS + wronglyCountedEliteMobs)
                        { // +2 because someone didn't noticed the elitemobs are mobs
                            break;
                        }
                    }

                    GameObject mob = Team.TeamNo == 1 ? Datasheet.Mercenaries()[1] : Datasheet.Mobs()[0];
                    this.Spawn(mob);
                }

                int rangedMobs = Mathf.RoundToInt(checkpointCounter * 1 / 3);
                for (int i = 0; i < rangedMobs; i++)
                {
                    if (Team.TeamNo == 1)
                    {
                        if (GameManager.GetInstance().GetTeam(1).GetMercenaryTeamMembers().Count >= MAXMOBS)
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (GameManager.GetInstance().GetTeam(2).GetMobTeamMembers().Count >= MAXMOBS + wronglyCountedEliteMobs)
                        { // +2 because someone didn't noticed the elitemobs are mobs
                            break;
                        }
                    }

                    GameObject mob = Team.TeamNo == 1 ? Datasheet.Mercenaries()[0] : Datasheet.Mobs()[1];
                    this.Spawn(mob);
                }

                yield return new WaitForSeconds(SPAWNRATE);
            }
        }

        private void Spawn(GameObject mob)
        {
            Vector3 newPosition = transform.position + new Vector3(Random.Range(-7, 10), 0, Random.Range(-6, 4));
            Character character;
            if (!_net.IsServer)
            {
                character = ((GameObject)Instantiate(mob, newPosition, Quaternion.identity)).GetComponent<Character>();
            }
            else
            {   // Instantiate via network and add them on all clients to this bases team.
                character = ((GameObject)Network.Instantiate(mob, newPosition, Quaternion.identity, 0)).GetComponent<Character>();
                networkView.RPC("AddMobToTeam", RPCMode.Others, character.name, character.networkView.viewID);
            }

            character.Type = Types.AI;
            character.name = character.name + character.GetInstanceID().ToString();
            Team.AddMember(character);
        }
        #endregion
    }
}

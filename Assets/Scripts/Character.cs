namespace Assets.Scripts
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// This class defines all AI or human playable classes.
    /// </summary>
    [RequireComponent(typeof(AI))]
    public abstract class Character : AbstractSpawnable
    {
        #region "global properties"

        #region "Fields"
        #region "public"
        #region "movement"
        /// <summary>
        /// The movement direction of the player (changed by human input or ai).
        /// </summary>
        public Vector3 Direction = Vector3.zero;
        #endregion

        /// <summary>
        /// Type of this character.
        /// </summary>
        public Types Type = Types.CHARACTER_SELECTION;

        /// <summary>
        /// The current item we have picked up.
        /// </summary>
        public Item CurrentItem;

        /// <summary>
        /// Sets the player number for testing.
        /// </summary>
        public int TestPlayerNo;

        /// <summary>
        /// XXX: Team functionality testing code.
        /// </summary>
        public int TestTeamValue;
        #endregion

        #region "protected"
        #region "default animations"
        /// <summary>
        /// List of all character animations.
        /// </summary>
        protected List<string> AnimationList = new List<string>();

        /// <summary>
        /// if walking animation should be played
        /// </summary>
        protected bool PlayWalking;

        /// <summary>
        /// if running animation should be played
        /// </summary>
        protected bool PlayRunning;

        /// <summary>
        /// if jumping animation should be played
        /// </summary>
        protected bool PlayJumping;

        /// <summary>
        /// if falling animation should be played
        /// </summary>
        protected bool PlayFalling;

        /// <summary>
        /// if landing animation should be played
        /// </summary>
        protected bool PlayLanding;

        /// <summary>
        /// if standing animation should be played
        /// </summary>
        protected bool PlayStanding;
        #endregion

        /// <summary>
        /// The character controller of this spawnable.
        /// </summary>
        protected CharacterController CharacterController;

        /// <summary>
        /// The character controller of this spawnable.
        /// </summary>
        protected AudioSource DyingSound;

        /// <summary>
        /// The current item we _could_ pick up if we wanted to.
        /// </summary>
        protected Item MaybeItem;

        /// <summary>
        /// The instantiated Healthbar
        /// </summary>
        protected GameObject Healthbar;

        /// <summary>
        /// The Prefab for the Healthbar
        /// </summary>
        protected GameObject HealthbarPrefab;

        /// <summary>
        /// The instantiated Manabar
        /// </summary>
        protected GameObject Manabar;

        /// <summary>
        /// The Prefab for the Manabar
        /// </summary>
        protected GameObject ManabarPrefab;

        /// <summary>
        /// The instantiated Staminabar
        /// </summary>
        protected GameObject Staminabar;

        /// <summary>
        /// The Prefab for the Staminabar
        /// </summary>
        protected GameObject StaminabarPrefab;

        /// <summary>
        /// The Prefab for the Name String
        /// </summary>
        protected GameObject NamePrefab;

        /// <summary>
        /// The instantiated NameObject
        /// </summary>
        protected GameObject NameText;

        /// <summary>
        /// The Prefab for the Dying-Smoke
        /// </summary>
        protected GameObject DieSmokePrefab;

        /// <summary>
        /// The percentaged amount of damage this Character should receive when defending.
        /// </summary>
        /// <remarks>Also see <see cref="DefendDamagePercentage"/>.</remarks>
        protected float DefendDamagePercentageHidden = 1f;

        /// <summary>
        /// Additional damage multiplier for specialattacks
        /// </summary>
        protected float SpecialDamageMultiplierHidden;

        /// <summary>
        /// Damage done by attacks
        /// </summary>
        protected int AttackDamageHidden;

        /// <summary>
        /// actual amount of life
        /// </summary>
        protected int HpHidden;

        /// <summary>
        /// actual amount of mana
        /// </summary>
        protected float MpHidden;

        /// <summary>
        /// actual amount of stamina
        /// </summary>
        protected float StaminaHidden;

        /// <summary>
        /// Team of this spawnable.
        /// </summary>
        protected Team TeamHidden;

        /// <summary>
        /// Can the Character move?
        /// </summary>
        protected bool CanMoveHidden;

        /// <summary>
        /// Is the Character attacking?
        /// </summary>
        protected bool IsAttackingHidden;

        /// <summary>
        /// Is the Character knocked back?
        /// </summary>
        protected bool IsKnockedBackHidden;

        /// <summary>
        /// Is the Character immune to Knockback?
        /// </summary>
        protected bool IsKnockbackImmuneHidden;

        /// <summary>
        /// Is the Character immune to Damage?
        /// </summary>
        protected bool IsDamageImmuneHidden;

        /// <summary>
        /// Is the Character in the air?
        /// </summary>
        protected bool IsJumpingHidden;

        /// <summary>
        /// The state machine used for special attacks.
        /// </summary>
        protected StateMachine StateMachine;

        /// <summary>
        /// The position where projectiles shot by this Character should be spawned.
        /// </summary>
        /// <remarks>
        /// Currently, this is need because our player models have their center point
        /// _under_ their feet (FIXME!).
        /// </remarks>
        protected Vector3 ProjectilePos;
        #endregion

        #region "private"

        /// <summary>
        /// Whether this should be set to inactive on next update call
        /// </summary>
        private bool _setInactiveInNextFrame;

        /// <summary>
        /// Whether the item shall be shown for this character
        /// </summary>
        private bool _showItem;

        /// <summary>
        /// the GameObject for the itemTexture
        /// </summary>
        private GameObject _itemTexture;
        #endregion
        #endregion

        #region "Enum Types"
        /// <summary>
        /// Types of characters.
        /// </summary>
        public enum Types
        {
            /// <summary>
            /// Controlled by a human player.
            /// </summary>
            HUMAN,

            /// <summary>
            /// Controlled by the AI.
            /// </summary>
            AI,

            /// <summary>
            /// An inactive character displayed in the character selection screen.
            /// </summary>
            CHARACTER_SELECTION
        }

        /// <summary>
        /// The Action types which could be performed each frame
        /// </summary>
        public enum Action
        {
            /// <summary>
            /// Don't perform any action in this frame
            /// </summary>
            NONE,

            /// <summary>
            /// The action in this frame is attacking (normal)
            /// </summary>
            ATTACK,

            /// <summary>
            /// start or stay defending in this frame
            /// </summary>
            DEFEND,

            /// <summary>
            /// Pick up an item near
            /// </summary>
            PICKUP_ITEM,

            /// <summary>
            /// drop current item
            /// </summary>
            DROP_ITEM,

            /// <summary>
            /// use the current item
            /// </summary>
            USE_ITEM,

            /// <summary>
            /// this frame detected a special attack
            /// </summary>
            SPECIAL_ATTACK
        }
        #endregion

        #region "Properties"
        #region "public"
        #region "action"
        /// <summary>
        /// Gets or sets the next action this character shoud perform.
        /// </summary>
        /// <remarks>
        /// This can be set via human input or the AI script.
        /// </remarks>
        public Action NextAction { get; set; }

        /// <summary>
        /// Gets or sets the special attack that should be performed.
        /// </summary>
        /// <remarks>
        /// The attack to perform can either be detected by the state
        /// machine (for human players) or simply set by the AI.
        /// </remarks>
        public StateMachine.AttackCallback SpecialAttack { get; set; }
        #endregion

        #region "movement"
        /// <summary>
        /// Gets or sets the movement speed of the character.
        /// </summary>
        public float MovementSpeed { get; set; }

        /// <summary>
        /// Gets or sets the height / velocity of jumping.
        /// </summary>
        public float JumpSpeed { get; set; }

        /// <summary>
        /// Gets or sets the multiplicator of the movement speed while running.
        /// </summary>
        public float RunningFactor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the hero is able to move.
        /// </summary>
        public bool CanMove
        {
            get
            {
                return CanMoveHidden;
            }

            set
            {
                if ((_net.IsServer || _net.IsClient) && Application.loadedLevelName.Equals("SM_Game"))
                {
                    networkView.RPC("NetworkSetCanMove", RPCMode.All, value);
                }
                else
                {
                    // Running locally.
                    CanMoveHidden = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the hero is attacking at the moment.
        /// </summary>
        public bool IsAttacking
        {
            get
            {
                return IsAttackingHidden;
            }

            set
            {
                if ((_net.IsServer || _net.IsClient) && Application.loadedLevelName.Equals("SM_Game"))
                {
                    networkView.RPC("NetworkSetIsAttacking", RPCMode.All, value);
                }
                else
                {
                    // Running locally.
                    IsAttackingHidden = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the character is actually knockbacked.
        /// </summary>
        public bool IsKnockedBack
        {
            get
            {
                return IsKnockedBackHidden;
            }

            set
            {
                if ((_net.IsServer || _net.IsClient) && Application.loadedLevelName.Equals("SM_Game"))
                {
                    networkView.RPC("NetworkSetIsKnockedBack", RPCMode.All, value);
                }
                else
                {
                    // Running locally.
                    IsKnockedBackHidden = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the time that the Character has to wait
        /// between two attacks.
        /// </summary>
        public float AttackRechargeTime { get; set; }
        #endregion

        #region stats
        /// <summary>
        /// Gets or sets the number of health packs used by this character.
        /// </summary>
        public int StatsUsedHealthPacks { get; set; }

        /// <summary>
        /// Gets or sets the total number of health points that this character has lost
        /// until now.
        /// </summary>
        public int StatsLostHP { get; set; }

        /// <summary>
        /// Gets or sets the number of damage points (HP) that this character has dealt to enemies.
        /// </summary>
        public int StatsEnemyDealtDamage { get; set; }

        /// <summary>
        /// Gets or sets the number of mercs freed by this character that didn't make it, i. e. were
        /// killed.
        /// </summary>
        public int StatsMercenariesLost { get; set; }

        /// <summary>
        /// Gets or sets the number of mercenaries that this character freed.
        /// </summary>
        /// <remarks>
        /// This includes alive and dead mercenaries.
        /// </remarks>
        public int StatsMercenariesFreed { get; set; }

        /// <summary>
        /// Gets or sets the number of entities killed by this Character.
        /// </summary>
        public int StatsKillCount { get; set; }
        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether the godmode is enabled
        /// (true) or disabled (false).
        /// </summary>
        /// <remarks>
        /// God mode makes this character invulnerable to damage.
        /// </remarks>
        public bool Godmode { get; set; }

        /// <summary>
        /// Gets or sets the player ID of this character.
        /// </summary>
        public int PlayerNo
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the basic damage done by this character.
        /// </summary>
        public int BasicDamage { get; set; }

        /// <summary>
        /// Gets or sets the range within this heros attacks hitting enemys.
        /// </summary>
        public float MeeleRange { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the character is in a defending state or not.
        /// </summary>
        public bool IsDefending
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the percentaged amount of damage this Character
        /// should receive when defending.
        /// </summary>
        /// <remarks>
        /// Example: If this is set to 0.15 (15%), then the attacked Character only
        /// gets 15% of the damage when defending.
        /// </remarks>
        public float DefendDamagePercentage
        {
            get { return DefendDamagePercentageHidden; }
            set { DefendDamagePercentageHidden = Mathf.Clamp01(value); }
        }

        /// <summary>
        /// Gets or sets damage done by attacks
        /// </summary>
        public int AttackDamage
        {
            get { return AttackDamageHidden; }
            set { AttackDamageHidden = Mathf.Clamp(value, 0, 999999); }
        }

        /// <summary>
        /// Gets or sets additional damage multiplier for specialattacks.
        /// </summary>
        public float SpecialDamageMultiplier
        {
            get { return SpecialDamageMultiplierHidden; }
            set { SpecialDamageMultiplierHidden = Mathf.Clamp(value, 1.0f, 100.0f); }
        }

        /// <summary>
        /// Gets or sets the highest amount of health which this character can have
        /// </summary>
        public int MaxHP { get; set; }

        /// <summary>
        /// Gets or sets the highest amount of mana which this character can have
        /// </summary>
        public float MaxMP { get; set; }

        /// <summary>
        /// Gets or sets the amount of MP this character should regenerate over
        /// one second.
        /// </summary>
        public float ManaRegenerationAmount { get; set; }

        /// <summary>
        /// Gets or sets the highest amount of Stamina which this character can have
        /// </summary>
        public int MaxStamina { get; set; }

        /// <summary>
        /// Gets or sets the actual amount of life.
        /// </summary>
        public int HP
        {
            get { return HpHidden; }
            set { HpHidden = Mathf.Clamp(value, 0, MaxHP); }
        }

        /// <summary>
        /// Gets or sets the actual amount of mana.
        /// </summary>
        /// <remarks>
        /// The setter is network-aware and will sync the MP value to all
        /// other clients and the server (but only if the multiplayer level is loaded).
        /// </remarks>
        public float MP
        {
            get
            {
                return MpHidden;
            }

            set
            {
                if ((_net.IsServer || _net.IsClient) && Application.loadedLevelName.Equals("SM_Game"))
                {
                    // Notify others about the MP change.
                    networkView.RPC("NetworkSetMP", RPCMode.All, value);
                }
                else
                {
                    // Running locally.
                    MpHidden = Mathf.Clamp(value, 0, MaxMP);
                }
            }
        }

        /// <summary>
        /// Gets or sets the actual amount of Stamina.
        /// </summary>
        public float Stamina
        {
            get { return StaminaHidden; }
            set { StaminaHidden = Mathf.Clamp(value, 0, MaxStamina); }
        }

        /// <summary>
        /// Gets or sets the Rate which describes the regeneration of Stamina
        /// </summary>
        public float StaminaRegRate { get; set; }

        /// <summary>
        /// Gets or sets the Rate which describe the consumption of Stamina
        /// </summary>
        public float StaminaUseRate { get; set; }

        /// <summary>
        /// Gets or sets the Team of this spawnable.
        /// </summary>
        /// <remarks>
        /// If you call Team::AddMember() / RemoveMember(), this property
        /// is automatically set / unset.
        /// Assigning to this property does not automatically add this spawnable to the team,
        /// you have to do this manually.
        /// </remarks>
        public Team Team
        {
            get
            {
                return TeamHidden;
            }

            set
            {
                TeamHidden = value;
            }
        }

        /// <summary>
        /// Gets or sets the movement keys for this character.
        /// </summary>
        public ControlKeysManager PlayerControlKeys { get; set; }

        /// <summary>
        /// Gets or sets the AI of this character.
        /// </summary>
        public AI AIComponent { get; set; }
        #endregion

        #region "protected"
        #region "movement"
        /// <summary>
        ///  Gets or sets a value indicating whether the hero is jumping now and over 0 meters in the air.
        /// </summary>
        protected bool IsJumping
        {
            get
            {
                return IsJumpingHidden;
            }

            set
            {
                if ((_net.IsServer || _net.IsClient) && Application.loadedLevelName.Equals("SM_Game"))
                {
                    networkView.RPC("NetworkSetIsJumping", RPCMode.All, value);
                }
                else
                {
                    // Running locally.
                    IsJumpingHidden = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the player is running.
        /// </summary>
        protected bool IsRunning { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the character is immune to Knockback.
        /// </summary>
        protected bool IsKnockbackImmune
        {
            get
            {
                return IsKnockbackImmuneHidden;
            }

            set
            {
                if ((_net.IsServer || _net.IsClient) && Application.loadedLevelName.Equals("SM_Game"))
                {
                    networkView.RPC("NetworkSetIsKnockbackImmune", RPCMode.All, value);
                }
                else
                {
                    // Running locally.
                    IsKnockbackImmuneHidden = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the character is immune to Damage.
        /// </summary>
        protected bool IsDamageImmune
        {
            get
            {
                return IsDamageImmuneHidden;
            }

            set
            {
                if ((_net.IsServer || _net.IsClient) && Application.loadedLevelName.Equals("SM_Game"))
                {
                    networkView.RPC("NetworkSetIsDamageImmune", RPCMode.All, value);
                }
                else
                {
                    // Running locally.
                    IsDamageImmuneHidden = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the running timeout on last time pressed(l,r,b,f)
        /// </summary>
        protected float[] RunningTimeout { get; set; }
        #endregion
        #endregion
        #endregion
        #endregion

        #region "Methodes"
        #region "public"
        #region "unity triggered"
        /// <summary>
        /// Called by Instanciate()
        /// </summary>
        public override void Awake()
        {
            base.Awake();

            // Do not use the network-aware setter; this object may not yet exist on the
            // other peers.
            MpHidden = MaxMP;

            HP = MaxHP;
            Stamina = MaxStamina;
            DieSmokePrefab = Resources.Load("Particles/DieSmokePrefab") as GameObject;
            InitAndInstantiateStatusbars();
        }

        /// <summary>
        /// called before the first call of Update()
        /// Adjusting first values which shouldn't been updated each frame
        /// </summary>
        public virtual void Start()
        {
            // moved here after switching fields into properties:
            CanMove = true;
            IsRunning = false;
            IsKnockedBack = false;
            RunningTimeout = new float[] { 0, 0, 0, 0 };

            // originally start()
            if (!_net.IsClient || this is Hero)
            {
                CharacterController = gameObject.GetComponent<CharacterController>();
                AIComponent = gameObject.GetComponent<AI>();

                Godmode = false;
                IsJumping = false;

                if (TestPlayerNo != 0)
                {   // Set player number for testing.
                    PlayerNo = TestPlayerNo;
                }

                // AI does't need the statemachine
                if (Type == Types.HUMAN && IsMine())
                {
                    // Make sure we know about the player's key mapping.
                    PlayerControlKeys = ConfigManager.GetInstance().GetControlKeysForPlayer(PlayerNo);

                    // Set up the state machine.
                    StateMachine = new StateMachine();

                    // Add default combos/keys for special attacks.
                    AddSpecialAttackKeyCombos();
                }

                // XXX: Team functionality testing code.
                if (TestTeamValue != 0)
                {
                    Team = GameManager.GetInstance().GetTeam(TestTeamValue);
                    if (Team == null)
                    {
                        Hero leader = null;

                        var hero = this as Hero;

                        if (hero != null)
                        {
                            leader = hero;
                        }

                        Team = GameManager.GetInstance().AddTeam(new Team(TestTeamValue, leader));
                    }
                    else
                    {   // Team already exists, just add our player.
                        // Debug.Log("TEAM TEST: Adding player #" + PlayerNo + " to existing team " +
                        // testTeamValue);
                        Team.AddMember(this);
                    }
                }
            }

            // cache default animations
            // AnimationExists() would be called every frame
            // no good...
            PlayWalking = AnimationExists("Walking");
            PlayRunning = AnimationExists("Running");
            PlayJumping = AnimationExists("Jumping");
            PlayFalling = AnimationExists("Falling");
            PlayLanding = AnimationExists("Landing");
            PlayStanding = AnimationExists("Standing");
        }

        /// <summary>
        /// Unity's update callback.
        /// All stuff which should be updated on each single frame.
        /// </summary>
        /// <remarks>
        /// If you override this method, call base.Update() (i. e.
        /// this implementation) as the LAST operation.
        /// </remarks>
        public void Update()
        {
            if (!Application.loadedLevelName.Equals("StatisticScreen"))
            {
                if (_setInactiveInNextFrame)
                {
                    gameObject.SetActive(false);
                    return;
                }

                if (!_net.IsClient || this is Hero)
                {
                    if (_setInactiveInNextFrame)
                    {
                        gameObject.SetActive(false);
                        return;
                    }

                    // Our player models have their center point _under_ their feet (FIXME!),
                    // so we need to move the projectile by half of our size along the Y axis
                    // to make it appear at our real center point.
                    ProjectilePos = new Vector3(
                        transform.position.x,
                        transform.position.y + collider.bounds.extents.y,
                        transform.position.z);
                    if (IsMine())
                    {
                        CalculateMovement();
                    }

                    if (Type == Types.AI)
                    {
                        AIComponent.DetectAiAction();
                    }
                    else if (Type == Types.HUMAN && IsMine())
                    {
                        // not part in _canMove case, cause defending and attacking can be performend even if i can't move
                        DetectHumanAction();
                    }

                    if (IsMine())
                    {
                        Move();
                        PerformAction();
                    }

                    // Regenerate mana points, but only if we are the server or are running locally.
                    if ((_net.IsServer || !_net.IsClient) &&
                        !IsDead() && ManaRegenerationAmount != 0 && MP < MaxMP)
                    {
                        MP += ManaRegenerationAmount * Time.deltaTime;
                    }
                }
            }
        }

        /// <summary>
        /// Set the MP value.
        /// </summary>
        /// <remarks>[RPC] Visible via network.</remarks>
        /// <param name="amount">New MP value</param>
        [RPC]
        public void NetworkSetMP(float amount)
        {
            // We use MPHidden here to prevent infinite recursion caused by the
            // network-aware MP setter.
            MpHidden = Mathf.Clamp(amount, 0, MaxMP);
        }

        /// <summary>
        /// Set CanMove bool.
        /// </summary>
        /// <remarks>[RPC] Visible via network.</remarks>
        /// <param name="canMove">New canMove value</param>
        [RPC]
        public void NetworkSetCanMove(bool canMove)
        {
            CanMoveHidden = canMove;
        }

        /// <summary>
        /// Set IsAttacking bool.
        /// </summary>
        /// <remarks>[RPC] Visible via network.</remarks>
        /// <param name="isAttacking">New isAttacking value</param>
        [RPC]
        public void NetworkSetIsAttacking(bool isAttacking)
        {
            IsAttackingHidden = isAttacking;
        }

        /// <summary>
        /// Set IsKnockbackImmune bool.
        /// </summary>
        /// <remarks>[RPC] Visible via network.</remarks>
        /// <param name="isKnockbackImmune">New IsKnockbackImmune value</param>
        [RPC]
        public void NetworkSetIsKnockbackImmune(bool isKnockbackImmune)
        {
            IsKnockbackImmuneHidden = isKnockbackImmune;
        }

        /// <summary>
        /// Set IsDamageImmune bool.
        /// </summary>
        /// <remarks>[RPC] Visible via network.</remarks>
        /// <param name="isDamageImmune">New IsDamageImmune value</param>
        [RPC]
        public void NetworkSetIsDamageImmune(bool isDamageImmune)
        {
            IsDamageImmuneHidden = isDamageImmune;
        }

        /// <summary>
        /// Unity's OnTriggerEnter event callback.
        /// </summary>
        /// <param name="other">The collider that is colliding with us.</param>
        public virtual void OnTriggerEnter(Collider other)
        {
            if (!_net.IsClient || this is Hero)
            {
                // TODO: use tags...
                switch (other.tag)
                {
                    case "Item":
                        GetHitByItem(other.GetComponent<Item>());
                        break;

                    case "Projectile":
                        if (!_net.IsClient)
                        {
                            GetHitByProjectile(other.GetComponent<Projectile>());
                        }

                        break;

                    case "SectionTrigger":
                        TriggerSectionChange(other);
                        break;
                    case "Checkpoint":
                        other.GetComponent<Checkpoint>().Enter(this);
                        break;
                    case "InstantKill":
                        if (!IsDead())
                        {
                            if (IsMine() && (_net.IsClient || _net.IsServer))
                            {
                                networkView.RPC("InstantKill", RPCMode.Others);
                            }

                            InstantKill();
                        }

                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Unity's OnTriggerExit callback.
        /// </summary>
        /// <param name="other">The collider that is colliding with us.</param>
        public void OnTriggerExit(Collider other)
        {
            // TODO: tags, see OnTriggerEnter()
            switch (other.tag)
            {
                case "Checkpoint":
                    other.GetComponent<Checkpoint>().Exit(this);
                    break;
                default:
                    break;
            }

            Item item = other.GetComponent<Item>();
            if (item == null || !item.gameObject.renderer.enabled)
            {   // Not an item.
                return;
            }

            // TODO: Is this check necessary or can we simply unset maybeItem?
            if (item == MaybeItem)
            {   // We cannot pick up this item anymore.
                MaybeItem = null;

                // Debug.Log("[BasicSpawnable.cs] Item " + item.GetType().Name + " OUT OF RANGE for pickup!");
            }
        }

        /// <summary>
        /// Draws Healthbar above of the Mob
        /// </summary>
        public void OnGUI()
        {
            if (Healthbar == null || (IsMyHero() && (Manabar == null || Staminabar == null)))
            {
                InitAndInstantiateStatusbars();
                return;
            }

            if (GameManager.GetInstance().GameMode == GameManager.Mode.PLAY)
            {
                if (this is Hero)
                {
                    return;
                }
            }

            if (this is Assets.Scripts.Mobs.Base)
            {
                return;
            }

            if (Type != Types.CHARACTER_SELECTION)
            {
                Healthbar.SetActive(true);
            }

            if ((HP == MaxHP && !(this is Hero)) || IsDead())
            {
                Healthbar.SetActive(false);
            }
            else
            {
                Vector3 position = new Vector3(transform.position.x, transform.position.y + 4.4f, transform.position.z);

                Healthbar.transform.position = UnityEngine.Camera.main.WorldToViewportPoint(position);
                Healthbar.SetActive(true);
                if (Manabar != null)
                {
                    position = new Vector3(transform.position.x, transform.position.y + 4f, transform.position.z);
                    Manabar.transform.position = UnityEngine.Camera.main.WorldToViewportPoint(position);
                    Manabar.SetActive(true);
                }

                if (Staminabar != null)
                {
                    position = new Vector3(transform.position.x, transform.position.y + 3.6f, transform.position.z);
                    Staminabar.transform.position = UnityEngine.Camera.main.WorldToViewportPoint(position);
                    Staminabar.SetActive(true);
                }

                if (NameText != null)
                {
                    NameText.transform.position = UnityEngine.Camera.main.WorldToViewportPoint(new Vector3(position.x, position.y + 1.5f, position.z));
                }

                if (_itemTexture != null)
                {
                    if (CurrentItem != null)
                    {
                        _itemTexture.GetComponent<GUITexture>().texture = CurrentItem.ItemTexture;
                    }

                    if (_itemTexture.GetComponent<GUITexture>() != null && _showItem &&
                        _itemTexture.GetComponent<GUITexture>().texture != null)
                    {
                        _itemTexture.transform.position =
                            UnityEngine.Camera.main.WorldToViewportPoint(new Vector3(position.x,
                                position.y + 2.25f,
                                position.z));
                        _itemTexture.transform.localScale = new Vector3(0.025f, 0.04f, 0.025f);
                        if (CurrentItem == null)
                        {
                            _itemTexture.GetComponent<GUITexture>().texture = null;
                        }
                    }
                }
            }

            float height = Screen.height / 100;
            float width = (HP / (float)MaxHP) * (Screen.width / 8);
            Healthbar.guiTexture.pixelInset = new Rect(-width / 2, 0, width, Screen.height / 40);
            if (Manabar != null)
            {
                width = (HP / (float)MaxHP) * (Screen.width / 8);
                Healthbar.guiTexture.pixelInset = new Rect(-width / 2, 0, width, height);
                width = (MP / (float)MaxMP) * (Screen.width / 8);
                Manabar.guiTexture.pixelInset = new Rect(-width / 2, 0, width, height);
            }

            if (Staminabar != null)
            {
                width = (Stamina / (float)MaxStamina) * (Screen.width / 8);
                Staminabar.guiTexture.pixelInset = new Rect(-width / 2, 0, width, height);
            }
        }

        /// <summary>
        /// Called when this characte going to be destroyed.
        /// ..destroys attached healthbars
        /// </summary>
        public void OnDestroy()
        {
            if (Healthbar != null)
            {
                Destroy(Healthbar);
            }
        }
        #endregion

        #region "movement"
        /// <summary>
        /// Perform a Jump
        /// </summary>
        public virtual void PerformJump()
        {
            IsJumping = true;
            animation.Play("Jumping");
            if (_net.IsServer || (_net.IsClient && this is Hero))
            {
                networkView.RPC("NetworkPlayAnimation",
                    RPCMode.Others,
                    "Jumping",
                    -1f,
                    false);
            }

            Direction.y = JumpSpeed;
        }

        /// <summary>
        /// Move this entity.
        /// </summary>
        /// <remarks>
        /// Overriding classes simply need to set Direction in their Move() method and call base.Move()
        /// (i. e. this implementation) as the LAST operation.
        /// </remarks>
        public override void Move()
        {
            if (Type == Types.CHARACTER_SELECTION)
            {   // Dead characters cannot move.
                return;
            }

            // The AI has it own rotation!
            if (Type != Types.AI && ((Direction.x != 0 || Direction.z != 0) || IsDefending))
            {
                if (IsDefending)
                {
                    float axisV = CustomInput.GetAxis(PlayerNo, "vertical");
                    float axisH = CustomInput.GetAxis(PlayerNo, "horizontal");

                    // enable rotation while defending
                    if (axisV != 0 || axisH != 0)
                    {
                        Direction = new Vector3(axisH, 0, axisV).normalized;
                        RotateTowards(Direction);
                    }
                }

                if (!IsKnockedBack && !IsDefending)
                {
                    RotateTowards(Direction);
                }
            }

            if (CharacterController == null)
            {
                // Debug.LogError("[BasicSpawnable: Move()]: no Controller set for: " + name);
            }
            else
            {
                if (!CanMove && !IsKnockedBack)
                {
                    Direction.x = 0;
                    Direction.z = 0;
                }

                CharacterController.Move(Direction * Time.deltaTime);

                if (IsJumping && CharacterController.isGrounded)
                {   // We just landed.
                    IsJumping = false;
                    IsAttacking = false;

                    // Debug.Log("[character.cs]: Knockback disabled");
                    Direction.x = 0;
                    Direction.z = 0;
                }

                if (IsKnockedBack && CharacterController.isGrounded)
                {
                    IsKnockedBack = false;

                    Direction.x = 0;
                    Direction.z = 0;
                }

                if (CharacterController.isGrounded && !IsAttacking)
                {
                    IsKnockbackImmune = false;
                }

                // If we are not on the ground, we need to simulate gravity.
                if (!CharacterController.isGrounded)
                {
                    Direction.y -= Gravity * Time.deltaTime;
                }

                if (IsMine())
                {
                    PlayMovementAnimation();
                }
            }
        }
        #endregion

        #region "receiver"
        /// <summary>
        /// Hero is receiving damage from the ground
        /// </summary>
        /// <param name="amtOfDamage">the exact amount of damage</param>
        /// <param name="attacker">The attacking character (optional, may be null).</param>
        public void ReceiveDamage(int amtOfDamage, Character attacker)
        {
            ReceiveDamage(amtOfDamage, Quaternion.Euler(Vector3.up), attacker);
        }

        /// <summary>
        /// Hero is receiving damage from a specified direction
        /// it take count of the defending state
        /// </summary>
        /// <param name="attacker">name of the attacker</param>
        /// <param name="amtOfDamage">the exact amount of damage</param>
        /// <param name="damageDirection">the direction damage is coming from</param>
        /// <remarks>
        /// [RPC] Visible via Network.
        /// calls the local receiveDamage function. Needed since the RPC calls couldn't send most class data.
        /// </remarks>
        [RPC]
        public void NetworkReceiveDamage(string attacker, int amtOfDamage, Quaternion damageDirection)
        {
            Character other = GameObject.Find(attacker) != null ? GameObject.Find(attacker).GetComponent<Character>() : null; // should be senseless but its just in case :)
            ReceiveDamage(amtOfDamage, damageDirection, other);
        }

        /// <summary>
        /// Hero is receiving damage from a specified direction
        /// it take count of the defending state
        /// </summary>
        /// <param name="amtOfDamage">the exact amount of damage</param>
        /// <param name="damageDirection">the direction damage is coming from</param>
        /// <param name="attacker">The attacking character (optional, may be null).</param>
        public void ReceiveDamage(int amtOfDamage, Quaternion damageDirection, Character attacker)
        {
            if (!Godmode && !IsDamageImmune)
            {
                int ratio = (int)transform.rotation.eulerAngles.y + (int)damageDirection.eulerAngles.y;
                if ((ratio >= 280 && ratio <= 440) && IsDefending)
                {
                    amtOfDamage = (int)((float)amtOfDamage * DefendDamagePercentage);
                }

                // Damage is dealt with a min of 0 and a max of HP
                int realAmtOfDamage = Mathf.Clamp(amtOfDamage, 0, HP);
                Debug.Log("[BasicSpawnable.cs] Entity " + name + " loses " + realAmtOfDamage + " HP");

                HP -= realAmtOfDamage;
                StatsLostHP += realAmtOfDamage;
                if (attacker != null)
                {
                    attacker.StatsEnemyDealtDamage += realAmtOfDamage;
                }

                if (IsDead())
                {
                    Die(attacker);
                }
            }

            /*if (this is Hero && (_net.IsServer || _net.IsClient))
            {
                networkView.RPC("NetworkDebugLog", RPCMode.All, ("Serverside: " + _net.IsServer + " Name: " + name + " HP: " + HP));
            }*/
        }

        /// <summary>
        /// Knockback function which is called when you git hit by an enemy
        /// [RPC]
        /// </summary>
        /// <param name="knockback">Knockback direction</param>
        /// <param name="damageDirection">Direction the damage comes from</param>
        /// <param name="projectilepos">center of the projectile that hit</param>
        /// <param name="knockbackHorizontal">value of the knockback from center to character</param>
        [RPC]
        public virtual void ReceiveKnockback(Vector3 knockback, Quaternion damageDirection, Vector3 projectilepos, float knockbackHorizontal)
        {
            if (!IsMine())
            {
                return;
            }

            int ratio = (int)transform.rotation.eulerAngles.y + (int)damageDirection.eulerAngles.y;

            if (!(((ratio >= 280 && ratio <= 440) && IsDefending) || IsKnockbackImmune))
            {
                if (AnimationExists("BeingHit"))
                {
                    animation.Play("BeingHit");
                }

                if (knockbackHorizontal > 0)
                {
                    Vector3 temp = Vector3.zero;
                    temp = (transform.position - projectilepos).normalized * knockbackHorizontal;
                    temp.y = knockback.y;
                    knockback = temp;
                }

                Direction = knockback;
                IsKnockedBack = true;
                Move();
            }
        }
        #endregion

        #region "default animations"
        /// <summary>
        /// Returns list of all animation names attached to gameObject
        /// </summary>
        /// <returns>List of animation names.</returns>
        public List<string> AnimationNames()
        {
            if (AnimationList.Count == 0 && animation != null)
            {
                foreach (AnimationState state in animation)
                {
                    AnimationList.Add(state.name);
                }
            }

            return AnimationList;
        }

        /// <summary>
        /// Checks whether an given animation exists for this character.
        /// </summary>
        /// <param name="name">name of the animation</param>
        /// <returns>true if animation is found</returns>
        public bool AnimationExists(string name)
        {
            return AnimationNames().Contains(name);
        }

        /// <summary>
        /// List of all specialattack animations of this character
        /// </summary>
        /// <returns>list of special attack names</returns>
        public List<string> SpecialAttacks()
        {
            List<string> specialAttacks = new List<string>();
            foreach (string animationName in AnimationNames())
            {
                if (animationName.StartsWith("Special"))
                {
                    specialAttacks.Add(animationName);
                }
            }

            return specialAttacks;
        }
        #endregion

        /// <summary>
        /// Function to call animations for this character via network.
        /// </summary>
        /// <param name="name"> animation name</param>
        /// <param name="fadelength"> fading length</param>
        /// <param name="cross"> using crossfade</param>
        /// <remarks>
        /// [RPC] Visible via network. Used when the claculations are serversided.
        /// </remarks>
        [RPC]
        public void NetworkPlayAnimation(string name, float fadelength, bool cross)
        {
            if (cross)
            {
                if (fadelength != -1f)
                {
                    animation.CrossFade(name, fadelength);
                }
                else
                {
                    animation.CrossFade(name);
                }
            }
            else
            {
                animation.Play(name);
            }
        }

        /// <summary>
        /// Function to spawn the character.
        /// </summary>
        /// <param name="position">lokation to spawn at</param>
        public override void Spawn(Vector3 position)
        {
            base.Spawn(position);
            _setInactiveInNextFrame = false;
            HP = MaxHP;
            MP = MaxMP;
            gameObject.SetActive(true);

            if (Type == Types.AI && AIComponent != null)
            {
                AIComponent.StartAi();
            }
        }

        /// <summary>
        /// Drop the item that is currently picked up.
        /// </summary>
        /// <returns>Whether the item was dropped (true) or not (false)</returns>
        public bool DropItem()
        {
            if (CurrentItem == null)
            {
                // Debug.Log("[BasicSpawnable.cs] Cannot drop item: No item picked up!");
                return false;
            }

            if (CurrentItem.ItemInUse)
            {   // Cannot drop item while it's in use.
                // Debug.Log("[BasicSpawnable.cs] Cannot drop item: Item is in use");
                return false;
            }

            // Make sure that there's an item to pick up after we drop this one.
            if (MaybeItem == null)
            {
                MaybeItem = CurrentItem;
            }

            // Drop the item!
            // Debug.Log("[BasicSpawnable.cs] Dropping item " + CurrentItem.GetType().Name);
            CurrentItem.Drop();
            StartCoroutine(AnimationDelay("ItemInteraction"));

            CurrentItem = null;

            return true;
        }

        /// <summary>
        /// Pick up the item that is in range of this spawnable.
        /// </summary>
        /// <returns>Whether an item was picked up (true) or not (false)</returns>
        public bool PickupItem()
        {
            if (MaybeItem == null)
            {
                // Debug.Log("[BasicSpawnable.cs] Cannot pickup item: No item in range!");
                return false;
            }

            if (CurrentItem != null)
            {
                // Debug.Log("[BasicSpawnable.cs] Cannot pickup item: Unused item " +
                // CurrentItem.GetType().Name + " already picked up!");
                return false;
            }

            // Pick up this item.
            CurrentItem = MaybeItem;
            CurrentItem.Pickup(this);
            StartCoroutine(AnimationDelay("ItemInteraction"));

            // Make sure we cannot pick up this item again...
            MaybeItem = null;

            // Debug.Log("[BasicSpawnable.cs] Picked up item " + CurrentItem.GetType().Name);
            return true;
        }

        /// <summary>
        /// The currentItem is used
        /// </summary>
        public void UseItem()
        {
            if (CurrentItem != null && !CurrentItem.ItemInUse)
            {
                // Statistics
                if (CurrentItem is Assets.Scripts.Items.HealthPack)
                {
                    if (_net.IsClient || _net.IsServer)
                    {
                        networkView.RPC("IncUsedHealthPacks", RPCMode.Others);
                    }

                    StatsUsedHealthPacks++;
                }

                // Debug.Log("[BasicSpawnable.cs] Using item " + CurrentItem.GetType().Name);
                StartCoroutine(AnimationDelay("ItemConsumption"));
                CurrentItem.Use();
            }
        }

        /// <summary>
        /// Add a specific amount of health to the character.
        /// </summary>
        /// <remarks>
        /// If the amount is greater than this characters life it's fillen up until the maximum of health.
        /// Visible via network.
        /// </remarks>
        /// <param name="amount">amount of health to add</param>
        /// <param name="sendRPC">If a RPC should be sent to Client or Server if in Specialmode otherwise it has no effect</param>
        [RPC]
        public void AddHealth(int amount, bool sendRPC)
        {
            HP += amount;
            if ((_net.IsServer || _net.IsClient) && sendRPC)
            {
                networkView.RPC("AddHealth", RPCMode.Others, amount, false);
            }
        }

        /// <summary>
        /// Checks if the hero is dead.
        /// </summary>
        /// <returns>true if the hero's HP is equal 0</returns>
        public bool IsDead()
        {
            return HP <= 0;
        }

        /// <summary>
        /// Animate and let character die
        /// </summary>
        /// <remarks>
        /// Overridden for some special stuff, e.g. removing the healthbar, ...
        /// </remarks>
        /// <param name="killer">The character that killed this entity (optional).</param>
        public virtual void Die(Character killer)
        {
            if (IsMine() && (_net.IsServer || _net.IsClient))
            {
                networkView.RPC("NetworkDie", RPCMode.Others);
            }

            CanMove = false;

            foreach (Checkpoint c in GameManager.GetInstance().Checkpoints)
            {
                c.NotifyDeath(this);
            }

            if (GameManager.GetInstance().GameMode == GameManager.Mode.SPECIAL)
            {
                if (this is Mob || this is Mercenary)
                {
                    Team.RemoveMember(this);
                }
                else
                {
                    // No, this is Patrick!
                }
            }

            // Keep track of the attacker's kill count.
            if (killer != null)
            {
                killer.StatsKillCount++;
            }

            StartCoroutine(AnimateDeath());

            if (DyingSound != null)
            {
                StartCoroutine(PlayDeathSound());
            }
        }

        /// <summary>
        /// Dying methode callable via Network
        /// [RPC]
        /// </summary>
        [RPC]
        public void NetworkDie()
        {
            CanMove = false;

            foreach (Checkpoint c in GameManager.GetInstance().Checkpoints)
            {
                c.NotifyDeath(this);
            }

            StartCoroutine(AnimateDeath());

            if (DyingSound != null)
            {
                StartCoroutine(PlayDeathSound());
            }
        }

        /// <summary>
        /// Sets the IsKnockedBack value.
        /// [RPC] (Used via Network.)
        /// </summary>
        /// <param name="isKnockedBack">New IsKnockedBack value</param>
        [RPC]
        public void NetworkSetIsKnockedBack(bool isKnockedBack)
        {
            IsKnockedBackHidden = isKnockedBack;
        }

        /// <summary>
        /// Sets the IsJumping value.
        /// [RPC] (Used via Network.)
        /// </summary>
        /// <param name="isJumping">New IsJumping value</param>
        [RPC]
        public void NetworkSetIsJumping(bool isJumping)
        {
            IsJumpingHidden = isJumping;
        }

        /// <summary>
        /// Methode that kills this character in one call.
        /// [RPC]
        /// </summary>
        [RPC]
        public void InstantKill()
        {
            if (IsDead())
            {
                return;
            }

            HP = 0;
            ReceiveDamage(0, null);
        }

        /// <summary>
        /// This methods performs the decidet action
        /// this function is called every frame (like the Move function)
        /// </summary>
        public void PerformAction()
        {
            if (IsDead())
            {
                return;
            }

            switch (NextAction)
            {
                case Action.ATTACK:
                    // Debug.Log("perform attack by " + name);
                    PerformAttack();
                    break;

                case Action.DEFEND:
                    // Debug.Log("perform defend by " + name);
                    PerformDefend();
                    break;

                case Action.SPECIAL_ATTACK:
                    // Debug.Log("perform special attack by " + name);
                    PerformSpecialAttack();
                    break;

                case Action.PICKUP_ITEM:
                    PickupItem();
                    break;

                case Action.USE_ITEM:
                    UseItem();
                    break;

                case Action.DROP_ITEM:
                    DropItem();
                    break;

                case Action.NONE:
                default:
                    // reset the stuff from other actions
                    // TODO: is there more to reset?
                    if (IsDefending)
                    {
                        CanMove = true;
                        IsDefending = false;
                    }

                    break;
            }
        }

        /// <summary>
        /// Returns the position of the character's feet.
        /// </summary>
        /// <returns>The position of the character's feet.</returns>
        public Vector3 GetFeetPosition()
        {
            return transform.position - (Vector3.up * CharacterController.height * 0.5F);
        }
        #endregion

        #region "protected"

        #region "movement"
        /// <summary>
        /// Claculates how the characters movement is. (Humanplayed movements AND AI movements)
        /// </summary>
        protected void CalculateMovement()
        {
            CanMove = CanMove && (HpHidden != 0);
            if (CanMove)
            {
                if (Type == Types.AI)
                {
                    AIComponent.CalculateAiMovement();
                }
                else if (Type == Types.HUMAN)
                {
                    DetectRunning();
                    DetectJumping();
                    CalculateStamina();

                    CalculateHumanMovement();
                }
            }
        }

        /// <summary>
        /// Detects running by human played characters
        /// </summary>
        protected void DetectRunning()
        {
            if (!IsMine())
            {
                return;
            }

            if (!IsRunning && CanMove)
            {
                if (ControlKeysManager.GetKeyDown(PlayerControlKeys.LeftKey))
                {
                    if ((Time.time - RunningTimeout[0]) < 0.5f)
                    {
                        IsRunning = true;
                    }

                    RunningTimeout[0] = Time.time;
                }

                if (ControlKeysManager.GetKeyDown(PlayerControlKeys.RightKey))
                {
                    if ((Time.time - RunningTimeout[1]) < 0.5f)
                    {
                        IsRunning = true;
                    }

                    RunningTimeout[1] = Time.time;
                }

                if (ControlKeysManager.GetKeyDown(PlayerControlKeys.BackwardKey))
                {
                    if ((Time.time - RunningTimeout[2]) < 0.5f)
                    {
                        IsRunning = true;
                    }

                    RunningTimeout[2] = Time.time;
                }

                if (ControlKeysManager.GetKeyDown(PlayerControlKeys.ForwardKey))
                {
                    if ((Time.time - RunningTimeout[3]) < 0.5f)
                    {
                        IsRunning = true;
                    }

                    RunningTimeout[3] = Time.time;
                }

                if (ControlKeysManager.GetKeyDown(PlayerControlKeys.RunKey))
                {
                    IsRunning = true;
                }
            }

            if (CustomInput.GetAxis(PlayerNo, "horizontal") == 0 && CustomInput.GetAxis(PlayerNo, "vertical") == 0)
            {
                IsRunning = false;
            }
        }

        /// <summary>
        /// detects jumping by human played characters
        /// </summary>
        protected void DetectJumping()
        {
            if (!IsMine())
            {
                return;
            }

            if (_net.IsClient || _net.IsServer)
            {
                if (!networkView.isMine)
                {
                    return;
                }
            }

            if (IsJumping || !CharacterController.isGrounded || !CanMove)
            {   // Can't jump twice or while falling/attacking.
                return;
            }

            if (ControlKeysManager.GetKeyDown(PlayerControlKeys.JumpKey))
            {
                PerformJump();
            }
        }

        /// <summary>
        /// Calculates the Stamina usage and regeneration.
        /// Checks if the Character can run (Stamina empty?).
        /// </summary>
        protected void CalculateStamina()
        {
            if (Stamina == 0)
            {
                IsRunning = false;
            }

            if (IsRunning)
            {
                Stamina -= StaminaUseRate * Time.deltaTime;
            }
            else
            {
                Stamina += StaminaRegRate * Time.deltaTime;
            }
        }

        /// <summary>
        /// calculates the new direction for human played characters
        /// </summary>
        protected void CalculateHumanMovement()
        {
            if (CharacterController.isGrounded)
            {
                float axisV = CustomInput.GetAxis(PlayerNo, "vertical");
                float axisH = CustomInput.GetAxis(PlayerNo, "horizontal");

                Direction = (new Vector3(axisH, 0, axisV).normalized * MovementSpeed) + (Vector3.up * Direction.y);

                if (IsRunning)
                {
                    Direction.x *= RunningFactor;
                    Direction.z *= RunningFactor;
                }
            }
        }

        /// <summary>
        /// Rotate the character towards a given point.
        /// </summary>
        /// <param name="dir">The direction to look into.</param>
        protected virtual void RotateTowards(Vector3 dir)
        {
            Vector3 euler = Quaternion.LookRotation(dir).eulerAngles;
            euler.z = 0;
            euler.x = 0;
            transform.rotation = Quaternion.Euler(euler);
        }
        #endregion

        #region "actions"
        /// <summary>
        /// Performs the actual attack
        /// must be overritten by the child class, e.g. CockMan, ...
        /// </summary>
        /// <remarks>
        /// The attack delay is also implemented in the override method!
        /// </remarks>
        protected abstract void PerformAttack();

        /// <summary>
        /// Block all movement while attacking.
        /// </summary>
        /// <param name="time">Duration of the attack</param>
        /// <returns>An IEnumerator indicating how long to block all movement.</returns>
        protected IEnumerator AttackDelay(float time)
        {
            yield return new WaitForSeconds(time);

            CanMove = true;
            IsAttacking = false;
            IsKnockbackImmune = false;
            IsDamageImmune = false;
        }

        /// <summary>
        /// Block all movement while attacking.
        /// </summary>
        /// <remarks>
        /// Plays the named animation and waits as long as the animation plays.
        /// </remarks>
        /// <param name="animationName">Animation to use.</param>
        /// <returns>An IEnumerator indicating how long to block all movement.</returns>
        protected IEnumerator AttackDelay(string animationName)
        {
            if (!IsJumping)
            {
                Direction.x = 0;
                Direction.z = 0;
            }

            CanMove = false;
            IsAttacking = true;

            animation.Play(animationName);
            if (_net.IsServer || (_net.IsClient && this is Hero))
            {
                networkView.RPC("NetworkPlayAnimation",
                    RPCMode.Others,
                    animationName,
                    -1f,
                    false);
            }

            return AttackDelay((animation[animationName].length /
                animation[animationName].speed) + AttackRechargeTime);
        }

        /// <summary>
        /// Block all movement while playing an animation.
        /// </summary>
        /// <remarks>
        /// Plays the named animation and waits as long as the animation plays.
        /// </remarks>
        /// <param name="animationName">Animation to use.</param>
        /// <returns>An IEnumerator indicating how long to block all movement.</returns>
        protected IEnumerator AnimationDelay(string animationName)
        {
            if (!AnimationExists(animationName))
            {
                Debug.LogWarning(name + " has no animation " + animationName + "! Delay will be 0.");
                yield break;
            }

            animation.Play(animationName);
            if (_net.IsServer || (_net.IsClient && this is Hero))
            {
                networkView.RPC("NetworkPlayAnimation",
                    RPCMode.Others,
                    animationName,
                    -1f,
                    false);
            }

            if (!IsJumping)
            {
                Direction.x = 0;
                Direction.z = 0;
            }

            CanMove = false;

            yield return new WaitForSeconds(Mathf.Abs(animation[animationName].length /
                                                      animation[animationName].speed));

            CanMove = true;
        }

        /// <summary>
        /// Performs the actual block
        /// see Perform attack
        /// </summary>
        protected virtual void PerformDefend()
        {
            // TODO: should there be a release delay?
            if (AnimationExists("Defending"))
            {
                // NB: We need to use Play() here; CrossFade() does not work properly.
                animation.Play("Defending");
                if (_net.IsServer || (_net.IsClient && this is Hero))
                {
                    networkView.RPC("NetworkPlayAnimation",
                        RPCMode.Others,
                        "Defending",
                        -1f,
                        false);
                }
            }

            Direction = Vector3.zero;
            CanMove = false;

            IsDefending = true;
        }

        /// <summary>
        /// this performs the current special attack, detected by the statemachine
        /// </summary>
        protected virtual void PerformSpecialAttack()
        {
            if (SpecialAttack != null)
            {
                SpecialAttack();
                SpecialAttack = null;
            }
        }

        /// <summary>
        /// Add the key combos for the special attacks of this Character.
        /// </summary>
        protected virtual void AddSpecialAttackKeyCombos()
        {
            // Add all the keys that are most likely used for special attack combos.
            StateMachine.AddKeyCode(PlayerControlKeys.AttackKey);
            StateMachine.AddKeyCode(PlayerControlKeys.DefendKey);
            StateMachine.AddKeyCode(PlayerControlKeys.JumpKey);
            StateMachine.AddKeyCode(PlayerControlKeys.PickupKey);
            StateMachine.AddKeyCode(PlayerControlKeys.DropItemKey);
            StateMachine.AddKeyCode(PlayerControlKeys.ForwardKey);
            StateMachine.AddKeyCode(PlayerControlKeys.BackwardKey);
            StateMachine.AddKeyCode(PlayerControlKeys.LeftKey);
            StateMachine.AddKeyCode(PlayerControlKeys.RightKey);
            StateMachine.AddKeyCode(PlayerControlKeys.CheatKey);

            // Now initialize the state chart.
            StateMachine.InitStateChart();
        }

        /// <summary>
        /// Detects the next action
        /// called each frame if character is human controlled
        /// </summary>
        protected void DetectHumanAction()
        {
            NextAction = Action.NONE;
            if (IsMine())
            {
                DetectAttack();
                DetectDefend();
                DetectItem();

                // special attack has to be the last one, because attack and defend key can be part of an special attack.
                // so this overrides the previous action if the pressed key is the last one of a special attack
                DetectSpecialAttack();
            }
        }

        /// <summary>
        /// Detect human performed attack
        /// </summary>
        protected void DetectAttack()
        {
            if (!IsMine())
            {
                return;
            }

            if (IsRunning || IsAttacking || IsKnockedBack)
            {   // Cannot attack while running or while another attack is executing.
                return;
            }

            if (ControlKeysManager.GetKeyDown(PlayerControlKeys.AttackKey))
            {
                NextAction = Action.ATTACK;
            }
        }

        /// <summary>
        /// Detect human performed defend
        /// </summary>
        protected void DetectDefend()
        {
            if (IsRunning || IsJumping || IsAttacking)
            {   // Cannot defend while running, jumping or attacking.
                return;
            }

            // defend as long as the defend key is pressed
            if (ControlKeysManager.GetKey(PlayerControlKeys.DefendKey))
            {
                NextAction = Action.DEFEND;
            }
        }

        /// <summary>
        /// Detecte human performed item actions
        /// </summary>
        protected void DetectItem()
        {
            if (!IsMine())
            {
                return;
            }

            if (IsRunning || IsJumping || IsAttacking)
            {   // Cannot pick up/drop items while running, jumping or attacking.
                return;
            }

            if (ControlKeysManager.GetKeyDown(PlayerControlKeys.PickupKey))
            {
                if (CurrentItem == null)
                {
                    NextAction = Action.PICKUP_ITEM;
                }
                else
                {
                    NextAction = Action.USE_ITEM;
                }
            }

            if (ControlKeysManager.GetKeyDown(PlayerControlKeys.DropItemKey))
            {
                NextAction = Action.DROP_ITEM;
            }
        }

        /// <summary>
        /// Detect human performed special attack
        /// </summary>
        protected void DetectSpecialAttack()
        {
            if (IsRunning || IsAttacking)
            {   // Cannot attack while running or while another attack is executing.
                return;
            }

            // update state machine
            StateMachine.Check();

            // player did correct input for special attack
            if (StateMachine.SpecialAttackDetected())
            {
                // set the parameter for PerformAction()
                SpecialAttack = StateMachine.DetectedSpecialAttack;
                NextAction = Action.SPECIAL_ATTACK;
            }
        }
        #endregion

        /// <summary>
        /// Sound played on death
        /// </summary>
        /// <returns>time it takes to played until the end</returns>
        protected IEnumerator PlayDeathSound()
        {
            DyingSound.Play();
            yield return new WaitForSeconds(DyingSound.time);
        }

        /// <summary>
        /// the actual charaters death animation
        /// </summary>
        /// <returns>An IEnumerator indicating how long the death animation takes.</returns>
        protected IEnumerator AnimateDeath()
        {
            if (AnimationExists("Dying"))
            {
                // Debug.Log("Playing Death Animation");
                animation.Play("Dying");
                /*
                if (_net.IsServer || _net.IsClient && this is Hero)
                {
                    networkView.RPC("NetworkPlayAnimation", RPCMode.Others, "Dying", -1f, true);
                }
                */
                yield return new WaitForSeconds(
                    animation["Dying"].length / animation["Dying"].speed);
            }

            if (Team != null)
            {
                Team.NotifyMemberDead(this);
            }

            if (this is Mob || this is Mercenary)
            {
                Instantiate(DieSmokePrefab, transform.position, DieSmokePrefab.transform.rotation);
            }

            if (GameManager.GetInstance().GameMode != GameManager.Mode.SPECIAL)
            {
                _setInactiveInNextFrame = true;
            }
            else
            {
                if (this is Mob || this is Mercenary)
                {
                    if (_net.IsServer)
                    {
                        Network.Destroy(this.gameObject);
                    }
                    else if (_net.IsClient)
                    {
                    }
                    else
                    {
                        Destroy(this.gameObject);
                    }
                }
            }
        }

        /// <summary>
        /// Indicate that we are being hit by a projectile.
        /// </summary>
        /// <param name="projectile">The hitting projectile.</param>
        protected virtual void GetHitByProjectile(Projectile projectile)
        {
            // Debug.Log("Hit by projectile");
            if (projectile.Player == null)
            {
                ApplyProjectileHit(projectile);
            }
            else if (!projectile.Player.gameObject.Equals(this.gameObject) && !projectile.ForkTriggerEnemy == this &&
                (Team == null || projectile.Player.Team == null || !projectile.Player.Team.Equals(Team)))
            {   // Character got hit by an attack (of the enemy team).
                // If we don't have a team, everything is an enemy.
                ApplyProjectileHit(projectile);
            }
        }

        /// <summary>
        /// Applies Damage and Knockback of the projectile.
        /// </summary>
        /// <param name="projectile">The hitting projectile</param>
        protected void ApplyProjectileHit(Projectile projectile)
        {
            if (projectile != null)
            {
                if (_net.IsServer)
                { // server says to all connected that this character got damaged
                    networkView.RPC("NetworkSetIsKnockedBack", RPCMode.All, true);
                    string name = projectile.Player != null ? projectile.Player.name : string.Empty;
                    networkView.RPC("NetworkReceiveDamage", RPCMode.All, name, projectile.Damage, projectile.transform.rotation);
                    if (projectile.AoEKnockbackHor > 0)
                    {
                        networkView.RPC("ReceiveKnockback", RPCMode.All, projectile.Knockback, projectile.transform.rotation, projectile.transform.position, projectile.AoEKnockbackHor);
                    }
                    else
                    {
                        networkView.RPC("ReceiveKnockback", RPCMode.All, projectile.Knockback, projectile.transform.rotation, Vector3.zero, 0f);
                    }
                }
                else
                { // != in network
                    ReceiveDamage(projectile.Damage, projectile.transform.rotation, projectile.Player);
                    if (projectile.AoEKnockbackHor > 0)
                    {
                        ReceiveKnockback(projectile.Knockback, projectile.transform.rotation, projectile.transform.position, projectile.AoEKnockbackHor);
                    }
                    else
                    {
                        ReceiveKnockback(projectile.Knockback, projectile.transform.rotation, Vector3.zero, 0);
                    }
                }
            }
            else
            {
                Debug.LogError("SOMETHING vanished ^^ ...like a projectile");
            }
        }

        /// <summary>
        /// Initialises the healthbar for the actual gamemode.
        /// </summary>
        protected void InitAndInstantiateStatusbars()
        {
            if (GameManager.GetInstance().GameMode == GameManager.Mode.PLAY)
            {
                HealthbarPrefab = (GameObject)Resources.Load("HealthbarPrefab");
            }
            else if (GameManager.GetInstance().GameMode == GameManager.Mode.SPECIAL)
            {
                if (TeamHidden == null)
                {
                    return;
                }

                if (TeamHidden.TeamNo == 2 || IsMine())
                {
                    HealthbarPrefab = (GameObject)Resources.Load("HealthbarPrefab");
                }
                else
                {
                    HealthbarPrefab = (GameObject)Resources.Load("ManabarPrefab");
                }

                if (IsMine())
                {
                    ManabarPrefab = (GameObject)Resources.Load("ManabarPrefab");
                    StaminabarPrefab = (GameObject)Resources.Load("StaminabarPrefab");
                    _showItem = true;
                    _itemTexture = (GameObject)Instantiate(new GameObject());
                    _itemTexture.AddComponent<GUITexture>();
                    _itemTexture.GetComponent<GUITexture>().texture = null;
                }

                if (this is Hero)
                {
                    NamePrefab = (GameObject)Resources.Load("NamePrefab");
                    NamePrefab.GetComponent<GUIText>().text = DisplayName;
                }
            }

            if (HealthbarPrefab != null)
            {
                Healthbar = (GameObject)Instantiate(HealthbarPrefab, UnityEngine.Camera.main.WorldToViewportPoint(new Vector3(1, 1, 0)), Quaternion.identity);
                Healthbar.SetActive(false);
            }
            else
            {
                Healthbar = null;
            }

            if (ManabarPrefab != null)
            {
                Manabar = (GameObject)Instantiate(ManabarPrefab, UnityEngine.Camera.main.WorldToViewportPoint(new Vector3(1, 1, 0)), Quaternion.identity);
                Manabar.SetActive(false);
            }
            else
            {
                Manabar = null;
            }

            if (StaminabarPrefab != null)
            {
                Staminabar = (GameObject)Instantiate(StaminabarPrefab, UnityEngine.Camera.main.WorldToViewportPoint(new Vector3(1, 1, 0)), Quaternion.identity);
                Staminabar.SetActive(false);
            }
            else
            {
                Staminabar = null;
            }

            if (NamePrefab != null)
            {
                NameText = (GameObject)Instantiate(NamePrefab, UnityEngine.Camera.main.WorldToViewportPoint(new Vector3(1, 1, 0)), Quaternion.identity);
                NameText.GetComponent<GUIText>().color = (this.Team.TeamNo == 1) ? Color.blue : Color.red;
            }
        }

        /// <summary>
        /// Checks whether this object is originally instantiated in this running applications code.
        /// </summary>
        /// <returns>true: instantiated within this applications code, false: somewhere else</returns>
        protected bool IsMine()
        {
            if (_net.IsClient || _net.IsServer)
            {
                if (!networkView.isMine)
                {
                    return false;
                }

                return true;
            }

            return true;
        }
        #endregion

        #region "private"
        /// <summary>
        /// Increases the statistic stat HealthPacksUsed.
        /// </summary>
        /// <remarks>
        /// Visible via network.
        /// </remarks>
        [RPC]
        private void IncUsedHealthPacks()
        {
            StatsUsedHealthPacks++;
        }

        private bool IsMyHero()
        {
            if (_net.IsServer)
            {
                return this.name.StartsWith("HeroServer");
            }
            else if (_net.IsClient)
            {
                return this.name.StartsWith("HeroClient");
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// plays the correct movement animation
        /// </summary>
        private void PlayMovementAnimation()
        {
            if (animation == null || IsDead())
            {
                return;
            }

            if (Direction.y < 0 && !CharacterController.isGrounded)
            {
                if (PlayFalling && !IsDead())
                {
                    animation.CrossFade("Falling");
                    if (_net.IsServer || (_net.IsClient && this is Hero))
                    {
                        networkView.RPC("NetworkPlayAnimation",
                            RPCMode.Others,
                            "Falling",
                            -1f,
                            true);
                    }
                }
            }
            else if (IsJumping && CharacterController.isGrounded)
            {   // Only play jump animation once.
                if (PlayJumping)
                {
                    animation.CrossFade("Jumping");
                    if (_net.IsServer || (_net.IsClient && this is Hero))
                    {
                        networkView.RPC("NetworkPlayAnimation",
                            RPCMode.Others,
                            "Jumping",
                            -1f,
                            true);
                    }
                }
            }
            else if (!IsJumping && IsRunning)
            {
                if (PlayRunning)
                {
                    animation.CrossFade("Running");
                    if (_net.IsServer || (_net.IsClient && this is Hero))
                    {
                        networkView.RPC("NetworkPlayAnimation",
                            RPCMode.Others,
                            "Running",
                            -1f,
                            true);
                    }
                }
            }
            else if (!IsJumping && (Direction.x != 0 || Direction.z != 0))
            {
                if (PlayWalking)
                {
                    animation.CrossFade("Walking");
                    if (_net.IsServer || (_net.IsClient && this is Hero))
                    {
                        networkView.RPC("NetworkPlayAnimation",
                            RPCMode.Others,
                            "Walking",
                            -1f,
                            true);
                    }
                }
            }
            else if (CharacterController.isGrounded && !IsAttacking &&
                !(animation.IsPlaying("ItemConsumption") || animation.IsPlaying("ItemInteraction")))
            {
                if (PlayStanding)
                {
                    animation.CrossFade("Standing", 0.1f);
                    if (_net.IsServer || (_net.IsClient && this is Hero))
                    {
                        networkView.RPC("NetworkPlayAnimation",
                            RPCMode.Others,
                            "Standing",
                            0.1f,
                            true);
                    }
                }
            }
        }

        /// <summary>
        /// Function that can receive strings from server AND client.
        /// It will print it in the Debug
        /// [RPC]
        /// </summary>
        /// <param name="log">String to print</param>
        [RPC]
        private void NetworkDebugLog(string log)
        {
            Debug.Log(log);
        }

        /// <summary>
        /// Called when we collide with an item.
        /// </summary>
        /// <param name="item">The item that is colliding with us.</param>
        private void GetHitByItem(Item item)
        {
            if (item == null || item.IsPickedUp())
            {   // There's no item we can pick up.
                return;
            }

            // Debug.Log("[BasicSpawnable.cs] Item " + item.GetType().Name + " in range for pickup!");
            MaybeItem = item;
        }

        /// <summary>
        /// this is called in OnTriggerEnter()
        /// This respawns dead human controlled comrades on section change (in playmode)
        /// </summary>
        /// <param name="other">The collider that triggered the section change.</param>
        private void TriggerSectionChange(Collider other)
        {
            int sectionNo = GameManager.GetInstance().CurrentSection.SectionNumber;
            for (int i = 0; i < GameManager.GetInstance().CurrentLevel.Sections.Count; i++)
            {
                Section s = GameManager.GetInstance().CurrentLevel.Sections[i];

                // Is the Object I collided with the Section s?
                if (other.gameObject.transform.parent == s.transform)
                {
                    // Last Section End and all Mobs Killed? -> Level Complete
                    if (i == GameManager.GetInstance().CurrentLevel.Sections.Count - 1)
                    {
                        if (other.name == "ExitTrigger" && s.RemainingMobs() <= 0)
                        {
                            GameManager.GetInstance().NotifyGameEnd();
                        }
                    }

                    if (s.SectionNumber > sectionNo)
                    {
                        Debug.Log("Changed to section " + s.SectionNumber);
                        GameManager.GetInstance().CurrentSection = s;
                        GameManager.GetInstance().SectionNo = s.SectionNumber;
                        if (!s.MobSpawner.IsActive())
                        {
                            GameManager.GetInstance().CurrentSection.MobSpawner.StartSpawnRoutine();
                        }

                        if (!s.MercSpawner.IsActive())
                        {
                            GameManager.GetInstance().CurrentSection.MercSpawner.StartSpawnRoutine();
                        }
                    }

                    // rescan new section for AI
                    AIHelper.GraphBuilder graphBuilder = AIHelper.GraphBuilder.GetInstance();
                    if (graphBuilder != null)
                    {
                        graphBuilder.TryMapScanning();
                    }

                    if (other.name == "EntranceTrigger")
                    {
                        // Respawn my dead team mates team in the new section
                        Team.RespawnDeadMember();
                    }

                    // finished new section stop running throw all sections...
                    return;
                }
            }
        }

        #endregion
        #endregion
    }
}
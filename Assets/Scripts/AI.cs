namespace Assets.Scripts
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using AIHelper;
    using Map;
    using UnityEngine;

    /// <summary>
    /// The AI for computer controled heroes, mobs and mercs
    /// </summary>
    /// <remarks>
    /// Using three-routine-system
    /// Routine 1: Game loop (each frame) - Calculates movement and action
    /// Routine 2: Coroutine (temporarily) searching for near targets (Mobs, Items, ...)
    /// Routine 3: Coroutine (temporarily) searching for distance targets (Waypoints, Checkpoints, ...)
    /// Important:
    /// Routine 2 and 3 are just used / started, if current near or distance targets are invalid
    /// </remarks>
    public sealed class AI : AIPath
    {
        private const float INACTIVERANGE = 35f;
        private const float MAPNEARSCANRATE = 1.5f;
        private const float MAPNEARSCANRANGE = 22f;
        private const float MAPDISTANCESCANRATE = 3.5f;
        private const float MAXFOLLOWTIME = 10.0f;

        private Section _currentSection;
        private GameManager _gameManager;
        private bool _aiStarted;

        private DistanceHolder _distanceHolder;
        private bool _isNearTargetMapScanning;
        private bool _isDistanceTargetMapScanning;

        private Character _basicComponent;
        private TargetTypes _targetType;

        private Transform _nearTarget;
        private TargetTypes _nearTargetType;

        private Transform _distanceTarget;
        private TargetTypes _distanceTargetType;
        private int _distanceTargetIndex;

        private float _currentFollowTime;

        /// <summary>
        /// Type of the current target in AI
        /// </summary>
        private enum TargetTypes
        {
            NONE,
            ITEM,
            CHARACTER,
            WAYPOINT,
            CHECKPOINT
        }

        /// <summary>
        /// Gets or sets a value indicating whether current Ai is from an elite mob
        /// </summary>
        public bool IsElite { get; set; }

        /// <summary>
        /// Get Basic stuff
        /// completly skipped, if BasicSpawnable.Type isn't KI
        /// </summary>
        public override void Start()
        {
            _targetType = TargetTypes.NONE;
            _gameManager = GameManager.GetInstance();

            /*if (_gameManager.NetworkController.IsClient)
            {
                return;
            }*/

            _basicComponent = GetComponent<Character>();

            _gameManager = GameManager.GetInstance();

            if (_gameManager.GameMode == GameManager.Mode.PLAY)
            {
                _currentSection = _gameManager.CurrentSection;
            }

            if (_basicComponent.Team != null)
            {
                StartAi();
            }

            base.Start();
        }

        /// <summary>
        /// Starts the ai
        /// </summary>
        public void StartAi()
        {
            // we're  controlled by keyboard, skip this...
            if (_basicComponent.Type != Character.Types.AI)
            {
                return;
            }

            /*if (_gameManager.NetworkController.IsClient)
            {
                return;
            }*/

            // initiate some basic values
            speed = _basicComponent.MovementSpeed;
            pickNextWaypointDist = 1f;
            turningSpeed = 5f;
            endReachedDistance = 0.6f;

            _nearTarget = null;
            _distanceTarget = null;
            _distanceTargetIndex = 0;

            canSearch = true;

            StartCoroutine(DistanceTargetMapScanning());
            StartCoroutine(NearTargetMapScanning());

            _aiStarted = true;
        }

        /// <summary>
        /// Detects section change in Play mode
        /// </summary>
        public override void Update()
        {
            if (!_aiStarted ||
                _gameManager.NetworkController.IsClient ||
                _gameManager.GameMode != GameManager.Mode.PLAY ||
                _currentSection == _gameManager.CurrentSection)
            {
                return;
            }

            Debug.Log("[AI] Section change detected");

            StopCoroutine("DistanceTargetMapScanning");
            currentWaypointIndex = 0;
            _distanceTarget = null;

            if (_targetType == TargetTypes.WAYPOINT)
            {
                target = null;
            }

            _currentSection = _gameManager.CurrentSection;
            StartCoroutine(DistanceTargetMapScanning());
        }

        /// <summary>
        /// Calculates moving towards target
        /// </summary>
        public void CalculateAiMovement()
        {
            if (!_aiStarted || _basicComponent == null || _basicComponent.Team == null)
            {
                return;
            }

            _currentFollowTime += Time.deltaTime;

            ValidateTarget();
            CalculateDirection();
        }

        /// <summary>
        /// Performs an action based on target type
        /// </summary>
        public void DetectAiAction()
        {
            if (!_aiStarted || _basicComponent == null || _targetType == TargetTypes.NONE)
            {
                return;
            }

            _basicComponent.NextAction = Character.Action.NONE;

            if (!TargetReached)
            {
                return;
            }

            _currentFollowTime = 0;

            if (_basicComponent.IsAttacking)
            {
                return;
            }

            switch (_targetType)
            {
                case TargetTypes.ITEM:
                    _basicComponent.NextAction = Character.Action.PICKUP_ITEM;
                    break;
                case TargetTypes.CHARACTER:
                    if (_nearTarget != null)
                    {
                        RotateTowards(_nearTarget.position - tr.position, false);
                        _basicComponent.NextAction = Character.Action.ATTACK;
                    }
                    else
                    {
                        _basicComponent.NextAction = Character.Action.NONE;
                    }

                    break;
                case TargetTypes.WAYPOINT:
                    _basicComponent.NextAction = Character.Action.NONE;
                    currentWaypointIndex++;
                    break;
                case TargetTypes.CHECKPOINT:
                    StartCoroutine(CaptureCheckpoint());
                    break;
            }

            if (_basicComponent.CurrentItem != null && _basicComponent.CanMove)
            {
                _basicComponent.NextAction = Character.Action.USE_ITEM;
            }
        }

        /// <summary>
        /// Sets the distance holder for the new target in a given distance
        /// </summary>
        /// <param name="newTarget">target for distance holder</param>
        /// <param name="distance">distance between distance holder and target</param>
        /// <param name="randomize">randomize distance holder around target</param>
        private void SetTarget(Transform newTarget, float distance, bool randomize)
        {
            if (_distanceHolder == null)
            {
                var gameObj = Instantiate(Resources.Load("AI/DistanceHolder")) as GameObject;

                System.Diagnostics.Debug.Assert(gameObj != null, "gameObj != null");

                _distanceHolder = gameObj.GetComponent<DistanceHolder>();

                System.Diagnostics.Debug.Assert(_distanceHolder != null, "_distanceHolder != null");

                _distanceHolder.AI = transform;
            }

            _distanceHolder.Target = newTarget;
            _distanceHolder.DistanceToTarget = distance;
            _distanceHolder.Randomize = randomize;
            _distanceHolder.CalculatePosition();

            target = _distanceHolder.transform;
            canSearch = true;
        }

        private void SetTarget()
        {
            float distance;
            bool randomize = false;
            Transform currentTarget = null;

            if (_nearTarget != null && IsValidNearTarget(_nearTarget.GetComponent<AbstractSpawnable>()))
            {
                _currentFollowTime = 0;
                _targetType = _nearTargetType;
                currentTarget = _nearTarget;
            }
            else if (_distanceTarget != null && IsValidDistanceTarget(_distanceTarget))
            {
                _targetType = _distanceTargetType;
                currentTarget = _distanceTarget;
                StartCoroutine(NearTargetMapScanning());
            }
            else
            {
                _targetType = TargetTypes.NONE;

                StartCoroutine(DistanceTargetMapScanning());
                StartCoroutine(NearTargetMapScanning());
            }

            if (_targetType == TargetTypes.NONE)
            {
                return;
            }

            switch (_targetType)
            {
                case TargetTypes.CHARACTER:
                    distance = _basicComponent.MeeleRange;
                    break;
                case TargetTypes.CHECKPOINT:
                    randomize = true;
                    distance = 4.5f;
                    break;
                case TargetTypes.ITEM:
                    distance = 0f;
                    break;
                default:
                    distance = 1.5f;
                    break;
            }

            SetTarget(currentTarget, distance, randomize);
        }

        /// <summary>
        /// recalculates target every frame
        /// maybe current target is invalid (dead, used, ...)
        /// </summary>
        private void ValidateTarget()
        {
            if (target == null)
            {
                SetTarget();
                return;
            }

            if (_distanceHolder.Target == null)
            {
                target = null;
            }
            else if (_distanceHolder.Target.GetComponent<Character>() != null &&
                (_distanceHolder.Target.GetComponent<Character>().IsDead() ||
                 Vector3.Distance(transform.position, _distanceHolder.Target.position) > INACTIVERANGE ||
                _currentFollowTime > MAXFOLLOWTIME))
            {
                target = null;
            }
            else if (_distanceHolder.Target.GetComponent<Item>() &&
                     _distanceHolder.Target.GetComponent<Item>().IsPickedUp())
            {
                target = null;
            }
            else if (_distanceHolder.Target.GetComponent<Checkpoint>() &&
                _distanceHolder.Target.GetComponent<Checkpoint>().CheckType(_basicComponent, Checkpoint.Type.FRIENDLY))
            {
                target = null;
            }
            else if ((_targetType == TargetTypes.CHECKPOINT || _targetType == TargetTypes.WAYPOINT) && _nearTarget != null)
            {
                target = null;
            }
        }

        /// <summary>
        /// Calculates direction based on path to current target
        /// </summary>
        private void CalculateDirection()
        {
            // no target, so just apply gravity
            if (target == null || TargetReached)
            {
                _basicComponent.Direction = new Vector3(0, _basicComponent.Direction.y, 0);
                return;
            }

            if (!controller.isGrounded)
            {
                return;
            }

            if (!_basicComponent.CanMove)
            {
                return;
            }

            // Preserve Y coord. Fixes falling animation bugs.
            float origY = _basicComponent.Direction.y;
            _basicComponent.Direction = CalculateVelocity(GetFeetPosition());
            _basicComponent.Direction.y = origY;

            // We need to rotate directly towards our target if we have reached it;
            // otherwise, we are walking and need to rotate along the path.
            if (targetDirection != Vector3.zero)
            {
                RotateTowards(targetDirection, true);
            }
        }

        /// <summary>
        /// Coroutine for capturing the checkpoints in sm-mode
        /// </summary>
        /// <returns>Waiting time</returns>
        private IEnumerator CaptureCheckpoint()
        {
            canMove = false;

            var c = _distanceTarget.GetComponent<Checkpoint>();
            while (!c.CheckType(_basicComponent, Checkpoint.Type.FRIENDLY))
            {
                yield return null;
            }

            canMove = true;
        }

        #region "near targets"
        private bool SearchNearTarget(float range)
        {
            _nearTarget = null;
            var others = Physics.OverlapSphere(transform.position, range);

            foreach (var other in others)
            {
                var obj = other.GetComponent<AbstractSpawnable>();

                if (!IsValidNearTarget(obj))
                {
                    continue;
                }

                _nearTarget = other.transform;
                _nearTargetType = TargetTypes.CHARACTER;

                if (obj is Item)
                {
                    _nearTargetType = TargetTypes.ITEM;
                    (obj as Item).ReservingPlayer = _basicComponent;
                }

                return true;
            }

            return false;
        }

        private IEnumerator NearTargetMapScanning()
        {
            if (_isNearTargetMapScanning)
            {
                yield break;
            }

            _isNearTargetMapScanning = true;

            while (!SearchNearTarget(MAPNEARSCANRANGE))
            {
                yield return new WaitForSeconds(MAPNEARSCANRATE);
            }

            _isNearTargetMapScanning = false;
        }

        private bool IsValidNearTarget(AbstractSpawnable obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (Vector3.Distance(transform.position, obj.transform.position) > INACTIVERANGE)
            {
                return false;
            }

            if (obj is Item && _gameManager.GameMode != GameManager.Mode.SPECIAL)
            {
                var item = obj as Item;
                if (_basicComponent.Team.TeamNo != GameManager.MOBTEAMNO && !item.IsPickedUp() &&
                    (item.ReservingPlayer == null || item.ReservingPlayer == _basicComponent))
                {
                    return true;
                }
            }
            else if (obj is Character)
            {
                if (!(obj as Character).IsDead() && (obj as Character).Team != null &&
                    !Equals(_basicComponent.Team, (obj as Character).Team))
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region "distance targets"
        private bool SearchDistanceTarget()
        {
            _distanceTarget = null;

            if (_gameManager.GameMode == GameManager.Mode.SPECIAL)
            {
                if (IsElite)
                {
                    var waypoitGameObject = Instantiate(Resources.Load("Pathfinding/Waypoint"), tr.position, Quaternion.identity) as GameObject;

                    System.Diagnostics.Debug.Assert(waypoitGameObject != null, "waypoitGameObject != null");

                    _distanceTarget = waypoitGameObject.transform;
                    _distanceTargetType = TargetTypes.WAYPOINT;
                }
                else
                {
                    SearchNextCheckpoint();
                }
            }
            else if (_basicComponent.Team.TeamNo != GameManager.MOBTEAMNO)
            {
                SearchNextWaypoint();
            }

            return _distanceTarget != null;
        }

        private IEnumerator DistanceTargetMapScanning()
        {
            if (_isDistanceTargetMapScanning)
            {
                yield break;
            }

            _isDistanceTargetMapScanning = true;

            while (!SearchDistanceTarget())
            {
                yield return new WaitForSeconds(MAPDISTANCESCANRATE);
            }

            _isDistanceTargetMapScanning = false;
        }

        private void SearchNextCheckpoint()
        {
            _distanceTarget = null;

            var checkpoints = (List<Checkpoint>)_gameManager.Checkpoints;

            for (int i = _distanceTargetIndex; i < checkpoints.Count; i++)
            {
                int index = _basicComponent.Team.TeamNo == 1 ? i : checkpoints.Count - 1 - i;

                if (!checkpoints[index].CheckType(_basicComponent, Checkpoint.Type.FRIENDLY))
                {
                    _distanceTarget = checkpoints[index].transform;
                    _distanceTargetType = TargetTypes.CHECKPOINT;
                    _distanceTargetIndex = i;
                    break;
                }
            }
        }

        private void SearchNextWaypoint()
        {
            if (_basicComponent.Team == null || _basicComponent.Team.TeamNo == GameManager.MOBTEAMNO)
            {
                return;
            }

            var w = _gameManager.CurrentSection.transform.GetComponentsInChildren<Waypoint>().FirstOrDefault(
                waypoint =>
                    /*waypoint.TeamNo == _basicComponent.Team.TeamNo &&*/
                    ((_distanceTarget == null && waypoint.Sort == 0) ||
                     (_distanceTargetIndex + 1 == waypoint.Sort)));

            if (w != null)
            {
                _distanceTarget = w.transform;
                _distanceTargetType = TargetTypes.WAYPOINT;
            }
        }

        private bool IsValidDistanceTarget(Transform distanceTarget)
        {
            var c = distanceTarget.GetComponent<Checkpoint>();

            if (c != null)
            {
                return !c.CheckType(_basicComponent, Checkpoint.Type.FRIENDLY);
            }

            return true;
        }
        #endregion
    }
}

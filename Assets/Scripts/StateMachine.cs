namespace Assets.Scripts
{
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Implements a state machine that detects (special) attacks based on KeyCodes fed to
/// it.
/// </summary>
    public class StateMachine
    {
        #region public constants
        /// <summary>
        /// Time to input a combo.
        /// </summary>
        public const float COMBOTIME = 0.5f;
        #endregion

        #region private fields
        /// <summary>
        /// Maps states to attacks.
        /// </summary>
        private Dictionary<int, AttackCallback> _attackMap;

        /// <summary>
        /// Maps key codes to DFA state chart indizes (i. e. the second
        /// dimension of the state chart).
        /// </summary>
        private Dictionary<KeyCode, int> _keyCodeMap;

        /// <summary>
        /// Time of last input.
        /// </summary>
        private float _lastTimePress = 0;

        /// <summary>
        /// Current DFA state.
        /// </summary>
        private int _currentState;

        /// <summary>
        /// The DFA's state chart.
        /// </summary>
        private List<int[]> _stateChart;

        /// <summary>
        /// The special attack detected by the DFA.
        /// </summary>
        private AttackCallback _detectedSpecialAttack;
        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachine"/> class.
        /// </summary>
        public StateMachine()
        {
            _attackMap = new Dictionary<int, AttackCallback>();
            _keyCodeMap = new Dictionary<KeyCode, int>();
            _stateChart = new List<int[]>(1);
        }
        #endregion

        #region public types
        /// <summary>
        /// The delegate used to perform a special attack.
        /// </summary>
        public delegate void AttackCallback();
        #endregion

        #region public properties
        /// <summary>
        /// Gets the special attack detected by the DFA.
        /// </summary>
        public AttackCallback DetectedSpecialAttack
        {
            get
            {
                return _detectedSpecialAttack;
            }

            private set
            {
                _detectedSpecialAttack = value;
            }
        }
        #endregion

        #region public methods
        /// <summary>
        /// Initialize the state chart after all key codes have been added.
        /// </summary>
        public void InitStateChart()
        {
            // Initialize the starting state.
            AddState();
        }

        /// <summary>
        /// Add a new key code.
        /// </summary>
        /// <param name="keyCode">Key code to add.</param>
        public void AddKeyCode(KeyCode keyCode)
        {
            _keyCodeMap[keyCode] = _keyCodeMap.Count;
        }

        /// <summary>
        /// Add a special attack with multiple key combos to the DFA.
        /// </summary>
        /// <remarks>
        /// Any of the key combos trigger the attack.
        /// </remarks>
        /// <param name="keyCombos">Array of key combos that trigger this attack.</param>
        /// <param name="attackCallback">Callback to be returned when the attack is triggered.</param>
        public void AddSpecialAttack(KeyCode[][] keyCombos, AttackCallback attackCallback)
        {
            foreach (KeyCode[] keyCombo in keyCombos)
            {
                AddSpecialAttack(keyCombo, attackCallback);
            }
        }

        /// <summary>
        /// Add a new special attack with a single key combo to the DFA.
        /// </summary>
        /// <remarks>
        /// Only the specified key combo will trigger the attack (unless you add more key combos).
        /// </remarks>
        /// <param name="keyCombo">Key combo that triggers this attack.</param>
        /// <param name="attackCallback">Callback to be returned when the attack is triggered.</param>
        public void AddSpecialAttack(KeyCode[] keyCombo, AttackCallback attackCallback)
        {
            int state = 0;
            foreach (KeyCode key in keyCombo)
            {
                // TODO: Handle keys not added by AddKeyCode().
                int idx = _keyCodeMap[key];

                if (_stateChart[state][idx] == 0)
                {   // We need to add a new state.
                    int newState = AddState();

                    // Add the transition.
                    _stateChart[state][idx] = newState;
                }

                // Follow the transition.
                state = _stateChart[state][idx];
            }

            // Good, we should now be in the final state.
            // TODO: Handle key combo collisions.
            _attackMap[state] = attackCallback;
        }

        /// <summary>
        /// Feed the currently pressed key to the DFA.
        /// </summary>
        /// <remarks>
        /// If a special attack is detected, <see cref="DetectedSpecialAttack"/> is set
        /// to a non-NULL value. You can then call that callback to trigger the attack.
        /// </remarks>
        public void Check()
        {
            // No special attack detected yet, so make sure the last
            // detected action is cleared.
            _detectedSpecialAttack = null;

            // Check for the pressed key.
            KeyCode pressedKey = KeyCode.None;
            foreach (KeyCode key in _keyCodeMap.Keys)
            {
                if (ControlKeysManager.GetKeyDown(key))
                {
                    pressedKey = key;
                    break;
                }
            }

            if (pressedKey == KeyCode.None)
            {   // No (known) key pressed.
                return;
            }

            if (_lastTimePress > 0 && Time.time - _lastTimePress > COMBOTIME)
            {   // Timeout. Combo rejected.
                _currentState = 0;
            }

            _lastTimePress = Time.time;

            // This key maps to which index?
            int keyIndex = _keyCodeMap[pressedKey];

            // Determine the next state.
            _currentState = _stateChart[_currentState][keyIndex];

            // And, is it a special attack?
            if (!_attackMap.TryGetValue(_currentState, out _detectedSpecialAttack))
            {   // Nope. It's a normal state.
                return;
            }

            // Yes, it is a special attack! We are done.
            foreach (int nextState in _stateChart[_currentState])
            {
                if (nextState != 0)
                {   // We cannot return to state 0 as there may be another special attack
                    // after this one.
                    // DO NOT RESET THE STATE HERE. State needs to be kept so that 
                    // e. g. the triple attack works.
                    return;
                }
            }

            // There is no special attack after this one; return to state 0.
            _currentState = 0;
        }

        /// <summary>
        /// Was a special attack detected? If true, it is stored in DetectedSpecialAttack.
        /// </summary>
        /// <returns>Special attack detected?</returns>
        public bool SpecialAttackDetected()
        {
            return _detectedSpecialAttack != null;
        }
        #endregion

        #region protected methods
        /// <summary>
        /// Add a new state to the state chart.
        /// </summary>
        /// <returns>Index of new state.</returns>
        protected int AddState()
        {
            _stateChart.Add(new int[_keyCodeMap.Count]);
            return _stateChart.Count - 1;
        }
        #endregion
    }
}

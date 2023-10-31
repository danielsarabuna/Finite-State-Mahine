using System;
using System.Collections.Generic;
using System.Linq;

namespace FiniteStateMaсhine
{
    public class StateMachine
    {
        public event Action<string, byte> OnLog;
        private readonly Butler _butler;
        private Dictionary<uint, Prop> _props;
        private Dictionary<byte, IState> _states;
        private List<ITransition> _allTransitions;
        private ITransition[] _transitions;
        private IState _currentState;
        private IUpdatable _updatable;

        public StateMachine(Butler butler, Dictionary<uint, Prop> props, IState startState)
        {
            _butler = butler;
            _props = props;
            _currentState = startState;
            _states = new Dictionary<byte, IState>();
            _allTransitions = new List<ITransition>();
            _transitions = Array.Empty<ITransition>();
        }

        public void Start()
        {
            if (!_states.ContainsKey(_currentState.ID))
                AddState(_currentState);
            TryMakeEnter();
            TryMakeUpdatable();
        }

        public void Stop()
        {
        }

        #region Prop Block

        public void AddProp(Prop prop) => _props.Add(prop.ID, prop);

        public bool TryGetProp(uint id, out Prop prop) => _props.TryGetValue(id, out prop);

        public bool TryGetProp(uint id, Type type, out Prop prop)
        {
            var tryGetValue = TryGetProp(id, out prop);
            return tryGetValue && prop.Type == type;
        }

        public void ChangePropValue(uint id, object value)
        {
            if (TryGetProp(id, out Prop prop))
            {
                prop.ApplyValue(value);
                _props[id] = prop;
            }
        }

        #endregion

        #region State Block

        public void AddState<T>(T state) where T : IState
        {
            _states.Add(state.ID, state);
        }

        public void SetState(byte id)
        {
            var state = _states[id];
            SetState(state);
        }

        public void SetState<T>(T state) where T : IState
        {
            MakeTransition(state);
            if (!_states.ContainsKey(state.ID))
                AddState(state);
        }

        public bool TryGetState(byte id, out IState state) => _states.TryGetValue(id, out state);

        public bool TryGetState<T>(string id, out T state) where T : IState
        {
            state = default;
            if (TryGetState(id, out IState tryState))
                if (tryState is T result)
                {
                    state = result;
                    return true;
                }

            return false;
        }

        #endregion

        #region Transition Block

        public void AddTransition<T>(T transition) where T : ITransition
        {
            _allTransitions.Add(transition);
        }

        public void RemoveTransition<T>(T transition) where T : ITransition
        {
            _allTransitions.Remove(transition);
        }

        #endregion

        public void Update()
        {
            foreach (var transition in _transitions)
            {
                if (transition.IsAllowed(this, _butler))
                {
                    if (_states.TryGetValue(transition.To, out IState state))
                    {
                        if (transition.To == state.ID)
                        {
                            MakeTransition(state);
                            return;
                        }
                    }
                }
            }

            if (_updatable != null)
            {
                _updatable.Update(this, _butler);
                OnLog?.Invoke("Update ", _currentState.ID);
            }
        }

        private void MakeTransition<T>(T state) where T : IState
        {
            if (_currentState is IExit exit)
            {
                exit.Exit(this, _butler);
                OnLog?.Invoke("Exit from ", _currentState.ID);
            }

            TryMakeExit();
            _currentState = state;
            TryMakeEnter();
            TryMakeUpdatable();
        }

        private void TryMakeExit()
        {
            if (_currentState is IExit exit)
            {
                exit.Exit(this, _butler);
                OnLog?.Invoke("Exit from ", _currentState.ID);
            }
        }

        private void TryMakeEnter()
        {
            if (_currentState is IEnter enter)
            {
                enter.Enter(this, _butler);
                OnLog?.Invoke("Enter to ", _currentState.ID);
            }
        }

        private void TryMakeUpdatable()
        {
            _updatable = _currentState as IUpdatable;
            _transitions = _allTransitions
                .Where(x => !x.From.HasValue || x.From.Value == _currentState.ID)
                .ToArray();
        }
    }
}
using System.Collections.Generic;

namespace JrpgGame.Combat
{
    public class CombatStateMachine
    {
        private readonly Dictionary<CombatStateType, CombatState> _states = new Dictionary<CombatStateType, CombatState>();

        public CombatState? CurrentState { get; private set; }

        public void RegisterState(CombatState state)
        {
            state.Machine = this;
            _states[state.Type] = state;
        }

        public void ChangeState(CombatStateType newType)
        {
            if (_states.TryGetValue(newType, out var newState))
            {
                CurrentState?.Exit();
                CurrentState = newState;
                CurrentState.Enter();
            }
        }

        public void Update()
        {
            CurrentState?.Update();
        }
    }
}

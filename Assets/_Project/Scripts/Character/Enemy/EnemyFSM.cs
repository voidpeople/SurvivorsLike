using UnityEngine;

namespace SurvivorsLike
{
    public enum EnemyStateType
    {
        Idle,
        Chase,
        Attack,
        Dead
    }


    public class EnemyFSM
    {
        private readonly EnemyStateBase[] _states;
        private EnemyStateBase _currentState;

        private EnemyStateType _currentStateType;

        public EnemyFSM()
        {
            _states = new EnemyStateBase[System.Enum.GetValues(typeof(EnemyStateType)).Length];
        }

        public void RegisterState(EnemyStateType type, EnemyStateBase state)
        {
            _states[(int)type] = state;
        }

        public void Init(EnemyStateType type)
        {
            _currentState?.Exit();
            _currentStateType = type;
            _currentState = _states[(int)_currentStateType];
            _currentState.Enter();
        }

        public void ChangeState(EnemyStateType type)
        {
            if (_currentStateType == type)
                return;

            _currentState?.Exit();
            _currentStateType = type;
            _currentState = _states[(int)_currentStateType];
            _currentState.Enter();
        }

        public void Update()
        {
            if(_currentState != null)
                _currentState.Update();
        }
    }
}

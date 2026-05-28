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

        public EnemyStateType CurrentStateType { get; private set; }

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
            CurrentStateType = type;
            _currentState = _states[(int)CurrentStateType];
            _currentState.Enter();
        }

        public void ChangeState(EnemyStateType type)
        {
            if (CurrentStateType == type)
                return;

            _currentState?.Exit();
            CurrentStateType = type;
            _currentState = _states[(int)CurrentStateType];
            _currentState.Enter();
        }

        public void Update()
        {
            if (_currentState != null)
                _currentState.Update();
        }

        public void OnDestinationReached()
        {
            (_currentState as IMovementListener)?.OnDestinationReached();
        }

        public void OnTargetDied()
        {

        }
    }
}

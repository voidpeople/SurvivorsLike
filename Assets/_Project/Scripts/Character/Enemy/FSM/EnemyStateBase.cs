using UnityEngine;


namespace SurvivorsLike
{
    public abstract class EnemyStateBase
    {
        protected EnemyController _controller { get; }
        protected EnemyFSM        _fsm { get; }

        public EnemyStateBase(EnemyController controller, EnemyFSM fsm)
        {
            _controller = controller;
            _fsm = fsm;
        }

        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void Update() { }
    }
}

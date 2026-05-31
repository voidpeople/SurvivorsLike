using UnityEngine;


namespace SurvivorsLike
{
    public abstract class EnemyStateBase : StateBase
    {
        protected readonly EnemyController _controller;
        protected readonly EnemyFSM _fsm;

        public EnemyStateBase(EnemyController controller, EnemyFSM fsm)
        {
            _controller = controller;
            _fsm = fsm;
        }
    }
}

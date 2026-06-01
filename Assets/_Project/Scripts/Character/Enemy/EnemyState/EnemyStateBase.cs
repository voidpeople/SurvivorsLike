using UnityEngine;


namespace SurvivorsLike
{
    public abstract class EnemyStateBase : StateBase
    {
        protected readonly EnemyController _ctrl;
        protected readonly EnemyFSM _fsm;

        public EnemyStateBase(EnemyController controller, EnemyFSM fsm)
        {
            _ctrl = controller;
            _fsm = fsm;
        }
    }
}

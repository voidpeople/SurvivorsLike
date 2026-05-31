using SurvivorsLike;
using UnityEngine;


namespace SurvivorsLike
{
    public abstract class BossStateBase : StateBase
    {
        protected readonly EnemyController _controller;
        protected readonly BossFSM _fsm;

        public BossStateBase(EnemyController controller, BossFSM fsm)
        {
            _controller = controller;
            _fsm = fsm;
        }
    }
}

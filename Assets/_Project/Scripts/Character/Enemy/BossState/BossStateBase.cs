using SurvivorsLike;
using UnityEngine;


namespace SurvivorsLike
{
    public abstract class BossStateBase : StateBase
    {
        protected readonly BossController _controller;
        protected readonly BossFSM _fsm;

        public BossStateBase(BossController controller, BossFSM fsm)
        {
            _controller = controller;
            _fsm = fsm;
        }
    }
}

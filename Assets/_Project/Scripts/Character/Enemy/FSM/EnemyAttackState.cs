using UnityEngine;


namespace SurvivorsLike
{
    public class EnemyAttackState : EnemyStateBase
    {
        public EnemyAttackState(EnemyController controller, EnemyFSM fsm)
            : base(controller, fsm)
        {

        }

        public override void Enter()
        {
            _controller.AnimCtrl.PlayIdle();
        }

        public override void Exit()
        {

        }

        public override void Update()
        {
        }
    }
}

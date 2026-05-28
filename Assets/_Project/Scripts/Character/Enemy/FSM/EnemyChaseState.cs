using UnityEngine;


namespace SurvivorsLike
{
    public class EnemyChaseState : EnemyStateBase, IMovementListener, ITargetListener
    {
        public EnemyChaseState(EnemyController controller, EnemyFSM fsm)
            : base(controller, fsm)
        {

        }

        public override void Enter()
        {
            //이 함수가 호출 되었다는 것은 EnemyController에 타겟이 설정 되었다는 의미~
            _controller.Movement.SetTarget(_controller.TargetTransform);
            _controller.AnimCtrl.PlayMove();
        }

        public override void Exit()
        {
        }

        public override void Update()
        {
        }

        //EnemyMovement가 목표 위치에 도착하면 발송되는 이벤트를 받는 함수
        public void OnDestinationReached()
        {
            _controller.AnimCtrl.PlayIdle();
            _fsm.ChangeState(EnemyStateType.Attack);
        }

        public void OnTargetDied()
        {

        }
    }
}

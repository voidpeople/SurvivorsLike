using UnityEngine;


namespace SurvivorsLike
{
    public class EnemyIdleState : EnemyStateBase
    {
        public EnemyIdleState(EnemyController controller, EnemyFSM fsm)
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
            //뱀서라이크 게임의 경우 적 캐릭터는 스폰하면 플레이어 캐릭터를 탐지할 필요 없이
            //초기화 때 주어진 플레이어 캐릭터의 Transform을 참조하여 이동한다.
            if(_controller.TargetTransform != null)
            {
                _fsm.ChangeState(EnemyStateType.Chase);
            }
        }
    }
}

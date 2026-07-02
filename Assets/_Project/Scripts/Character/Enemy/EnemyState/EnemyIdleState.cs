using System.Security.Cryptography;
using UnityEngine;


namespace SurvivorsLike
{
    public class EnemyIdleState : EnemyStateBase, ITargetListener
    {
        public EnemyIdleState(EnemyController controller, EnemyFSM fsm)
            : base(controller, fsm)
        {
        }

        public override void Enter()
        {
            _ctrl.Movement.Stop();
            _ctrl.AnimCtrl.PlayIdle();
        }

        public override void Update()
        {
            //뱀서라이크 게임의 경우 적 캐릭터는 스폰하면 플레이어 캐릭터를 탐지할 필요 없이
            //초기화 때 주어진 플레이어 캐릭터의 Transform을 참조하여 이동한다.
            if(_ctrl.TargetTransform != null)
            {
                _fsm.ChangeState(EnemyStateType.Chase);
            }
        }

        public void OnTargetDied()
        {

        }
    }
}

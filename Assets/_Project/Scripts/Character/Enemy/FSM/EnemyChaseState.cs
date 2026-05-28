using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;


namespace SurvivorsLike
{
    public class EnemyChaseState : EnemyStateBase, ITargetListener
    {
        //공격 거리
        private float _attackRangeSqr;
        private int _taskVersion;

        public EnemyChaseState(EnemyController controller, EnemyFSM fsm)
            : base(controller, fsm)
        {
            _taskVersion = 0;
        }

        public override void Enter()
        {
            _attackRangeSqr = _controller.AttackRange * _controller.AttackRange;
            _controller.AnimCtrl.PlayMove();

            int version = ++_taskVersion;
            UpdateDestinationAsync(_controller.CTS, version).Forget();
        }

        public override void Exit()
        {
            //현재 실행중인 태스크 지연 종료~
            _taskVersion++;
        }

        public override void Update()
        {
            if (_controller.TargetTransform == null)
            {
                _fsm.ChangeState(EnemyStateType.Idle);
                return;
            }

            Vector3 dirVec = _controller.TargetTransform.position - _controller.transform.position;
            dirVec.y = 0f;
            float distSqr = dirVec.sqrMagnitude;
            //타겟이 공격 거리에 들어 왔다면 Attack상태로 변경 요청
            if(distSqr <= _attackRangeSqr)
            {
                _fsm.ChangeState(EnemyStateType.Attack);
            }
        }

        //0.2초 마다 movement에게 새로운 목표 위치를 전송 해줌~
        private async UniTaskVoid UpdateDestinationAsync(CancellationToken ct, int taskVersion)
        {
            while((_taskVersion == taskVersion) && (ct.IsCancellationRequested == false))
            {
                if(_controller.TargetTransform != null)
                {
                    _controller.Movement.SetDestination(_controller.TargetTransform.position);

                    //0.2초 대기
                    await UniTask.Delay(200, cancellationToken: ct);
                }
            }
        }

        public void OnTargetDied()
        {
            _fsm.ChangeState(EnemyStateType.Idle);
        }
    }
}

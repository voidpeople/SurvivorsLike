using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;


namespace SurvivorsLike
{
    //적을 추적하는 상태
    public class EnemyChaseState : EnemyStateBase, ITargetListener, IMovementListener
    {
        //공격 거리
        private float _attackRangeSqr;
        //태스크의 중복 실행와 지연 종료을 위한 변수
        private int   _taskVersion;

        public EnemyChaseState(EnemyController controller, EnemyFSM fsm)
            : base(controller, fsm)
        {
            _taskVersion = 0;
        }

        public override void Enter()
        {
            _attackRangeSqr = _ctrl.AttackRange * _ctrl.AttackRange;
            _ctrl.AnimCtrl.PlayMove();

            int taskVersion = ++_taskVersion;
            UpdateAsync(taskVersion, _ctrl.CTS).Forget();
        }

        public override void Exit()
        {
            //현재 실행중인 태스크 지연 종료~
            _taskVersion++;
        }

        public override void Update()
        {
            if (_ctrl.TargetTransform == null)
            {
                _fsm.ChangeState(EnemyStateType.Idle);
                return;
            }
        }

        //0.2초 마다 movement에게 새로운 목표 위치를 전송 해줌~
        private async UniTaskVoid UpdateAsync(int taskVersion, CancellationToken ct)
        {
            while((_taskVersion == taskVersion) && (ct.IsCancellationRequested == false))
            {
                //Update에서도 _controller.TargetTransform의 null 체크를 하지만 UpdateAsync 보다
                //Update에서 먼저 _controller.TargetTransform의 null 체크를 먼저 한다는 보장은 안된다.
                //따라서 이곳에도 null 체크를 추가한다.
                if (_ctrl.TargetTransform == null)
                {
                    _fsm.ChangeState(EnemyStateType.Idle);
                    //태스크에서도 return 명령은 해당 태스크를 즉각 정상 종료 시킨다.
                    return;
                }

                Vector3 dirVec = _ctrl.TargetTransform.position - _ctrl.transform.position;
                dirVec.y = 0f;
                float distSqr = dirVec.sqrMagnitude;
                //타겟이 공격 거리에 들어 왔다면 Attack상태로 변경 요청
                if (distSqr <= _attackRangeSqr)
                {
                    _fsm.ChangeState(EnemyStateType.Attack);
                    return;
                }

                _ctrl.Movement.SetDestination(_ctrl.TargetTransform.position);
                //0.2초 대기
                await UniTask.Delay(200, cancellationToken: ct);
            }
        }

        //타겟의 위치까지 도착 했다면 공격 가능한 거리인지 검사하고 공격 상태로 전환 요청
        //UpdateAsync의 로직 보다 0.2초 빠르게 반응~
        public void OnDestinationReached()
        {
            if (_ctrl.TargetTransform == null)
            {
                _fsm.ChangeState(EnemyStateType.Idle);
                return;
            }

            Vector3 dirVec = _ctrl.TargetTransform.position - _ctrl.transform.position;
            dirVec.y = 0f;
            float distSqr = dirVec.sqrMagnitude;
            //타겟이 공격 거리에 들어 왔다면 Attack상태로 변경 요청
            if (distSqr <= _attackRangeSqr)
            {
                _fsm.ChangeState(EnemyStateType.Attack);
                return;
            }

            _ctrl.Movement.SetDestination(_ctrl.TargetTransform.position);
        }

        public void OnTargetDied()
        {
            _fsm.ChangeState(EnemyStateType.Idle);
        }
    }
}

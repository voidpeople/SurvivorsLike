using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;


namespace SurvivorsLike
{
    public class EnemyAttackState : EnemyStateBase, ITargetListener
    {
        //공격 거리
        private float _attackRangeSqr;
        //태스크의 중복 실행와 자연 스러운 종료를 위한 변수
        private int _taskVersion;

        public EnemyAttackState(EnemyController controller, EnemyFSM fsm)
            : base(controller, fsm)
        {
            _taskVersion = 0;
        }

        public override void Enter()
        {
            _attackRangeSqr = _ctrl.AttackRange * _ctrl.AttackRange;

            _ctrl.Movement.Stop();
            _ctrl.AnimCtrl.PlayAttack(true);

            //스킬 사용 시작
            _ctrl.SkillCtrl.UseAllSkill();

            int taskVersion = ++_taskVersion;
            UpdateAsync(taskVersion, _ctrl.CTS).Forget();
        }

        public override void Exit()
        {
            _ctrl.AnimCtrl.PlayAttack(false);
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
            while ((_taskVersion == taskVersion) && (ct.IsCancellationRequested == false))
            {
                //Update에서도 _controller.TargetTransform의 null 체크를 하지만 UpdateAsync 보다
                //Update에서 먼저 _controller.TargetTransform의 null 체크를 먼저 한다는 보장은 안된다.
                //따라서 이곳에도 null 체크를 추가한다.
                if (_ctrl.TargetTransform == null)
                {
                    _fsm.ChangeState(EnemyStateType.Idle);
                    return;
                }

                Vector3 dirVec = _ctrl.TargetTransform.position - _ctrl.transform.position;
                dirVec.y = 0f;
                float distSqr = dirVec.sqrMagnitude;
                //타겟이 공격 거리로 부터 멀어지면 Chase 상태로 전환 요청
                if (distSqr > _attackRangeSqr)
                {
                    _fsm.ChangeState(EnemyStateType.Chase);
                    return;
                }

                //0.2초 마다 실행
                await UniTask.Delay(200, cancellationToken: ct);
            }
        }

        public void OnTargetDied()
        {
            _fsm.ChangeState(EnemyStateType.Idle);
        }
    }
}

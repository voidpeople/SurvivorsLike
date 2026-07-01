using UnityEngine;


namespace SurvivorsLike
{
    public class EnemyChaseState : EnemyStateBase, ITargetListener, IMovementListener
    {
        private float _attackRangeSqr;

        public EnemyChaseState(EnemyController controller, EnemyFSM fsm)
            : base(controller, fsm) { }

        public override void Enter()
        {
            _attackRangeSqr = _ctrl.AttackRange * _ctrl.AttackRange;
            _ctrl.AnimCtrl.PlayMove();
        }

        public override void Exit() { }

        public override void Update()
        {
            if (_ctrl.TargetTransform == null)
            {
                _fsm.ChangeState(EnemyStateType.Idle);
                return;
            }

            Vector3 dirVec = _ctrl.TargetTransform.position - _ctrl.transform.position;
            dirVec.y = 0f;
            float distSqr = dirVec.sqrMagnitude;

            if (distSqr <= _attackRangeSqr)
            {
                _fsm.ChangeState(EnemyStateType.Attack);
                return;
            }

            _ctrl.Movement.SetDestination(_ctrl.TargetTransform.position);
        }

        public void OnDestinationReached()
        {
            if (_ctrl.TargetTransform == null)
                _fsm.ChangeState(EnemyStateType.Idle);
        }

        public void OnTargetDied()
        {
            _fsm.ChangeState(EnemyStateType.Idle);
        }
    }
}

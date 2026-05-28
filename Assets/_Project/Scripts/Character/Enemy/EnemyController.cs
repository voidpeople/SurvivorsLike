using UnityEngine;
using UnityEngine.InputSystem;


namespace SurvivorsLike
{
    public class EnemyController : MonoBehaviour
    {
        public EnemyAnimationController AnimCtrl { get; private set; }
        public EnemyMovement Movement { get; private set; }
        public EnemySkill Skill { get; private set; }
        public Transform TargetTransform { get; private set; }

        private EnemyHealth _health;
        private EnemyFSM _fsm;

        private void Awake()
        {
            AnimCtrl = GetComponentInChildren<EnemyAnimationController>();

            TryGetComponent(out EnemyMovement movement);
            Movement = movement;
            TryGetComponent(out EnemySkill skill);
            Skill = skill;
            TryGetComponent(out EnemyHealth _health);

            CreateFSM();
        }

        private void CreateFSM()
        {
            _fsm = new EnemyFSM();
            _fsm.RegisterState(EnemyStateType.Idle, new EnemyIdleState(this, _fsm));
            _fsm.RegisterState(EnemyStateType.Chase, new EnemyChaseState(this, _fsm));
            _fsm.RegisterState(EnemyStateType.Attack, new EnemyAttackState(this, _fsm));
            _fsm.RegisterState(EnemyStateType.Dead, new EnemyDeadState(this, _fsm));
        }

        public void Init(Transform targetTrasnform)
        {
            TargetTransform = targetTrasnform;
            //Movement.SetTarget(TargetTransform);
            _fsm.Init(EnemyStateType.Idle);
        }

        private void Update()
        {
            if (_fsm != null)
                _fsm.Update();
        }
    }
}

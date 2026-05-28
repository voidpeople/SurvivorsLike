using UnityEngine;
using UnityEngine.InputSystem;


namespace SurvivorsLike
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private EnemyStateType _currentStateType;

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
            Movement.OnDestinationReached += OnDestinationReached;
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
            _fsm.Init(EnemyStateType.Idle);

            TryGetComponent(out PlayerHealth health);
            health.OnDead += OnTargetDied;
        }

        //Enemy 캐릭터가 목표 위치에 도착하면 통보 받는 함수
        private void OnDestinationReached()
        {
            Debug.Log($"OnDestinationReached 함수 호출~");
            _fsm.OnDestinationReached();
        }

        //타겟이 죽으면 통보 받는 함수
        private void OnTargetDied()
        {
            _fsm.OnTargetDied();
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (_fsm != null)
                _currentStateType = _fsm.CurrentStateType;
#endif
            if (_fsm != null)
                _fsm.Update();
        }
    }
}

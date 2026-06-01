using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;


namespace SurvivorsLike
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private EnemyStateType _currentStateType;

        [Header("공격 거리")]
        [SerializeField] private float _attackRange = 1f;

        public EnemyAnimationController AnimCtrl { get; private set; }
        public EnemyMovement Movement { get; private set; }
        public SkillController SkillCtrl { get; private set; }
        public Transform TargetTransform { get; private set; }
        public float AttackRange { get { return _attackRange; } }

        private EnemyHealth _health;
        private EnemyFSM _fsm;

        private CancellationTokenSource _cts;
        public CancellationToken CTS { get { return _cts.Token; } }

        private void Awake()
        {
            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            AnimCtrl = GetComponentInChildren<EnemyAnimationController>();

            TryGetComponent(out EnemyMovement movement);
            Movement = movement;
            Movement.OnDestinationReached += OnDestinationReached;
            TryGetComponent(out SkillController skillCtrl);
            SkillCtrl = skillCtrl;
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


        //타겟은 플레이어 캐릭터 하나 이므로
        //적 캐릭터가 스폰되자 마자 Init 함수를 통해 타겟이 설정됨~
        public void Init(Transform targetTrasnform)
        {
            TargetTransform = targetTrasnform;
            _fsm.Init(EnemyStateType.Idle);

            TargetTransform.TryGetComponent(out PlayerHealth health);
            health.OnDead += OnTargetDied;
        }

        //Enemy 캐릭터가 목표 위치에 도착하면 통보 받는 함수
        private void OnDestinationReached()
        {
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
                _currentStateType = _fsm.CurrentType;
#endif
            if (_fsm != null)
                _fsm.Update();
        }

        private void OnDestroy()
        {
            _cts?.Cancel();   //진행 중인 비동기 작업에 취소 신호 전달
            _cts?.Dispose();  //내부 WaitHandle 등 비관리 리소스 해제
            _cts = null;
        }
    }
}

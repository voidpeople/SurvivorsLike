using R3;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;


namespace SurvivorsLike
{
    public class EnemyController : MonoBehaviour, IPoolable
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

        private bool _isPlaying;

        private readonly CompositeDisposable _disposables = new();


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

            _isPlaying = false;
        }
        private void Start()
        {
            InGameEventBus.OnInGameStart
                .Take(1)
                .Subscribe(_ => OnGameStart())
                .AddTo(_disposables);
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

        void OnGameStart()
        {
            _isPlaying = true;
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
            if (_isPlaying == false)
                return;

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

        #region IPoolable
        public void OnSpawn()
        {
            // 재사용 시 CTS 갱신 — Awake()는 최초 1회만 실행되므로 여기서 처리
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            _isPlaying = false;
        }

        public void OnDespawn()
        {
            // 진행 중인 비동기 작업 취소
            _cts?.Cancel();

            // R3 구독 전체 해제
            _disposables.Clear();

            // Init()에서 등록한 PlayerHealth 이벤트 해제 — 중복 누적 방지
            if (TargetTransform != null)
            {
                if (TargetTransform.TryGetComponent(out PlayerHealth health))
                    health.OnDead -= OnTargetDied;
                TargetTransform = null;
            }
        }

        public void ReturnToPool()
        {
            PoolManager.Instance.Return(this);
        }
        #endregion
    }
}

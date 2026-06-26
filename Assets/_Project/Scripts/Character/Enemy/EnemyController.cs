using R3;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;


namespace SurvivorsLike
{
    public class EnemyController : MonoBehaviour, IPoolable, IAlive, ITickable, ISkillOwner, ITargetListener, ITargetable 
    {
#if UNITY_EDITOR
        [Header("개발 테스트용")]
        [SerializeField] private EnemyStateType _currentStateType;
#endif

        [Header("씬 참조")]
        [SerializeField] private EnemyAnimationController _animCtrl;
        [SerializeField] private Transform _aimPoint;          // 피탄 기준점 — null이면 position + Y 0.5f 사용

        [Header("설정값")]
        [SerializeField] private float _attackRange = 1f;     // 공격 판정 반지름 (단위: m)


        private Health _health;
        private readonly SkillController _skillController = new();
        private EnemyFSM _fsm;        
        private Transform _firePoint;
        private EnemyManager _enemyMgr;

        public EnemyData EnemyData { get; private set; }
        public EnemyAnimationController AnimCtrl => _animCtrl;
        public EnemyMovement Movement { get; private set; }

        public Transform TargetTransform { get; private set; }
        public bool IsDead => _health.IsDead;                  // IAlive

        public float AttackRange => _attackRange;

        public Transform Transform => transform;               // ITargetable
        public Vector3 AimPoint => _aimPoint != null ? _aimPoint.position : transform.position + Vector3.up * 0.5f;  // ITargetable (조준 타겟)
        public Transform FirePoint => _firePoint;              // ISkillOwner

        // 비동기 토큰
        private CancellationTokenSource _cts;
        public CancellationToken CTS => _cts.Token;

        private void Awake()
        {
            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            TryGetComponent(out EnemyMovement movement);
            Movement = movement;
            Debug.Assert(Movement != null, $"{nameof(EnemyController)}::Awake=> EnemyMovement not found");
            Movement.OnDestinationReached += OnDestinationReached;

            TryGetComponent(out Health health);
            _health = health;
            Debug.Assert(_health != null, $"{nameof(EnemyController)}::Awake=> Health not found");
            _health.Died += OnDied;

            Debug.Assert(_animCtrl != null, $"{nameof(EnemyController)}::Awake=> _animCtrl is null");

            CreateFSM();
        }

        private void OnDestroy()
        {
            //if (EnemyManager.HasInstance)
            //    EnemyManager.Instance.UnRegister(this);

            _health.Died -= OnDied;
            Movement.OnDestinationReached -= OnDestinationReached;
            ClearTarget();

            _cts?.Cancel();   //진행 중인 비동기 작업에 취소 신호 전달
            _cts?.Dispose();  //내부 WaitHandle 등 비관리 리소스 해제
            _cts = null;
        }


        public void Tick(float deltaTime)
        {
#if UNITY_EDITOR
            if (_fsm != null)
                _currentStateType = _fsm.CurrentType;
#endif
            if (_fsm != null)
                _fsm.Tick(deltaTime);

            if (_skillController != null)
                _skillController.Tick(deltaTime);
        }

        //타겟은 플레이어 캐릭터 하나 이므로
        //적 캐릭터가 스폰되자 마자 Init 함수를 통해 타겟이 설정됨~
        public void Init(EnemyData data, Vector3 spawnPos, Transform targetTrans, EnemyManager enemyMgr)
        {
            Debug.Assert(data != null,       $"{nameof(EnemyController)}::Init=> data is null");
            Debug.Assert(targetTrans != null, $"{nameof(EnemyController)}::Init=> targetTrans is null");

            EnemyData = data;
            Movement.Init(data);
            _health.Init(data.Hp);

            transform.position = spawnPos;

            TargetTransform = targetTrans;
            _fsm.Init(EnemyStateType.Idle);

            if (!TargetTransform.TryGetComponent(out Health targetHealth))
            {
                Debug.LogError($"{nameof(EnemyController)}::Init=> Health not found on TargetTransform");
                return;
            }
            targetHealth.Died += OnTargetDied;

            _enemyMgr = enemyMgr;
            _enemyMgr?.Register(this);
        }

        //타겟이 죽으면 통보 받는 함수
        public void OnTargetDied()
        {
            _fsm.OnTargetDied();
            ClearTarget();
        }

        public void OnSpawn()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            _animCtrl.Spawn();
            //_enemyMgr.Register(this);
        }

        public void OnDespawn()
        {
            _enemyMgr?.UnRegister(this);

            ClearTarget();            
            _fsm.Despawn();           
            Movement.Despawn();       
            _cts?.Cancel();
        }

        public void ReturnToPool()
        {
            PoolManager.Instance.Return(this);
        }


        private void CreateFSM()
        {
            _fsm = new EnemyFSM();
            _fsm.RegisterState(EnemyStateType.Idle, new EnemyIdleState(this, _fsm));
            _fsm.RegisterState(EnemyStateType.Chase, new EnemyChaseState(this, _fsm));
            _fsm.RegisterState(EnemyStateType.Attack, new EnemyAttackState(this, _fsm));
            _fsm.RegisterState(EnemyStateType.Dead, new EnemyDeadState(this, _fsm));
        }

        //Enemy 캐릭터가 목표 위치에 도착하면 통보 받는 함수
        private void OnDestinationReached()
        {
            _fsm.OnDestinationReached();
        }

        private void OnDied()
        {
            _enemyMgr.NotifyKilled(
                new EnemyKilledEvent(
                    EnemyData.Type,
                    transform.position,
                    EnemyData.ExpReward));

            _fsm.ChangeState(EnemyStateType.Dead);
        }

        private void ClearTarget()
        {
            if (TargetTransform == null)
                return;

            if (TargetTransform.TryGetComponent(out Health targetHealth))
                targetHealth.Died -= OnTargetDied;

            TargetTransform = null;
        }
    }
}

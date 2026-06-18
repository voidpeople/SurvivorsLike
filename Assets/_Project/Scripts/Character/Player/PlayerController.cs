using UnityEngine;
using R3;



/*
Player(루트)
    ├─ PlayerController.cs     ← 이동, 입력, 상태
    ├─ PlayerStats.cs          ← HP, 스탯
    ├─ CharacterController     ← 물리 이동
    │
    └─ RobotKyle (자식)        ← 순수 비주얼만 담당
         ├─ Animator
         └─ SkinnedMeshRenderer
*/

namespace SurvivorsLike
{
    public class PlayerController : MonoBehaviour, IAlive, ISkillOwner, ITargetListener, ITargetable
    {
        // ─── Inspector (직렬화 필드) ──────────────────────────────────────────
        [Header("씬 참조")]
        [SerializeField] private Transform _modelRoot;     // 모델 프리팹이 붙는 루트 트랜스폼
        [SerializeField] private Transform _aimPoint;      // 피탄 기준점 — null이면 position + Y 0.5f 사용        

        // ─── Components / Systems (캐싱 참조) ─────────────────────────────────
        private PlayerMovement _movement;
        private TargetFinder _targetFinder;
        private Health _health;
        private SkillController _skillController = new();
        private PlayerAnimationController _animationController;
        private JoystickBase _joystick;

        // ─── Runtime State (런타임 상태) ──────────────────────────────────────
        private PlayerData _playerData;
        private bool _isPlaying;
        private ITargetable _target;
        private Health _targetHealth;
        private Transform _firePoint;                      // 총구 머즐 포인트 트랜스폼

        // ─── Disposables ──────────────────────────────────────────────────────
        private readonly CompositeDisposable _disposables = new();


        // ─── Properties (인터페이스 구현 포함) ────────────────────────────────
        public Transform Transform => transform;           // ISkillOwner, ITargetable
        public Transform ModelRoot => _modelRoot;
        public Transform FirePoint => _firePoint;          // ISkillOwner
        public bool IsDead => _health.IsDead;              // IAlive

        // ITargetable — 조준 타겟
        public Vector3 AimPoint
        {
            get
            {
                return _aimPoint != null ? _aimPoint.position : transform.position + Vector3.up * 0.5f;
            }
        }


        // ─── Unity Lifecycle ─────────────────────────────────────────────────
        private void Awake()
        {
            TryGetComponent(out _movement);
            TryGetComponent(out _targetFinder);
            TryGetComponent(out _health);

            _isPlaying = false;

            _target = null;
            _targetHealth = null;
            _firePoint = null;

            InGameStateManager.Instance.State
                .Where(s => s == InGameState.Playing)
                .Take(1)                                  // 첫 진입만
                .Subscribe(_ => OnStartBattle())
                .AddTo(_disposables);
        }

        private void Update()
        {
            if (_isPlaying == false)
                return;

            float dt = Time.deltaTime;

            _movement.SetMove(_joystick.IsPressed);
            _movement.SetInputDirection(_joystick.InputValue);
            _movement.Tick(dt);

            UpdateTargeting();

            _skillController.Tick(Time.deltaTime);

            _animationController.SetSpeed(_movement.AnimatorSpeed);

        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }


        // ─── Public API ──────────────────────────────────────────────────────
        public void Init(PlayerData data, PlayerAnimationController animController, JoystickBase joystick, ProjectileManager projectileMgr)
        {
            //Debug.Assert is automatically stripped in release builds
            Debug.Assert(data != null, $"{nameof(PlayerController)}::Init=> data is null");
            Debug.Assert(animController != null, $"{nameof(PlayerController)}::Init=> animController is null");
            Debug.Assert(joystick != null, $"{nameof(PlayerController)}::Init=> joystick is null");
            Debug.Assert(projectileMgr != null, $"{nameof(PlayerController)}::Init=> projectileMgr is null");

            _playerData = data;
            _movement.Init(data);
            _health.Init(data.Hp);

            _animationController = animController;
            _joystick = joystick;

            if(!DataManager.Instance.SkillDataSODic.TryGetValue(data.DefaultSkillId, out SkillDataSO skillData))
            {
                Debug.LogError($"{nameof(PlayerController)}::Init=> SkillDataSO does not exist. - DefaultSkillId: {data.DefaultSkillId}");
                return;
            }
            _skillController.Init(this, skillData, projectileMgr);
        }


        // ─── Private Logic ───────────────────────────────────────────────────
        private void UpdateTargeting()
        {
            if (_target != null)
                return;

            _targetFinder.Finding(50f);
            Transform targetTrans = _targetFinder.GetNearestTarget();
            if (targetTrans == null)
                return;

            //치명 2 수정: 전부 확보한 뒤에만 커밋 — 부분 성공 상태 차단
            if (targetTrans.TryGetComponent(out ITargetable target) == false ||
                targetTrans.TryGetComponent(out Health targetHealth) == false)
                return;

            _target = target;
            _targetHealth = targetHealth;
            _targetHealth.Died += OnTargetDied;
            _skillController.SetTarget(_target);
        }


        // ─── Event Handlers / Callbacks ──────────────────────────────────────
        // ITargetListener
        public void OnTargetDied()
        {
            _targetHealth.Died -= OnTargetDied;

            _target = null;
            _targetHealth = null;
            _skillController.SetTarget(null);
        }

        // InGameState.Playing 진입 콜백
        private void OnStartBattle()
        {
            _isPlaying = true;
        }
    }
}

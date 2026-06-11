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
    public class PlayerController : MonoBehaviour, ITargetListener, ITargetable, IAlive, ISkillOwner
    {
        [Header("모델 프리팹 링크 루트")]
        [SerializeField] private Transform _modelRoot;

        [Header("조준 타겟")]
        [SerializeField] private Transform _aimPoint;
        public Transform ModelRoot => _modelRoot;
        

        PlayerData _playerData;


        private PlayerAnimationController _animationController;
        private PlayerMovement _movement;        
        private TargetFinder _targetFinder;
        private SkillController _skillController;
        private Health _health;
        private JoystickBase _joystick;

        private bool _isPlaying;
        private ITargetable _currentTarget;
        private IAlive _crrrentTargetAlive;

        //총구 머즐 포인트 트랜스폼 설정
        private Transform _firePoint;

        private readonly CompositeDisposable _disposables = new();

        public bool IsDead => _health.IsDead;

        
        public Transform Transform => transform;

        #region ITargetable
        //조준 타겟
        public Vector3 AimPoint
        {
            get
            {
                return _aimPoint != null ? _aimPoint.position : transform.position + Vector3.up * 0.5f;
            }
        }
        #endregion

        #region ISkillOwner
        public Transform FirePoint => _firePoint;
        #endregion

        private void Awake()
        {            
            TryGetComponent(out _movement);            
            TryGetComponent(out _targetFinder);
            TryGetComponent(out _skillController);
            TryGetComponent(out _health);

            _isPlaying = false;

            _currentTarget = null;
            _crrrentTargetAlive = null;
            _firePoint = null;
        }

        public void Init(PlayerData data, PlayerAnimationController animController, JoystickBase joystick)
        {
            //Debug.Assert은 배포 빌드에서는 자동 제거됨~
            Debug.Assert(data != null, $"{nameof(PlayerController)}::Init — data is null");
            Debug.Assert(animController != null, $"{nameof(PlayerController)}::Init — animController is null");
            Debug.Assert(joystick != null, $"{nameof(PlayerController)}::Init — joystick is null");

            _playerData = data;
            _health.Init(data.Hp);

            DataManager.Instance.SkillDataSODic.TryGetValue(data.DefaultSkillId, out SkillDataSO skillData);
            _skillController.Init(skillData);

            _animationController = animController;
            _joystick = joystick;


            InGameEventBus.OnInGameStart
                .Take(1)
                .Subscribe(_ => OnGameStart())
                .AddTo(_disposables);
        }

        void OnGameStart()
        {
            _isPlaying = true;
        }

        #region ITargetListener
        public void OnTargetDied()
        {
            _currentTarget = null;
            _crrrentTargetAlive = null;
            _skillController.SetTarget(null);
        }
        #endregion

        private void Update()
        {
            if (_isPlaying == false)
                return;
            
            if ((_crrrentTargetAlive == null) || (_crrrentTargetAlive.IsDead == true))
            {
                _targetFinder.Finding(50f);
                Transform targetTrans = _targetFinder.GetNearestTarget();
                if (targetTrans != null)
                {
                    targetTrans.TryGetComponent(out _currentTarget);
                    targetTrans.TryGetComponent(out _crrrentTargetAlive);

                    if (_currentTarget != null)
                    {
                        _skillController.SetTarget(_currentTarget);
                    }    
                    //Debug.Log($"{Time.deltaTime} - {targetTrans.gameObject.name}");
                }
            }

            _movement.SetMove(_joystick.IsPressed);
            _movement.SetInputDirection(_joystick.InputValue);
            _animationController.SetSpeed(_movement.AnimatorSpeed);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}

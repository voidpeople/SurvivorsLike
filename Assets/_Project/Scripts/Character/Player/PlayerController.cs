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
    public class PlayerController : MonoBehaviour
    {
        [Header("모델 프리팹 링크 루트")]
        [SerializeField] private Transform _modelRoot;
        public Transform ModelRoot => _modelRoot;

        PlayerData _playerData;


        private PlayerAnimationController _animationController;
        private PlayerMovement _movement;        
        private TargetFinder _targetFinder;
        private SkillController _skillController;
        private Health _health;
        private JoystickBase _joystick;

        private bool _isPlaying;

        private readonly CompositeDisposable _disposables = new();


        private void Awake()
        {            
            TryGetComponent(out _movement);            
            TryGetComponent(out _targetFinder);
            TryGetComponent(out _skillController);
            TryGetComponent(out _health);

            _isPlaying = false;
        }

        public void Init(PlayerData data, PlayerAnimationController animController, JoystickBase joystick)
        {
            _playerData = data;
            _health.Init(data.Hp);

            _animationController = animController;
            if (_animationController == null)
                Debug.LogError($"PlayerController::Init - _animationController is Null.");

            _joystick = joystick;
            if (_joystick == null)
                Debug.LogError($"PlayerController::Init - _joystick is Null.");

            //기본 스킬이 쿠나이 스킬 추가
            if (DataManager.Instance.SkillDataSODic.TryGetValue(1001, out SkillDataSO skillData) == true)
            {
                _skillController.AddSkill(skillData);
            }

            InGameEventBus.OnInGameStart
                .Take(1)
                .Subscribe(_ => OnGameStart())
                .AddTo(_disposables);
        }

        void OnGameStart()
        {
            _isPlaying = true;
        }

        private void Update()
        {
            if (_isPlaying == false)
                return;

            _targetFinder.Finding(50f);
            //Transform trans = _targetFinder.GetNearestTarget();
            //if(trans != null)
            //{
            //    Debug.Log($"{Time.deltaTime} - {trans.gameObject.name}");
            //}

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

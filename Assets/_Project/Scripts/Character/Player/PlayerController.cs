using SurvivorsLike;
using UnityEngine;
using UnityEngine.InputSystem;



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
        [SerializeField] private JoystickBase _joystick;
        private PlayerAnimationController _animationController;

        private PlayerMovement  _movement;

        private void Awake()
        {
            TryGetComponent(out _movement);
            _animationController = GetComponentInChildren<PlayerAnimationController>();
        }

        private void Update()
        {
            _movement.SetMove(_joystick.IsPressed);

            _movement.SetInputDirection(_joystick.InputValue);
            _animationController.SetSpeed(_movement.AnimatorSpeed);
        }
    }
}

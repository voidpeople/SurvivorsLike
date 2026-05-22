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
        private CharacterAnimatorController _animatorCtrler;

        private PlayerMover  _mover;

        private void Awake()
        {
            TryGetComponent(out _mover);
            _animatorCtrler = GetComponentInChildren<CharacterAnimatorController>();
        }

        private void Update()
        {
            _mover.SetMove(_joystick.IsPressed);

            _mover.SetInputDirection(_joystick.InputValue);
            _animatorCtrler.SetSpeed(_mover.AnimatorSpeed);
        }
    }
}

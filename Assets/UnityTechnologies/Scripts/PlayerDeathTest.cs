using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;


/// <summary>
/// 1번 키로 플레이어 사망 테스트 (에디터 디버그용)
/// </summary>
public class PlayerDeathTest : MonoBehaviour
{
    private Animator _animator;
    private ThirdPersonController _controller;
    private CharacterController _characterController;

    private static readonly int IsDeadHash = Animator.StringToHash("IsDead");
    private static readonly int IsHitHash  = Animator.StringToHash("IsHit");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<ThirdPersonController>();
        _characterController = GetComponent<CharacterController>();
    }

private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            TriggerDeath();

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            TriggerHit();
    }

    private void TriggerDeath()
    {
        if (_animator.GetBool(IsDeadHash)) return;

        _animator.SetBool(IsDeadHash, true);

        // 이동 입력 차단
        if (_controller != null) _controller.enabled = false;
        if (_characterController != null) _characterController.enabled = false;
    }

private void TriggerHit()
    {
        if (_animator.GetBool(IsDeadHash)) return;

        _animator.SetTrigger(IsHitHash);
    }

}

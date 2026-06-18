using UnityEngine;


namespace SurvivorsLike
{
    public class PlayerAnimationController : MonoBehaviour
    {
        protected static readonly int SpeedHash = Animator.StringToHash("Speed");
        protected static readonly int IsDeadHash = Animator.StringToHash("IsDead");

        protected Animator _animator;

        protected virtual void Awake()
        {
            TryGetComponent(out _animator);  // 같은 오브젝트, 1회 캐싱
            Debug.Assert(_animator != null, $"{nameof(PlayerAnimationController)}::Awake=> Animator not found");
        }

        public void SetSpeed(float speed)
        {
            _animator.SetFloat(SpeedHash, speed);
        }

        public void SetDead()
        {
            _animator.SetBool(IsDeadHash, true);
        }
    }
}

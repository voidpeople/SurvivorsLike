using UnityEngine;


namespace SurvivorsLike
{
    public class EnemyAnimationController : MonoBehaviour
    {
        protected static readonly int SpeedHash = Animator.StringToHash("Speed");
        protected static readonly int IsAttackHash = Animator.StringToHash("IsAttack");

        protected Animator _animator;

        protected virtual void Awake()
        {
            TryGetComponent(out _animator);
            Debug.Assert(_animator != null, $"{nameof(EnemyAnimationController)}::Awake=> Animator not found");
        }

        public void PlayIdle()
        {
            _animator.SetFloat(SpeedHash, 0f);
        }

        public void PlayMove()
        {
            _animator.SetFloat(SpeedHash, 1f);
        }

        public void PlayAttack(bool isAttack)
        {
            _animator.SetBool(IsAttackHash, isAttack);
        }

        public void Spawn()
        {
            _animator.SetFloat(SpeedHash, 0f);
            _animator.SetBool(IsAttackHash, false);
        }
    }
}

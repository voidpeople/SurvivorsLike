using UnityEngine;


namespace SurvivorsLike
{
    public class EnemyAnimationController : MonoBehaviour
    {
        protected static readonly int IsMoveHash = Animator.StringToHash("IsMove");
        protected static readonly int IsDeadHash = Animator.StringToHash("IsDead");

        protected Animator _animator;

        protected virtual void Awake()
        {
            TryGetComponent(out _animator);
        }

        public void PlayIdle()
        {
            _animator.SetBool(IsMoveHash, false);
        }

        public void PlayMove()
        {
            _animator.SetBool(IsMoveHash, true);
        }

        public void PlayDead()
        {
            _animator.SetBool(IsDeadHash, true);
        }
    }
}

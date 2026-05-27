using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    protected static readonly int IsMoveHash = Animator.StringToHash("IsMove");
    protected static readonly int IsDeadHash = Animator.StringToHash("IsDead");

    protected Animator _animator;
}

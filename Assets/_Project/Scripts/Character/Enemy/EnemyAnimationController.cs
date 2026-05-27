using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    protected static readonly int IsMoveHash = Animator.StringToHash("IsMove");
    protected static readonly int IsDeadHash = Animator.StringToHash("IsDead");

    protected Animator _animator;

    protected virtual void Awake()
    {
        TryGetComponent(out _animator); 
    }

    public void SetMove(bool isMove)
    {
        _animator.SetBool(IsMoveHash, isMove);
    }

    public void SetDead()
    {
        _animator.SetBool(IsDeadHash, true);
    }
}

using UnityEngine;

namespace SurvivorsLike
{
    public enum EnemyStateType : int
    {
        Idle,
        Chase,
        Attack,
        Dead
    }

    public class EnemyFSM : FSM<EnemyStateType, EnemyStateBase>
    {
    }
}

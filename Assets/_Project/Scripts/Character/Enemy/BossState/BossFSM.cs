using SurvivorsLike;
using UnityEngine;


namespace SurvivorsLike
{
    public enum BossStateType : int
    {
        Intro,
        Phase1,
        Phase2,
        SpecialAttack,
        Dead
    }

    public class BossFSM : FSM<BossStateType, BossStateBase>
    {
    }
}

using SurvivorsLike;
using UnityEngine;

public class MeleeSkill : SkillBase
{
    public override void Init()
    {
        base.Init();
    }

    public override void UseSkill()
    {
        //근접 스킬이라고 해도 일정 방향을 공격하는 근접 스킬이 있고
        //캐릭터 주위 일정 영역 안의 모든 적을 공격하는 근접 공격이 있다.
    }
}

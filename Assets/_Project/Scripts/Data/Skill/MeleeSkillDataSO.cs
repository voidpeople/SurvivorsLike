using SurvivorsLike;
using UnityEngine;

namespace SurvivorsLike
{
    //스킬 테이블 정보
    [CreateAssetMenu(fileName = "ChapterDataSO", menuName = "SurvivorsLike/Data/MeleeSkillDataSO")]
    public class MeleeSkillDataSO : SkillDataSO
    {
        public float AttackRange;
        public float AttackAngle; //캐릭터 정면을 기준으로 어느 정도 앵글 범위로 스킬을 사용할 것인가? 만약 360이면 모든 범위 공격~
    }
}

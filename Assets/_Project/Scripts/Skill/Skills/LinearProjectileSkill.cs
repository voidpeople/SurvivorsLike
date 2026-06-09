using SurvivorsLike;
using UnityEngine;


namespace SurvivorsLike
{
    //쿠나이, 드릴샷 
    public class LinearProjectileSkill : SkillBase
    {
        private LinearProjectileSkillDataSO _linearProjectileSkillData;

        public override void Init(SkillDataSO data, int level = 1)
        {
            base.Init(data, level);

            _linearProjectileSkillData = data as LinearProjectileSkillDataSO;
            Debug.Assert(_linearProjectileSkillData != null,
                $"{nameof(LinearProjectileSkill)}::Init — data를 LinearProjectileSkillDataSO로 캐스팅 실패");
        }

        public override void OnUseSkill()
        {
        }
    }
}

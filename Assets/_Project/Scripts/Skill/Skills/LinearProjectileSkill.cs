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
            if (_linearProjectileSkillData == null)
            {
                Debug.LogError($"LinearProjectileSkill::Init() - data is null!");
            }
        }

        public override void OnUseSkill()
        {
        }
    }
}

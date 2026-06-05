using UnityEngine;

namespace SurvivorsLike
{
    public class MeleeSkill : SkillBase
    {
        public override void Init(SkillDataSO data, int level = 1)
        { 
            base.Init(data, level);
        }

        public override bool UseSkill()
        {
            return base.UseSkill();
        }

        public override void OnUseSkill()
        {
        }
    }
}

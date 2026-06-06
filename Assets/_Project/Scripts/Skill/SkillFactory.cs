using System;
using UnityEngine;


namespace SurvivorsLike
{
    public static class SkillFactory
    {
        public static SkillBase Create(SkillDataSO data)
        {
            if (data == null)
            {
                Debug.LogError($"SkillFactory::Create() - data is null!");
                return null;
            }

            SkillBase skill = null;
            switch (data)
            {
                case MeleeSkillDataSO:
                    skill = new MeleeSkill(); break;
                case LinearProjectileSkillDataSO:
                    skill = new LinearProjectileSkill(); break;
                default:
                    Debug.LogError($"SkillFactory::Create() - 미 등록 스킬 타입: {data.GetType().Name}");
                    skill = null;
                    break;
            }

            return skill;
        }
    }
}

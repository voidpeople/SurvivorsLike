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
                Debug.LogError($"{nameof(SkillFactory)}::Create=> data is null!");
                return null;
            }

            SkillBase skill = data switch
            {
                MeleeSkillDataSO => new MeleeSkill(),
                LinearProjectileSkillDataSO => new LinearProjectileSkill(),
                _ => null
            };

            if (skill == null)
                Debug.LogError($"{nameof(SkillFactory)}::Create=> Unregistered SkillDataSO type: {data.GetType().Name}");

            return skill;
        }
    }
}

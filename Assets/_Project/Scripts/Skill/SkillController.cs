using System.Collections.Generic;
using UnityEngine;



namespace SurvivorsLike
{
    public class SkillController : MonoBehaviour
    {
        //8개의 스킬 슬롯
        private const int MaxSkillSlot = 8;

        private readonly List<SkillBase> _skillList = new List<SkillBase>(MaxSkillSlot);


        public void Init()
        {

        }

        public void UseAllSkill()
        {
            foreach (SkillBase skill in _skillList)
            {
                skill.UseSkill();
            }
        }

        public void StopAllSkill()
        {
            foreach (SkillBase skill in _skillList)
            {
                skill.StopSkill();
            }
        }

        public void UseSkill(int skillId)
        {

        }

        public void StopSkill(int skillId)
        {

        }

        private void Update()
        {
            //_skills
        }
    }
}

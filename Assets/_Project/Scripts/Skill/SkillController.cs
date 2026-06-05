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
            _skillList.Clear();
        }

        public bool AddSkill(SkillDataSO data)
        {
            return true;
        }

        public void UpgradeSkill(int skillId, int newLevel)
        {

        }

        private void Update()
        {
            float dt = Time.deltaTime;
            int count = _skillList.Count;
            for(int ii = 0; ii < count; ++ii)
            {
                _skillList[ii].Tick(dt);
                _skillList[ii].TryUseSkill();
            }
        }
    }
}

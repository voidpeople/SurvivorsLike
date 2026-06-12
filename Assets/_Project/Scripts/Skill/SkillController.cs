using R3;
using System;
using System.Collections.Generic;
using UnityEngine;



namespace SurvivorsLike
{
    public class SkillController
    {
        //8개의 스킬 슬롯
        private const int MaxSkillSlot = 8;

        private readonly List<SkillBase> _skillList = new List<SkillBase>(MaxSkillSlot);

        private ISkillOwner _owner;


        public void Init(ISkillOwner owner, SkillDataSO defaultSkillData)
        {
            Debug.Assert(owner != null, $"{nameof(SkillController)}::Init => ISkillOwner is null");
            Debug.Assert(defaultSkillData != null, $"{nameof(SkillController)}::Init => defaultSkillData is null");

            _owner = owner;
            _skillList.Clear();
            AddSkill(defaultSkillData);
        }

        public void SetTarget(ITargetable target)
        {
            //Debug.Assert(target != null, $"{nameof(SkillController)}::SetTarget — target is null.");

            int count = _skillList.Count;
            for (int ii = 0; ii < count; ++ii)
            {
                //타겟의 위치 정보가 필요한 스킬
                //타겟의 위치 정보가 필요 없는 스킬
                _skillList[ii].SetTarget(target);
            }
        }

        public bool AddSkill(SkillDataSO data)
        {
            if (_skillList.Count >= MaxSkillSlot)
                return false;

            SkillBase skill = SkillFactory.Create(data);
            skill.Init(_owner, data);
            _skillList.Add(skill);

            return true;
        }

        public void UpgradeSkill(int skillId, int newLevel)
        {
            int count = _skillList.Count;
            for (int ii = 0; ii < count; ++ii)
            {
                if(_skillList[ii].SkillId == skillId)
                {
                    _skillList[ii].SetLevel(newLevel);
                    return;
                }
            }
        }

        public  void Tick(float deltaTime)
        {
            int count = _skillList.Count;
            for(int ii = 0; ii < count; ++ii)
            {
                _skillList[ii].Tick(deltaTime);
                _skillList[ii].TryUseSkill();
            }
        }
    }
}

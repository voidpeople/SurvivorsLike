using System.Collections.Generic;
using UnityEditor.ShaderGraph.Configuration;
using UnityEngine;



namespace SurvivorsLike
{
    public class SkillController
    {
        //8개의 스킬 슬롯
        private const int MaxSkillSlot = 8;

        private readonly List<SkillBase> _skillList = new List<SkillBase>(MaxSkillSlot);

        private ISkillOwner _owner;

        private ProjectileManager _projectileMgr;

        private ITargetProvider _targetProvider;

        public void Init(ISkillOwner owner, SkillDataSO defaultSkillData, ProjectileManager projectileMgr, ITargetProvider targetProvider)
        {
            Debug.Assert(owner != null, $"{nameof(SkillController)}::Init=> owner is null");
            Debug.Assert(defaultSkillData != null, $"{nameof(SkillController)}::Init=> defaultSkillData is null");
            Debug.Assert(projectileMgr != null, $"{nameof(SkillController)}::Init=> projectileMgr is null");
            Debug.Assert(targetProvider != null, $"{nameof(SkillController)}::Init=> targetProvider is null");

            _owner = owner;
            _projectileMgr = projectileMgr;
            _targetProvider = targetProvider;
            _skillList.Clear();
            AddSkill(defaultSkillData);
            
        }

        public bool AddSkill(SkillDataSO data)
        {
            if (_skillList.Count >= MaxSkillSlot)
                return false;

            SkillBase skill = SkillFactory.Create(data, _projectileMgr, _targetProvider);
            Debug.Assert(skill != null, $"{nameof(SkillController)}::AddSkill=> Unregistered type: {data?.GetType().Name}");

            if (skill == null)
            {
                Debug.LogError($"{nameof(SkillController)}::AddSkill=> Skill creation failed: {data?.name} ({data?.GetType().Name})");
                return false;
            }
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
            }
        }
    }
}

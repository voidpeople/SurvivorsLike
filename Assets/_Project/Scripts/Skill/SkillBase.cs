using UnityEngine;
using static Unity.Cinemachine.RequiredTargetAttribute;
using static UnityEngine.UI.GridLayoutGroup;


namespace SurvivorsLike
{
    public abstract class SkillBase
    {
        protected SkillDataSO _skillData;
        protected ISkillOwner _owner;

        protected int _currentLevel = 1;
        protected float _cooldownTimer;
        

        //사용 가능 여부
        public bool IsReady => (_cooldownTimer <= 0f);

        public int SkillId => _skillData.Id;
        public int CurrentLevel => _currentLevel;



        public virtual void Init(ISkillOwner owner, SkillDataSO data, int level = 1)
        {
            Debug.Assert(owner != null, $"{nameof(SkillBase)}::Init=> owner is null");
            Debug.Assert(data != null, $"{nameof(SkillBase)}::Init=> data is null");

            _owner = owner;
            _skillData = data;
            _currentLevel = level;
            _cooldownTimer = 0f;
        }

        public void Tick(float deltaTime)
        {
            if(_cooldownTimer > 0f)
            {
                _cooldownTimer -= deltaTime;
            }

            TryUseSkill();
        }

        protected virtual bool TryUseSkill()
        {
            if (IsReady == false)
                return false;

            OnUseSkill();
            _cooldownTimer = GetCurrentLevelCooldown();

            return true;
        }

        //파생 클래스에서 구현~
        public abstract void OnUseSkill();

        protected float GetCurrentLevelCooldown()
        {
            return _skillData.GetCooldown(_currentLevel);
        }

        public void SetLevel(int level)
        {
            _currentLevel = level;
        }

        public virtual void StopSkill()
        {
            _cooldownTimer = 0f;
        }
    }
}

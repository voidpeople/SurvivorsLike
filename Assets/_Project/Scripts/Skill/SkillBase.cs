using UnityEngine;


namespace SurvivorsLike
{
    public abstract class SkillBase
    {
        protected SkillDataSO _skillData;
        private int _currentLevel = 1;
        private float _cooldownTimer;

        //사용 가능 여부
        public bool IsReady => (_cooldownTimer <= 0f);

        public int SkillId => _skillData.Id;


        public virtual void Init(SkillDataSO data, int level = 1)
        {
            if (data == null)
            {
                Debug.LogError($"SkillBase::Init() - data is null!");
                return;
            }

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
        }

        public virtual bool TryUseSkill()
        {
            if (IsReady == false)
                return false;

            _cooldownTimer = GetCurrentLevelCooldown();
            OnUseSkill();

            return true;
        }

        //파생 클래스에서 구현~
        public abstract void OnUseSkill();

        private float GetCurrentLevelCooldown()
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

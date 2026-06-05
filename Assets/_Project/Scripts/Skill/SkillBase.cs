using UnityEngine;


namespace SurvivorsLike
{
    public abstract class SkillBase
    {
        protected SkillDataSO _skillData;
        private int _currentLevel = 1;
        private float _cooldownTimer;

        //사용 가능 여부
        public bool IsReady { get { return (_cooldownTimer <= 0f); } }


        public virtual void Init(SkillDataSO data, int level = 1)
        {
            _skillData = data;
            _currentLevel = level;
            _cooldownTimer = 0f;
        }

        public virtual void Tick(float dt)
        {

        }

        public virtual void UseSkill()
        {
        }

        public virtual void StopSkill()
        {
        }
    }
}

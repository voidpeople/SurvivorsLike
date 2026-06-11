using UnityEngine;
using static Unity.Cinemachine.RequiredTargetAttribute;


namespace SurvivorsLike
{
    public abstract class SkillBase
    {
        protected Transform _ownerTrasn;
        protected SkillDataSO _skillData;
        protected ITargetable _target;

        protected int _currentLevel = 1;
        private float _cooldownTimer;
        

        //사용 가능 여부
        public bool IsReady => (_cooldownTimer <= 0f);

        public int SkillId => _skillData.Id;



        public virtual void Init(Transform ownerTrans, SkillDataSO data, int level = 1)
        {
            Debug.Assert(data != null, $"{nameof(SkillBase)}::Init — data is null");

            _ownerTrasn = ownerTrans;
            _skillData = data;
            _currentLevel = level;
            _cooldownTimer = 0f;
        }

        public virtual void SetTarget(ITargetable target)
        {
            //Debug.Assert(target != null, $"{nameof(SkillBase)}::SetTarget — target is null.");
            _target = target;
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
            if ((_skillData.RequiresTarget == true) && (_target == null))
                return false;

            if (IsReady == false)
                return false;

            OnUseSkill();
            _cooldownTimer = GetCurrentLevelCooldown();

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

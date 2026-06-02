using UnityEngine;


namespace SurvivorsLike
{
    public abstract class SkillBase
    {
        private bool _isActive;

        public virtual void Init()
        {
            _isActive = false;
        }

        public virtual void SetActive(bool isActive)
        {
            _isActive = isActive;
        }

        public virtual void UseSkill()
        {
            _isActive = true;
        }

        public virtual void StopSkill()
        {
            _isActive = false;
        }
    }
}

using UnityEngine;



namespace SurvivorsLike
{
    //바운딩 스킬 (드릴샷)
    public class BouncingProjectile : PiercingProjectile
    {
        private int _bounceCount;

        public override void Init(Vector3 spawnPos, Vector3 dir, ProjectileData projectileData, LinearProjectileSkillLevelData skillData, ProjectileManager projectileMgr)
        {
            base.Init(spawnPos, dir, projectileData, skillData, projectileMgr);
            _bounceCount = projectileData.BounceCount;
            _pierceCount = int.MaxValue;
        }

        protected override bool ApplyMovement()
        {
            transform.position += _moveDir * _moveSpeed * Time.deltaTime;
            ApplyBounce();

            if (_bounceCount < 0) 
            {
                ReturnToPool();
                return false;
            }

            return true;
        }

        private void ApplyBounce()
        {
            Vector3 vp = Camera.main.WorldToViewportPoint(transform.position);
            bool bounced = false;

            //x축 영역 검사
            if (vp.x < 0f || vp.x > 1f)
            {
                _moveDir.x = -_moveDir.x;
                vp.x = Mathf.Clamp01(vp.x);
                bounced = true;
            }

            //y축 영역 검사
            if (vp.y < 0f || vp.y > 1f)
            {
                _moveDir.z = -_moveDir.z;
                vp.y = Mathf.Clamp01(vp.y);
                bounced = true;
            }

            if (bounced)
            {
                transform.rotation = Quaternion.LookRotation(_moveDir);
                transform.position = Camera.main.ViewportToWorldPoint(vp);                
                --_bounceCount;
            }
        }
    }
}

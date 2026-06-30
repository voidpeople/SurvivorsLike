using UnityEngine;



namespace SurvivorsLike
{
    //관통 스킬 (관통창)
    public class BouncingProjectile : Projectile
    {
        private int _pierceCount;

        public override void Init(Vector3 spawnPos, Vector3 dir, float collisionRadius, LinearProjectileSkillLevelData skillData, ProjectileManager projectileMgr)
        {
            base.Init(spawnPos, dir, collisionRadius, skillData, projectileMgr);
            _pierceCount = skillData.PierceCount;
        }

        public override void DetectHits()
        {

        }
    }
}

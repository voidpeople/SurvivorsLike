using UnityEngine;



namespace SurvivorsLike
{
    //관통 스킬 (관통창)
    public class BouncingProjectile : PiercingProjectile
    {
        private int _bounceCount;

        public override void Init(Vector3 spawnPos, Vector3 dir, ProjectileData projectileData, LinearProjectileSkillLevelData skillData, ProjectileManager projectileMgr)
        {
            base.Init(spawnPos, dir, projectileData, skillData, projectileMgr);
            _bounceCount = projectileData.BounceCount;
        }
    }
}

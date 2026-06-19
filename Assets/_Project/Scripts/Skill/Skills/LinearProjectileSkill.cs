using SurvivorsLike;
using System.Runtime.CompilerServices;
using UnityEngine;


namespace SurvivorsLike
{
    //쿠나이 스킬만 자동 조준 이므로 타겟 정보가 필요함~
    //쿠나이 - 자동 조준
    //샷건, 권총 - 바라보는 방향 공격
    public class LinearProjectileSkill : SkillBase
    {
        private LinearProjectileSkillDataSO _linearProjectileSkillData;
        private ProjectileManager _projectileMgr;
        private ITargetProvider _targetProvider;
        protected ITargetable _target;

        public LinearProjectileSkill(ProjectileManager projectileMgr, ITargetProvider targetProvider)
        {
            _projectileMgr = projectileMgr;
            _targetProvider = targetProvider;
        }

        public override void Init(ISkillOwner owner, SkillDataSO data, int level = 1)
        {
            base.Init(owner, data, level);

            _linearProjectileSkillData = data as LinearProjectileSkillDataSO;
            Debug.Assert(_linearProjectileSkillData != null,
                $"{nameof(LinearProjectileSkill)}::Init=> data를 LinearProjectileSkillDataSO로 캐스팅 실패");
        }

        //인라인 명령
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Vector3 GetSpawnPos(Vector3 dir)
        {
            return _owner.FirePoint != null
                ? _owner.FirePoint.position
                : _owner.Transform.position + (Vector3.up * 0.5f) + (dir * 0.2f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Vector3 GetFireDirection()
        {
            if(null != _target)
            {
                Vector3 dir = _target.Transform.position - _owner.Transform.position;
                dir.y = 0f;
                return dir.normalized;
            }

            return _owner.Transform.forward;
        }


        protected override bool TryUseSkill()
        {
            //타겟이 필요한 LinearProjectile 스킬
            //타겟이 필요 없는 LinearProjectile 스킬

            if (IsReady == false)
                return false;

            
            //타겟이 필요한 스킬인데 타겟이 없다면 함수 종료~
            if (_skillData.RequiresTarget == true)
            {
                _target = _targetProvider.GetNearest();
                if (_target == null)
                    return false;
            }

            OnUseSkill();
            _cooldownTimer = GetCurrentLevelCooldown();

            return true;
        }

        public override void OnUseSkill()
        {
            DataManager.Instance.ProjectileDataDic.TryGetValue(_linearProjectileSkillData.ProjectileId, out ProjectileData projectileData);
            if (projectileData == null)
            {
                Debug.LogError($"{nameof(LinearProjectileSkill)}::OnUseSkill=> ProjectileData does not exist. - ProjectileId: {_linearProjectileSkillData.ProjectileId}");
                return;
            }

            Projectile projectile = PoolManager.Instance.Get<Projectile>(projectileData.PrefabKey);
            if (projectile == null)
            {
                Debug.LogError($"{nameof(LinearProjectileSkill)}::OnUseSkill=> Projectile not found. - PrefabKey: {projectileData.PrefabKey}");
                return;
            }

            LinearProjectileSkillLevelData skillData = _linearProjectileSkillData.GetLevelData(_currentLevel);
            Vector3 dir = GetFireDirection();
            projectile.Init(GetSpawnPos(dir), dir, skillData.ProjectileSpeed, skillData.Damage, projectileData.ColliderRadius, _projectileMgr);
        }

    }
}

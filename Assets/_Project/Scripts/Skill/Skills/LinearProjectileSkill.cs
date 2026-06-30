using System.Runtime.CompilerServices;
using UnityEngine;


namespace SurvivorsLike
{
    //쿠나이 스킬만 자동 조준 이므로 타겟 정보가 필요함~
    //쿠나이 - 자동 조준
    //샷건, 권총 - 바라보는 방향 공격
    public class LinearProjectileSkill : SkillBase
    {
        private const float PROJECTILE_SIDE_OFFSET = 0.15f;

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

            LinearProjectileSkillLevelData skillData = _linearProjectileSkillData.GetLevelData(_currentLevel);
            Vector3 baseDir  = GetFireDirection();
            Vector3 spawnPos = GetSpawnPos(baseDir);
            Vector3 right    = Vector3.Cross(Vector3.up, baseDir).normalized;

            int   count       = skillData.ProjectileCount;
            float startOffset = -(count - 1) * PROJECTILE_SIDE_OFFSET * 0.5f;

            for (int ii = 0; ii < count; ii++)
            {
                Projectile projectile = PoolManager.Instance.Get<Projectile>(projectileData.PrefabKey);
                if (projectile == null)
                {
                    Debug.LogError($"{nameof(LinearProjectileSkill)}::OnUseSkill=> Projectile not found. - PrefabKey: {projectileData.PrefabKey}");
                    return;
                }

                //발사 방향을 기준으로 좌에서 우로 약간씩 오프셋을 주어 발사체를 발사함~
                //연속 발사시 발사체가 겹치는 것을 방지~
                Vector3 offsetPos = spawnPos + right * (startOffset + PROJECTILE_SIDE_OFFSET * ii);
                projectile.Init(offsetPos, baseDir, projectileData.ColliderRadius, skillData, _projectileMgr);
            }
        }

    }
}

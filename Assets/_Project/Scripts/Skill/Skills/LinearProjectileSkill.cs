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

        public override void Init(ISkillOwner owner, SkillDataSO data, int level = 1)
        {
            base.Init(owner, data, level);

            _linearProjectileSkillData = data as LinearProjectileSkillDataSO;
            Debug.Assert(_linearProjectileSkillData != null,
                $"{nameof(LinearProjectileSkill)}::Init=> data를 LinearProjectileSkillDataSO로 캐스팅 실패");
        }

        public override void SetTarget(ITargetable target)
        {
            _target = target;
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

            LinearProjectileSkillLevelData data = _linearProjectileSkillData.GetLevelData(_currentLevel);
            Vector3 dir = GetFireDirection();
            projectile.Init(GetSpawnPos(dir), dir, data.ProjectileSpeed, data.Damage);
        }

    }
}

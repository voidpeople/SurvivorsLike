using SurvivorsLike;
using UnityEngine;


namespace SurvivorsLike
{
    //쿠나이 스킬만 자동 조준 이므로 타겟 정보가 필요함~
    //쿠나이 - 자동 조준
    //샷건, 권총 - 바라보는 방향 공격
    public class LinearProjectileSkill : SkillBase
    {
        private LinearProjectileSkillDataSO _linearProjectileSkillData;

        public override void Init(Transform ownerTrans, SkillDataSO data, int level = 1)
        {
            base.Init(ownerTrans, data, level);

            _linearProjectileSkillData = data as LinearProjectileSkillDataSO;
            Debug.Assert(_linearProjectileSkillData != null,
                $"{nameof(LinearProjectileSkill)}::Init — data를 LinearProjectileSkillDataSO로 캐스팅 실패");
        }

        public override void SetTarget(ITargetable target)
        {
            _target = target;
        }

        private Vector3 GetFireDirection()
        {
            if(null != _target)
            {
                Vector3 dir = _target.Transform.position - _ownerTrasn.position;
                dir.y = 0f;
                return dir.normalized;
            }

            return _ownerTrasn.forward;
        }

        public override void OnUseSkill()
        {            
            //발사체를 타겟을 향해 발사하는 로직 추가
            //ProjectileBase projectile = PoolManager.Instance.Get<ProjectileBase>("projectile/kunai");
            ProjectileBase projectile = PoolManager.Instance.Get<ProjectileBase>(_linearProjectileSkillData.PrefabKey);
            if (projectile == null)
            {
                Debug.LogError($"{nameof(LinearProjectileSkill)}::OnUseSkill => Projectile not found. - PrefabKey: {_linearProjectileSkillData.PrefabKey}");
                return;
            }
            projectile.Init(_ownerTrasn.position + new Vector3(0f, 0.5f, 0f), GetFireDirection(), _linearProjectileSkillData.GetLevelData(_currentLevel).ProjectileSpeed);
        }

    }
}

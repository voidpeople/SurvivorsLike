using System.Collections.Generic;
using UnityEngine;

namespace SurvivorsLike
{
    //관통 스킬
    public class PiercingProjectile : Projectile
    {
        private int _pierceCount;

        private readonly HashSet<Health> _prevSet = new(16);
        private readonly HashSet<Health> _enterBuffer = new(16);
        private readonly HashSet<Health> _exitBuffer = new(16);

        public override void Init(Vector3 spawnPos, Vector3 dir, ProjectileData projectileData, LinearProjectileSkillLevelData skillData, ProjectileManager projectileMgr)
        {
            base.Init(spawnPos, dir, projectileData, skillData, projectileMgr);
            _pierceCount = skillData.PierceCount;

            _prevSet.Clear();
            _enterBuffer.Clear();
            _exitBuffer.Clear();
        }

        public override void DetectHits()
        {
            int count = Physics.OverlapCapsuleNonAlloc(
                _prevPos, transform.position, _collisionRadius,
                _overlapResults, _targetLayer, QueryTriggerInteraction.Collide);

            _enterBuffer.Clear();
            _exitBuffer.Clear();

            //새로 충돌 영역에 들어온 타겟들을 _enterBuffer에 추가
            for (int ii = 0; ii < count; ii++)
            {
                if(!_overlapResults[ii].TryGetComponent(out Health health))
                    continue;

                if (!_prevSet.Contains(health))
                    _enterBuffer.Add(health);
            }

            //이전 프레임에 충돌 영역에 들어온 타겟들이 충돌 영역을 벗어 났는지 검사
            foreach (Health prevHealth in _prevSet)
            {
                bool stillIn = false;
                for (int ii = 0; ii < count; ii++)
                {
                    if (_overlapResults[ii].TryGetComponent(out Health health)
                        && health == prevHealth)
                    {
                        stillIn = true;
                        break;
                    }
                }

                //발사체의 충돌 영역에서 나간 타겟들을 _exitBuffer 버퍼에 추가
                if (!stillIn)
                    _exitBuffer.Add(prevHealth);
            }

            //충돌 영역에 들어온 타겟들에 데미지 적용
            foreach (Health health in _enterBuffer)
            {
                //_pierceCount 값은 관통 횟수 이므로 _pierceCount가 0이면 첫 번째 충돌만 있는 것임~
                //_pierceCount가 3이면 총 4번의 충돌이 있음~
                health.TakeDamage(_damage);
                _prevSet.Add(health);

                --_pierceCount;
                if (_pierceCount < 0)
                {
                    ReturnToPool();
                    break;
                }
            }

            //충돌 영역에서 벗엇난 타겟들을 _prevSet에서 제거
            foreach (Health health in _exitBuffer)
            {
                _prevSet.Remove(health);
            }
        }

        public override void OnDespawn()
        {
            base.OnDespawn();
            _prevSet.Clear();
            _enterBuffer.Clear();
            _exitBuffer.Clear();
        }
    }
}

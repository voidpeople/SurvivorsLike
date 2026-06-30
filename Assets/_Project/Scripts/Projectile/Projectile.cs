using UnityEngine;


namespace SurvivorsLike
{
    public class Projectile : MonoBehaviour, ITickable, IPoolable
    {
        [Header("설정")]
        [SerializeField] private float _maxRange = 35f;       //최대 사거리 (단위: m)
        [SerializeField] private float _lifetime = 10f;       //사거리 도달 실패 시 강제 소멸 시간 (안전장치)
        [SerializeField] protected LayerMask _targetLayer;      //피격 대상 레이어

        private Vector3 _moveDir;
        private float _moveSpeed;
        private float _rangeSqr;    

        private Vector3 _spawnPos;
        private float _spawnTime;

        private ProjectileManager _projectileMgr;

        protected float _damage;
        protected float _collisionRadius;
        protected Vector3 _prevPos; //Hit 검사 중 터널링 방지 목적
        protected static readonly Collider[] _overlapResults = new Collider[5];

        //정적 버퍼: GC 없음
        //private static readonly RaycastHit[] _hitBuffer = new RaycastHit[5];

        
        //public virtual void Init(Vector3 spawnPos, Vector3 dir, float speed, float damage, float collisionRadius, int pierceCount, ProjectileManager projectileMgr)
        public virtual void Init(Vector3 spawnPos, Vector3 dir, ProjectileData projectileData, LinearProjectileSkillLevelData skillData, ProjectileManager projectileMgr)
        {
            _spawnPos = spawnPos;
            _prevPos = spawnPos;

            transform.SetPositionAndRotation(_spawnPos, Quaternion.LookRotation(dir));

            _moveDir = dir;
            _moveSpeed = skillData.ProjectileSpeed;
            _damage = skillData.Damage;
            _collisionRadius = projectileData.ColliderRadius;

            _rangeSqr = _maxRange * _maxRange;
            _spawnTime = Time.time;

            _projectileMgr = projectileMgr;
            _projectileMgr.Register(this);            
        }

        #region IPoolable
        public virtual void OnSpawn()
        {
        }

        public virtual void OnDespawn()
        {
            _projectileMgr?.UnRegister(this);
        }

        public void ReturnToPool()
        {
            PoolManager.Instance.Return(this);
        }
        #endregion

        private bool ApplyMovement()
        {
            transform.position += _moveDir * _moveSpeed * Time.deltaTime;

            if ((transform.position - _spawnPos).sqrMagnitude >= _rangeSqr)
            {
                ReturnToPool();
                return false;
            }

            if (Time.time - _spawnTime >= _lifetime)
            {
                ReturnToPool();
                return false;
            }

            return true;
        }

        public virtual void DetectHits()
        {
            //이전→현재 경로를 캡슐로 검사 = 터널링 방지된 Overlap
            //Capsule을 _prevPos위치 부터 transform.position 위치까지 늘려서 겹칩을 검사함~
            //따라서 터널링을 방지할 수 있음~
            int count = Physics.OverlapCapsuleNonAlloc(
                _prevPos, transform.position, _collisionRadius,
                _overlapResults, _targetLayer, QueryTriggerInteraction.Collide);

            Health firstHit = null;
            float bestSqr = float.MaxValue;

            for (int ii = 0; ii < count; ++ii)
            {
                if (!_overlapResults[ii].TryGetComponent(out Health health))
                    continue;

                //여러개의 컬라이더들을 가져올 수 있으므로 그중에서 _prevPos 위치와
                //가장 가까운 걸 첫 번째 타겟으로 삼는다.
                //콜라이더 표면 최 근접점
                Vector3 closest = _overlapResults[ii].ClosestPoint(_prevPos);

                float dSqr = (closest - _prevPos).sqrMagnitude;
                if (dSqr < bestSqr)
                {
                    bestSqr = dSqr;
                    firstHit = health;
                }
            }

            if (firstHit != null)
            {
                firstHit.TakeDamage(_damage);
                ReturnToPool();
            }
        }

        void ITickable.Tick(float deltaTime)
        {
            _prevPos = transform.position;

            if (ApplyMovement())
                DetectHits();
        }
    }
}

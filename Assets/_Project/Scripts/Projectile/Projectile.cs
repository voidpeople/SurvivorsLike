using UnityEngine;


namespace SurvivorsLike
{
    //TODO: 추후에 스폰된 Projectile 오브젝트들을 ProjectManager에 등록하여 이동와 충돌 체크를 for루프에서 일괄 처리하게 구현 할 것~

    public class Projectile : MonoBehaviour, ITickable, IPoolable
    {
        [Header("설정")]
        [SerializeField] private float _maxRange = 35f;       //최대 사거리 (단위: m)
        [SerializeField] private float _lifetime = 10f;       //사거리 도달 실패 시 강제 소멸 시간 (안전장치)
        [SerializeField] private LayerMask _targetLayer;      //피격 대상 레이어

        private Vector3 _moveDir;
        private float _moveSpeed;
        private float _rangeSqr;
        private float _damage;

        private Vector3 _spawnPos;
        private float _spawnTime;

        private ProjectileManager _projectileMgr;

        //정적 버퍼: GC 없음
        private static readonly RaycastHit[] _hitBuffer = new RaycastHit[5];

        public virtual void Init(Vector3 spawnPos, Vector3 dir, float speed, float damage, ProjectileManager projectileMgr)
        {
            _spawnPos = spawnPos;
            transform.SetPositionAndRotation(_spawnPos, Quaternion.LookRotation(dir));

            _moveDir = dir;
            _moveSpeed = speed;
            _damage = damage;

            _rangeSqr = _maxRange * _maxRange;
            _spawnTime = Time.time;

            _projectileMgr = projectileMgr;
            _projectileMgr.Register(this);
        }

        #region IPoolable
        public void OnSpawn()
        {
        }

        public void OnDespawn()
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

        private void DetectHits()
        {
            Vector3 move = _moveDir * _moveSpeed * Time.deltaTime;

            int hitCount = Physics.SphereCastNonAlloc(
                transform.position, 0.3f, _moveDir, _hitBuffer,
                move.magnitude, _targetLayer,
                QueryTriggerInteraction.Collide);

            if (hitCount > 0)
            {
                Health health = _hitBuffer[0].collider.GetComponent<Health>();
                if (health == null) return;

                health.TakeDamage(_damage);
                ReturnToPool();
            }
        }

        void ITickable.Tick(float deltaTime)
        {
            if (ApplyMovement())
                DetectHits();
        }
    }
}

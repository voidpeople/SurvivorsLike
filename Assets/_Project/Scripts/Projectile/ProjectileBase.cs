using UnityEngine;


namespace SurvivorsLike
{
    //TODO: 추후에 스폰된 Projectile 오브젝트들을 ProjectManager에 등록하여 이동와 충돌 체크를 for루프에서 일괄 처리하게 구현 할 것~

    public abstract class ProjectileBase : MonoBehaviour, IPoolable
    {
        [Header("설정")]
        [SerializeField] private float _maxRange = 35f;       //최대 사거리 (단위: m)
        [SerializeField] private float _lifetime = 10f;       //사거리 도달 실패 시 강제 소멸 시간 (안전장치)
        [SerializeField] private LayerMask _targetLayer;      //피격 대상 레이어

        Vector3 _moveDir;
        float _moveSpeed;
        float _rangeSqr;
        float _damage;

        Vector3 _spawnPos;
        float _spawnTime;

        //정적 버퍼: GC 없음
        private static readonly RaycastHit[] _hitBuffer = new RaycastHit[5];

        public virtual void Init(Vector3 spawnPos, Vector3 dir, float speed, float damage)
        {
            _spawnPos = spawnPos;
            transform.SetPositionAndRotation(_spawnPos, Quaternion.LookRotation(dir));

            _moveDir = dir;
            _moveSpeed = speed;
            _damage = damage;

            _rangeSqr = _maxRange * _maxRange;
            _spawnTime = Time.time;
        }

        public void OnDespawn()
        {
        }

        public void OnSpawn()
        {
        }

        private bool ApplyMovement()
        {
            transform.position += _moveDir * _moveSpeed * Time.deltaTime;

            if ((transform.position - _spawnPos).sqrMagnitude >= _rangeSqr)
            {
                PoolManager.Instance.Return(this);
                return false;
            }

            if (Time.time - _spawnTime >= _lifetime)
            {
                PoolManager.Instance.Return(this);
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
                PoolManager.Instance.Return(this);
            }
        }

        protected void Update()
        {
            if (ApplyMovement())
                DetectHits();
        }
    }
}

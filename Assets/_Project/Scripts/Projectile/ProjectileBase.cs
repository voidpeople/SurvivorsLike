using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;


namespace SurvivorsLike
{
    //TODO: 추후에 스폰된 Projectile 오브젝트들을 ProjectManager에 등록하여 이동와 충돌 체크를 for루프에서 일괄 처리하게 구현 할 것~

    public abstract class ProjectileBase : MonoBehaviour, IPoolable
    {
        [SerializeField] private float _maxRange = 35f;  //사거리
        [SerializeField] private float _lifetime = 10f;   //소멸 안전 장치~
        [SerializeField] private LayerMask _enemyLayer;

        Vector3 _moveDir;
        float _moveSpeed;
        float _rangeSqr;

        Vector3 _spawnPos;
        float _spawnTime;

        // 정적 버퍼: GC 없음
        private static readonly RaycastHit[] _hitBuffer = new RaycastHit[5];

        public virtual void Init(Vector3 spawnPos, Vector3 dir, float speed)
        {
            _spawnPos = spawnPos;
            transform.SetPositionAndRotation(_spawnPos, Quaternion.LookRotation(dir));

            _moveDir = dir;
            _moveSpeed = speed;

            _rangeSqr = _maxRange * _maxRange;
            _spawnTime = Time.time;
        }

        public void OnDespawn()
        {
        }

        public void OnSpawn()
        {
        }

        private void ApplyMovement()
        {
            //이동
            transform.position += _moveDir * _moveSpeed * Time.deltaTime;

            //거리 체크하여 소멸
            if ((transform.position - _spawnPos).sqrMagnitude >= _rangeSqr)
            {
                PoolManager.Instance.Return(this);
                return;
            }

            if (Time.time - _spawnTime >= _lifetime)
            {
                PoolManager.Instance.Return(this);
            }
        }

        private void DetectHits()
        {
            Vector3 move = _moveDir * _moveSpeed * Time.deltaTime;

            // 충돌 검사
            int hitCount = Physics.SphereCastNonAlloc(
                transform.position, 0.3f, _moveDir, _hitBuffer,
                move.magnitude, _enemyLayer,
                QueryTriggerInteraction.Collide);

            if (hitCount > 0)
            {
                Health health = _hitBuffer[0].collider.GetComponent<Health>();
                health.TakeDamage(10);
                PoolManager.Instance.Return(this);
            }
        }

        protected void Update()
        {
            ApplyMovement();
            DetectHits();
        }
    }
}

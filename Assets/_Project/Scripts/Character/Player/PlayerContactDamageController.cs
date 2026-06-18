using System.Runtime.CompilerServices;
using UnityEngine;

namespace SurvivorsLike
{
    public class PlayerContactDamageController : MonoBehaviour
    {
        [Header("감지 설정")]
        [SerializeField] private float _collisionRadius;

        private LayerMask _enemyLayer;
        private Collider[] _contactColliderBuffer = new Collider[12];

        private Health _health;
        private float _contactDamageInterval;

        private float _timer;

        private void Awake()
        {
            _enemyLayer = 1 << LayerMask.NameToLayer("Enemy");
        }

        public void Init(Health health, float contactDamageInterval)
        {
            _health = health;
            _contactDamageInterval = contactDamageInterval;
            _timer = 0f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetMaxDamage(int count)
        {
            float damage = 0f;

            for (int ii = 0; ii < count; ++ii)
            {
                if (!_contactColliderBuffer[ii].TryGetComponent(out EnemyController enemy))
                    continue;

                if (damage < enemy.EnemyData.ContactDamage)
                    damage = enemy.EnemyData.ContactDamage;
            }

            return damage;
        }

        public void Tick(float deltaTime)
        {
            _timer += deltaTime;

            if (_timer <= _contactDamageInterval)
                return;
            _timer = 0f;

            int contactCount = Physics.OverlapSphereNonAlloc(
                transform.position,
                _collisionRadius,
                _contactColliderBuffer,
                _enemyLayer);

            if (0 < contactCount)
            {
                float damage = GetMaxDamage(contactCount);
                if ((_health != null) && (damage > 0f))
                {
                    _health.TakeDamage(damage);
                    Debug.Log("PlayerContactDamageController=> Health::TakeDamage()");
                }
            }
        }
    }
}

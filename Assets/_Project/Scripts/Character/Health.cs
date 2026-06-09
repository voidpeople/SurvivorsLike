
using System;
using UnityEngine;

namespace SurvivorsLike
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float _health;
        private bool _isDead;

        public event Action OnDead;

        public void Init(float health)
        {
            _health = health;
            _isDead = false;
        }

        public void TakeDamage(float damage)
        {
            if (_isDead == true)
                return;

            _health -= damage;
            if (_health <= 0f)
            {
                _isDead = true;
            }
        }
    }
}


using System;
using UnityEngine;

namespace SurvivorsLike
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float _health;
        public bool IsDead { get; private set; }

        public event Action Died;

        public void Init(float health)
        {
            _health = health;
            IsDead = false;
        }

        public void TakeDamage(float damage)
        {
            if (IsDead == true)
                return;

            _health -= damage;
            if (_health <= 0f)
            {
                IsDead = true;
                Died?.Invoke();
            }
        }
    }
}

using UnityEngine;


namespace SurvivorsLike
{
    public class EnemyHealth : MonoBehaviour
    {
        [SerializeField] float _health;
        private bool _isDead;

        private void OnEnable()
        {
            Init();
        }

        private void Init()
        {
            _isDead = false;
        }

        public void TakeDamage(float damage)
        {
            if (_isDead == true)
                return;

            _health -= damage;
            if(_health <= 0f)
            {
                _isDead = true;
            }
        }
    }
}

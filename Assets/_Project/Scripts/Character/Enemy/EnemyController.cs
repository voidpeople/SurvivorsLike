using UnityEngine;


namespace SurvivorsLike
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private EnemyMovement _movement;

        private Transform _playerTransform;

        public void Init(Transform playerTransform)
        {
            _playerTransform = playerTransform;
            _movement.SetTarget(playerTransform);
        }
    }
}

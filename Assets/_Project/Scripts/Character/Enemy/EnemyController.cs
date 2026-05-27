using UnityEngine;
using UnityEngine.InputSystem;


namespace SurvivorsLike
{
    public class EnemyController : MonoBehaviour
    {        
        private EnemyMovement _movement;
        private EnemyAnimationController _animationController;
        private Transform _targetTransform;

        private void Awake()
        {
            TryGetComponent(out _movement);
            _animationController = GetComponentInChildren<EnemyAnimationController>();
        }

        public void Init(Transform playerTransform)
        {
            _targetTransform = playerTransform;
            _movement.SetTarget(_targetTransform);
        }
    }
}

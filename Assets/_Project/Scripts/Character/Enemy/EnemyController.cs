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

        public void Init(Transform targetTrasnform)
        {
            _targetTransform = targetTrasnform;
            _movement.SetTarget(_targetTransform);
            _animationController.SetMove(true);

            //목적지에 도착하면 애니메이션이 멈추게 해야 한다.
        }
    }
}

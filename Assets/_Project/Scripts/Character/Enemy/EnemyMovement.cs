using UnityEngine;


namespace SurvivorsLike
{
    public class EnemyMovement : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 3f;
        [SerializeField] private float _rotateSpeed = 10f;
        [SerializeField] private float _stoppingDistance = 0.5f;

        private Transform _transform;
        private Transform _targetTransform;
        private float _sqrStoppingDistance;
        private bool _isMoving;


        private void Awake()
        {
            //성능 최적화를 위해 캐싱
            _transform = transform;
            _sqrStoppingDistance = _stoppingDistance * _stoppingDistance;
            _isMoving = false;
        }

        private void Update()
        {
            if ((_isMoving == false) || (_targetTransform == null))
                return;

            RotateToTarget();
            ApplyMovement();
        }

        //에디터의 인스펙터에서 변수 값 수정하면 즉시 반영
        private void OnValidate()
        {
            _sqrStoppingDistance = _stoppingDistance * _stoppingDistance;
        }

        public void SetTarget(Transform targetTransform)
        {
            _targetTransform = targetTransform;
            _isMoving = true;
        }

        public void Stop()
        {
            _isMoving = false;
        }

        private void ApplyMovement()
        {
            Vector3 currentPos = _transform.position;
            Vector3 targetPos = _targetTransform.position;
            targetPos.y = _transform.position.y;
            Vector3 dirVec = targetPos - currentPos;

            if (dirVec.sqrMagnitude <= _sqrStoppingDistance)
                return;

            _transform.position = Vector3.MoveTowards(
                currentPos,
                targetPos,
                _moveSpeed * Time.deltaTime);
        }

        private void RotateToTarget()
        {
            Vector3 direction = (_targetTransform.position - _transform.position);
            direction.y = 0f; // 수평 회전

            if (direction.sqrMagnitude < 0.001f) return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            _transform.rotation = Quaternion.Slerp(_transform.rotation, targetRotation, _rotateSpeed * Time.deltaTime);
        }

    }
}

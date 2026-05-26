using UnityEngine;


namespace SurvivorsLike
{
    public class EnemyMovement : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 3f;

        private Transform _transform;
        private Transform _targetTransform;
        private bool _isMoving;

        //목표 위치에 도착 후 목표를 약간 오버해서 이동 후
        //다시 목표 위치로 이동 하려는 버그 예방
        private const float ArrivalSqrThreshold = 0.01f;

        private void Awake()
        {
            //성능 최적화를 위해 캐싱
            _transform = transform;
            _isMoving = false;
        }

        private void Update()
        {
            if ((_isMoving == false) || (_targetTransform == null))
                return;

            ApplyMovement();
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
            Vector3 dirVec = targetPos - currentPos;

            if (dirVec.sqrMagnitude < ArrivalSqrThreshold)
                return;

            _transform.position = Vector3.MoveTowards(
                currentPos,
                targetPos,
                _moveSpeed * Time.deltaTime);
        }
    }
}

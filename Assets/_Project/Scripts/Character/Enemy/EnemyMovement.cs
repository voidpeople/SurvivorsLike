using System;
using UnityEngine;


namespace SurvivorsLike
{
    public class EnemyMovement : MonoBehaviour
    {       
        [Header("이동 설정")]
        [SerializeField] private float _moveSpeed = 3f;
        [SerializeField] private float _rotateSpeed = 10f;
        [SerializeField] private float _stoppingDistance = 0.5f;   // 목표 위치 도달 판정 거리 (단위: m)

        private Transform _transform;
        private Vector3 _destination;
        private float _sqrStoppingDistance;
        private bool _isMoving;

        //목표 위치에 도착하면 이벤트 발송
        public event Action OnDestinationReached;

        private void Awake()
        {
            //성능 최적화를 위해 캐싱
            _transform = transform;
            _sqrStoppingDistance = _stoppingDistance * _stoppingDistance;
            _isMoving = false;
        }

        public void Init(EnemyData data)
        {
            Debug.Assert(data != null, $"{nameof(EnemyMovement)}::Init — data is null");

            _moveSpeed = data.MoveSpeed;
        }

        private void Update()
        {
            if (_isMoving == false)
                return;

            RotateToTarget();
            ApplyMovement();
        }

        //에디터의 인스펙터에서 변수 값 수정하면 즉시 반영
        private void OnValidate()
        {
            _sqrStoppingDistance = _stoppingDistance * _stoppingDistance;
        }

        //private void SetMoveSpeed(float speed)
        //{
        //    _moveSpeed = speed;
        //}

        public void SetDestination(Vector3 destination)
        {
            _destination = destination;
            _isMoving = true;
        }

        public void Stop()
        {
            if(_isMoving == false)
                return;

            _isMoving = false;
            OnDestinationReached?.Invoke();
        }

        private void ApplyMovement()
        {
            Vector3 currentPos = _transform.position;
            Vector3 targetPos = _destination;
            targetPos.y = _transform.position.y;
            Vector3 dirVec = targetPos - currentPos;

            if (dirVec.sqrMagnitude <= _sqrStoppingDistance)
            {
                Stop();
                return;
            }

            _transform.position = Vector3.MoveTowards(
                currentPos,
                targetPos,
                _moveSpeed * Time.deltaTime);
        }

        private void RotateToTarget()
        {
            Vector3 direction = (_destination - _transform.position);
            direction.y = 0f; // 수평 회전

            if (direction.sqrMagnitude < 0.001f)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            _transform.rotation = Quaternion.Slerp(_transform.rotation, targetRotation, _rotateSpeed * Time.deltaTime);
        }
    }
}

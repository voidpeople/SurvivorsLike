using System;
using UnityEngine;


namespace SurvivorsLike
{
    public class EnemyMovement : MonoBehaviour
    {       
        [Header("이동 설정")]
        [SerializeField] private float _moveSpeed        = 3f;
        [SerializeField] private float _rotateSpeed      = 10f;
        [SerializeField] private float _stoppingDistance = 0.5f;

        [Header("분리 설정")]
        [SerializeField] private float _separationRadius = 0.8f;
        [SerializeField] private float _separationForce  = 4f;

        private Transform _transform;
        private Transform _separationTarget;
        private Vector3   _destination;
        private float     _sqrStoppingDistance;
        private float     _sqrSeparationRadius;
        private bool      _isMoving;

        //목표 위치에 도착하면 이벤트 발송
        public event Action OnDestinationReached;

        private void Awake()
        {
            _transform           = transform;
            _sqrStoppingDistance = _stoppingDistance * _stoppingDistance;
            _sqrSeparationRadius = _separationRadius * _separationRadius;
            _isMoving            = false;
        }

        public void Init(EnemyData data, Transform separationTarget)
        {
            Debug.Assert(data != null, $"{nameof(EnemyMovement)}::Init=> data is null");

            _moveSpeed        = data.MoveSpeed;
            _separationTarget = separationTarget;
        }

        private void Update()
        {
            if (_isMoving)
            {
                RotateToTarget();
                ApplyMovement();
            }

            ApplySeparation();
        }

        private void OnValidate()
        {
            _sqrStoppingDistance = _stoppingDistance * _stoppingDistance;
            _sqrSeparationRadius = _separationRadius * _separationRadius;
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

        private void ApplySeparation()
        {
            if (_separationTarget == null)
                return;

            Vector3 toSelf = _transform.position - _separationTarget.position;
            toSelf.y = 0f;
            float distSqr = toSelf.sqrMagnitude;

            if (distSqr >= _sqrSeparationRadius || distSqr < 0.0001f)
                return;

            float dist     = Mathf.Sqrt(distSqr);
            float strength = (1f - dist / _separationRadius) * _separationForce;

            Vector3 newPos = _transform.position + (toSelf / dist) * (strength * Time.deltaTime);
            newPos.y       = _transform.position.y;
            _transform.position = newPos;
        }

        //TODO: Spatial Grid (공간 분할)을 통해 서로 겹치지 않게 이동 구현 할것~
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

        public void Despawn()
        {
            _isMoving         = false;
            _destination      = _transform.position;
            _separationTarget = null;
        }
    }
}

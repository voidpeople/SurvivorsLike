using UnityEngine;


namespace SurvivorsLike
{
    public class TargetFinder : MonoBehaviour
    {
        [Header("감지 설정")]
        [SerializeField] private LayerMask _enemyLayer;
        [SerializeField] private int _maxDetectCount = 50;    // OverlapSphere 최대 감지 수 — 버퍼 크기와 동기화

        private Collider[] _findedColliderBuffer;
        private int _hitCount;

        private void Awake()
        {
            _findedColliderBuffer = new Collider[_maxDetectCount];
        }

        public void Finding(float radius)
        {
            _hitCount = Physics.OverlapSphereNonAlloc(
                transform.position,
                radius,
                _findedColliderBuffer,
                _enemyLayer);
        }

        //탐지한 컬라이더들 중에서 가장 가까운 컬라이더 찾아서 Transform을 반환하기
        public Transform GetNearestTarget()
        {
            Transform nearestTarget = null;
            float minDistSqr = float.MaxValue;
            Vector3 finderPos = transform.position;

            for(int ii = 0; ii < _hitCount; ++ii)
            {
                //사망 상태의 캐릭터는 열외~
                if (_findedColliderBuffer[ii].TryGetComponent<IAlive>(out IAlive alive) && alive.IsDead)
                    continue;

                float distSqr = (_findedColliderBuffer[ii].transform.position - finderPos).sqrMagnitude;
                if(distSqr < minDistSqr)
                {
                    minDistSqr = distSqr;
                    nearestTarget = _findedColliderBuffer[ii].transform;
                }
            }

            return nearestTarget;
        }

        //일정 범위 안의 모든 적들을 타겟으로 반환~
        public int GetInRangeTargets(float radius, Transform[] results)
        {
            int count = 0;

            return count;
        }
    }
}

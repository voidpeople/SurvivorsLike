using TriInspector;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;


namespace SurvivorsLike
{
    public class EnemySpawner : MonoBehaviour
    {
        [Title("스폰 반경 설정")]
        [SerializeField] private float _minSpawnRadius = 12f;
        [SerializeField] private float _maxSpawnRadius = 16f;

        [Title("최대 적 캐릭터 마리 수")]
        [SerializeField] private int _maxEnemyCount = 300;

        private Transform _playerTrans;
        private EnemyManager _enemyMgr;

        public void Init(Transform playerTrans, EnemyManager enemyMgr)
        {
            Debug.Assert(playerTrans != null, $"{nameof(EnemySpawner)}::Init=> playerTrans is null");
            Debug.Assert(enemyMgr   != null, $"{nameof(EnemySpawner)}::Init=> enemyMgr is null");

            _playerTrans = playerTrans;
            _enemyMgr = enemyMgr;
        }

        public void Spawn(EnemyData data, int count)
        {
            for (int ii = 0; ii < count; ++ii)
            {
                if (_enemyMgr.ActiveCount >= _maxEnemyCount)
                    return; //상한 도달 — 이번 틱 스폰 포기 (프레임 보호)

                EnemyController enemyCtrl = PoolManager.Instance.Get<EnemyController>(data.PrefabKey);
                if (enemyCtrl == null)
                {
                    Debug.LogError($"{nameof(EnemySpawner)}=> PrefabKey does not exist. - PrefabKey: {data.PrefabKey}");
                    return;
                }

                enemyCtrl.Init(data, GetRandomRingPosition(), _playerTrans, _enemyMgr);
            }
        }
        private Vector3 GetRandomRingPosition()
        {
            //랜덤 방향 × 랜덤 반지름 = 도넛 영역의 한 점 (3D 탑뷰 기준 XZ 평면)
            Vector2 dir = Random.insideUnitCircle.normalized;
            float radius = Random.Range(_minSpawnRadius, _maxSpawnRadius);
            return _playerTrans.position + new Vector3(dir.x, 0f, dir.y) * radius;
        }
    }
}

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

        private Transform _player;
        private EnemyManager _enemyManager;

        public void Init(Transform playerTrans, EnemyManager enemyMgr)
        {

        }

        public void Spawn(EnemyData data)
        {
            GameObject enemyObj = PoolManager.Instance.Get(data.PrefabKey);
            if (enemyObj == null)
            {
                Debug.LogError($"{nameof(EnemySpawner)}=> PrefabKey does not exist. - PrefabKey: {data.PrefabKey}");
                return;
            }

            enemyObj.transform.position = GetRandomRingPosition();
            if (enemyObj.TryGetComponent(out ITickable tickable))
                _enemyManager.Register(tickable);

        }
        private Vector3 GetRandomRingPosition()
        {
            //랜덤 방향 × 랜덤 반지름 = 도넛 영역의 한 점 (3D 탑뷰 기준 XZ 평면)
            Vector2 dir = Random.insideUnitCircle.normalized;
            float radius = Random.Range(_minSpawnRadius, _maxSpawnRadius);
            return _player.position + new Vector3(dir.x, 0f, dir.y) * radius;
        }
    }
}

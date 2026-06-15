using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using TriInspector;
using UnityEngine;


namespace SurvivorsLike
{
    public class WaveSystemController : MonoBehaviour
    {

        [Title("컴포넌트")]
        [SerializeField] private EnemySpawner _spawner;
        [SerializeField] private EnemyManager _enemyMgr;
        [SerializeField] private WaveManager _waveMgr;

        private readonly List<EnemyData> _createdPoolEnemyDatas = new();

        public async UniTask InitAsync(WaveDataSO data, Transform playerTrans, CancellationToken ct)
        {
            //이번 챕터에서 등장하는 적들의 프리팹 키만 수집
            HashSet<EnemyData> enemyDatas = CollectEnemyDatas(data);

            //풀 생성(어드레서블 로드) + 인스턴스 프리웜
            foreach(EnemyData enemy in enemyDatas)
            {
                (int poolInitSize, int poolMaxSize) = GetPoolSize(enemy);

                await PoolManager.Instance.CreatePoolAsync(enemy.PrefabKey, poolInitSize, poolMaxSize, ct);
                await PoolManager.Instance.PreCreateAsync(enemy.PrefabKey, poolInitSize, ct: ct);
                _createdPoolEnemyDatas.Add(enemy);
            }

            //스포너와 웨이브 매니저 초기화
            _spawner.Init(playerTrans, _enemyMgr);
            _waveMgr.Init(data, _spawner);
        }

        private static (int poolInitSize, int poolMaxSize) GetPoolSize(EnemyData data)
        {
            if (data.PoolInitSize > 0 && data.PoolMaxSize > 0)
                return (data.PoolInitSize, data.PoolMaxSize);

            return data.Type switch
            {
                EnemyType.Boss  => (1, 2),
                EnemyType.Elite => (3, 8),
                _               => (40, 120), //Normal
            };
        }

        //WaveData로 부터 생성할 모든 적 캐릭터 데이터 수집하여 반환
        private static HashSet<EnemyData> CollectEnemyDatas(WaveDataSO data)
        {
            HashSet<EnemyData> enemyDatas = new HashSet<EnemyData>();
            foreach(WaveData wave in data.WaveDataList)
            {
                if (DataManager.Instance.EnemyDataDic.TryGetValue(wave.EnemyId, out EnemyData enemyData))
                    enemyDatas.Add(enemyData);
                else
                    Debug.LogError($"{nameof(WaveSystemController)}=> EnemyId does not exist. - EnemyId: {wave.EnemyId}");
            }
            return enemyDatas;
        }
    }
}

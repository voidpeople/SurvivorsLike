using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Collections.Generic;
using System.Threading;
using TriInspector;
using UnityEngine;


namespace SurvivorsLike
{
    public class WaveSystemController : MonoBehaviour
    {

        [Title("컴포넌트")]
        [SerializeField] private WaveManager _waveMgr;
        [SerializeField] private EnemySpawner _spawner;
        [SerializeField] private EnemyManager _enemyMgr;

        //모든 웨이브를 클리어 하면 이벤트 발송
        public event Action OnAllWavesCleared;

        private readonly CompositeDisposable _disposables = new();

        public void Awake()
        {
            Debug.Assert(_waveMgr != null, $"{nameof(WaveSystemController)}::Awake=> _waveMgr is null");
            Debug.Assert(_spawner != null, $"{nameof(WaveSystemController)}::Awake=> _spawner is null");
            Debug.Assert(_enemyMgr != null, $"{nameof(WaveSystemController)}::Awake=> _enemyMgr is null");

            InGameStateManager.Instance.State
                .Where(state => state == InGameState.Playing)
                .Take(1)                                  // 첫 진입만
                .Subscribe(_ => _waveMgr.StartWave())
                .AddTo(_disposables);

            InGameStateManager.Instance.State
                .Where(state => (state == InGameState.PlayerDead) || (state == InGameState.StageClear) || (state == InGameState.StageFail))
                .Subscribe(_ => _waveMgr.StopWave())
                .AddTo(_disposables);
        }

        public async UniTask InitAsync(WaveDataSO data, Transform playerTrans, CancellationToken ct)
        {
            Debug.Assert(data        != null, $"{nameof(WaveSystemController)}::InitAsync=> data is null");
            Debug.Assert(playerTrans != null, $"{nameof(WaveSystemController)}::InitAsync=> playerTrans is null");

            //이번 챕터에서 등장하는 적들의 프리팹 키만 수집
            HashSet<EnemyData> enemyDatas = CollectEnemyDatas(data);

            //풀 생성(어드레서블 로드) + 인스턴스 프리웜
            foreach(EnemyData enemyData in enemyDatas)
            {
                (int poolInitSize, int poolMaxSize) = GetPoolSize(enemyData);

                await PoolManager.Instance.CreatePoolAsync(enemyData.PrefabKey, poolInitSize, poolMaxSize, ct);
                await PoolManager.Instance.PreCreateAsync(enemyData.PrefabKey, poolInitSize, ct: ct);

                if (!DataManager.Instance.VfxDataDic.TryGetValue(enemyData.DeathVfxId, out VfxData vfxData))
                {
                    Debug.LogError($"{nameof(WaveSystemController)}::InitAsync=> VfxData does not exist. - DeathVfxId: {enemyData.DeathVfxId})");
                    continue;
                }
                await PoolManager.Instance.CreatePoolAsync(vfxData.PrefabKey, vfxData.PoolInitSize, vfxData.PoolMaxSize, ct);
                await PoolManager.Instance.PreCreateAsync(vfxData.PrefabKey, vfxData.PoolInitSize, ct: ct);
            }

            //스포너와 웨이브 매니저 초기화
            _waveMgr.Init(data, _spawner);
            _spawner.Init(playerTrans, _enemyMgr);

            UpdateAsync(ct).Forget();
        }

        private async UniTaskVoid UpdateAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                if (_waveMgr.IsAllWavesSpawned && _enemyMgr.ActiveCount == 0)
                {
                    OnAllWavesCleared?.Invoke();
                    return;
                }
                await UniTask.Delay(200, cancellationToken: ct);
            }
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
                    Debug.LogError($"{nameof(WaveSystemController)}::CollectEnemyDatas=> EnemyData does not exist. - EnemyId: {wave.EnemyId}");
            }
            return enemyDatas;
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}

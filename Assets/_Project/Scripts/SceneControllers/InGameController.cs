using Cysharp.Threading.Tasks;
using NUnit.Framework;
using R3;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace SurvivorsLike
{
    public class InGameController : MonoBehaviour
    {
        [Header("게임플레이")]
        [SerializeField] private PlayerSpawner _playerSpawner;
        [SerializeField] private CameraController _cameraCtrl;
        [SerializeField] private MapController _mapCtrl;
        [SerializeField] private WaveSystemController _waveSystemCtrl;
        [SerializeField] private EnemyManager _enemyManager;
        [SerializeField] private GemManager _gemManager;
        [SerializeField] private InGameHudController _hudCtrl;

        [Header("스킬 선택")]
        [SerializeField] private SkillSelectionView _skillSelectionView;

        [Header("결과창")]
        [SerializeField] private Canvas _resultPanelCanvas;
        [SerializeField] private GameResultPanelView _resultPanelView;

        [Header("Development")]
        [SerializeField] private DevManager _devManager;

        private GameResultPanelModel _resultModel;
        private GameResultPanelPresenter _resultPresenter;

        private PlayerLevelSystem _playerLevelSystem;
        private SkillSelectionPresenter _skillSelectionPresenter;

        private readonly CompositeDisposable _disposables = new();

        private void Awake()
        {
            InGameStateManager.Instance.State
                .Where(s => s == InGameState.StageClear || s == InGameState.StageFail)
                .Take(1)                                   // 결과창은 1번만 (중복 차단)
                .Subscribe(OnStageFinished)
                .AddTo(_disposables);
        }

        private async UniTaskVoid Start()
        {          
            CancellationToken ct = this.GetCancellationTokenOnDestroy();
            await InitAsync(ct);
        }

        private async UniTask InitAsync(CancellationToken ct)
        {
            GameManager.Instance.SetGameState(GameState.InGame);

#if UNITY_EDITOR
            //로비로 부터 정상적인 진입이 아니라면~
            if(GameManager.Instance.GameSessionData == null)
            {
                await _devManager.PrepareInGameAsync(ct);
            }
#endif           
            GameSessionData sessionData = GameManager.Instance.GameSessionData;            

            // 모든 시스템 병렬 로드 — 각 시스템이 자신의 에셋만 책임
            await UniTask.WhenAll(
                _mapCtrl.LoadAssetsAsync(sessionData.ChapterData.MapData, ct)
            );
            await _mapCtrl.SetupMapAsync(sessionData.ChapterData.MapData, ct);

            await _playerSpawner.SpawnAsync(ct);
            _cameraCtrl.SetTarget(_playerSpawner.SpawnPlayerController.transform);
            _playerLevelSystem = _playerSpawner.SpawnPlayerController.LevelSystem;
            _hudCtrl.Init(_playerLevelSystem);

            await CreatePlayerAssetPoolAsync(sessionData, ct);
            if (!DataManager.Instance.WaveDataSODic.TryGetValue(sessionData.ChapterData.WaveId, out WaveDataSO waveDta))
            {
                Debug.LogError($"{nameof(InGameController)}::InitAsync=> WaveDataSO does not exist. - WaveId: {sessionData.ChapterData.WaveId})");
                return;
            }
            await _waveSystemCtrl.InitAsync(waveDta, _playerSpawner.SpawnPlayerController.transform, ct);

            await CreateEnemyAssetsPool(sessionData, ct);

            await CreateGemPoolAsync(ct);
            _gemManager.Init(
                _playerSpawner.SpawnPlayerController.transform,
                _playerLevelSystem,
                DataManager.Instance.GemDataSO);
            _enemyManager.OnEnemyKilled += _gemManager.HandleEnemyKilled;

            _skillSelectionPresenter = new SkillSelectionPresenter(
                _playerLevelSystem,
                _skillSelectionView);

            InitResultPanelAsync(ct);

            //3초 후 실행
            await UniTask.Delay(3000, cancellationToken: ct);

            //이벤트 발송~
            InGameStateManager.Instance.StartBattle();
        }

        private async UniTask CreatePlayerAssetPoolAsync(GameSessionData sessionData, CancellationToken ct)
        {
            if (!DataManager.Instance.SkillDataSODic.TryGetValue(sessionData.PlayerData.DefaultSkillId, out SkillDataSO skillDataSO))
            {
                Debug.LogError($"{nameof(InGameController)}::CreatePlayerAssetPoolAsync=> SkillDataSO does not exist. - DefaultSkillId: {sessionData.PlayerData.DefaultSkillId})");
                return;
            }

            List<PoolAssetRef> poolAssetRefList = new List<PoolAssetRef>();
            skillDataSO.CollectPoolAssetRef(poolAssetRefList);

            await PoolManager.Instance.CreateAsync(poolAssetRefList, ct);
        }

        private async UniTask CreateGemPoolAsync(CancellationToken ct)
        {
            GemDataSO gemDataSO = DataManager.Instance.GemDataSO;
            if (gemDataSO == null || gemDataSO.DataList == null)
            {
                Debug.LogError($"{nameof(InGameController)}::CreateGemPoolAsync=> GemDataSO is null or empty");
                return;
            }

            for (int ii = 0; ii < gemDataSO.DataList.Count; ++ii)
            {
                GemData data = gemDataSO.DataList[ii];
                await PoolManager.Instance.CreatePoolAsync(data.PrefabKey, data.PoolInitSize, data.PoolMaxSize, ct);
                await PoolManager.Instance.PreCreateAsync(data.PrefabKey, data.PoolInitSize, ct: ct);
            }
        }

        private async UniTask CreateEnemyAssetsPool(GameSessionData sessionData, CancellationToken ct)
        {
            if (!DataManager.Instance.WaveDataSODic.TryGetValue(sessionData.ChapterData.WaveId, out WaveDataSO waveDataSO))
            {
                Debug.LogError($"{nameof(InGameController)}::CreateEnemyAssetsPool=> WaveDataSO does not exist. - WaveId: {sessionData.ChapterData.WaveId})");
                return;
            }

            for(int ii = 0; ii < waveDataSO.WaveDataList.Count; ++ii)
            {                
                if (!DataManager.Instance.EnemyDataDic.TryGetValue(waveDataSO.WaveDataList[ii].EnemyId, out EnemyData enemyData))
                {
                    Debug.LogError($"{nameof(InGameController)}::CreateEnemyAssetsPool=> EnemyData does not exist. - EnemyId: {waveDataSO.WaveDataList[ii].EnemyId})");
                    continue;
                }

                await PoolManager.Instance.CreatePoolAsync(enemyData.PrefabKey, enemyData.PoolInitSize, enemyData.PoolMaxSize, ct);
                await PoolManager.Instance.PreCreateAsync(enemyData.PrefabKey, enemyData.PoolInitSize, ct: ct);

                if (!DataManager.Instance.VfxDataDic.TryGetValue(enemyData.DeathVfxId, out VfxData vfxData))
                {
                    Debug.LogError($"{nameof(InGameController)}::CreateEnemyAssetsPool=> VfxData does not exist. - DeathVfxId: {enemyData.DeathVfxId})");
                    continue;
                }
                await PoolManager.Instance.CreatePoolAsync(vfxData.PrefabKey, vfxData.PoolInitSize, vfxData.PoolMaxSize, ct);
                await PoolManager.Instance.PreCreateAsync(vfxData.PrefabKey, vfxData.PoolInitSize, ct: ct);
            }
        }

        private void InitResultPanelAsync(CancellationToken ct)
        {
            _resultModel = new GameResultPanelModel();
            _resultPanelView.Init();
            _resultPresenter = new GameResultPanelPresenter(
                _resultModel,
                _resultPanelView,
                (sceneName) => GameManager.Instance.LoadSceneAsync(sceneName),
                ct);
            HideGameResultPanel();
        }


        public void OnAllWavesCleared()
        {
            InGameStateManager.Instance.ClearStage();
        }

        // EnemyBase.OnDead() → 이 메서드 호출로 패배 처리
        public void OnPlayerDead()
        {
            InGameStateManager.Instance.FailStage();
        }

        private void OnStageFinished(InGameState state)
        {
            bool isClear = (state == InGameState.StageClear);
            ShowGameResultPanel(isClear);
        }

        private void ShowGameResultPanel(bool isClear)
        {
            _resultPanelCanvas.gameObject.SetActive(true);
            _resultPresenter.Show();
        }

        private void HideGameResultPanel()
        {
            _resultPresenter.Hide();
            _resultPanelCanvas.gameObject.SetActive(false);
        }

        private void Destroy()
        {
            _enemyManager.OnEnemyKilled -= _gemManager.HandleEnemyKilled;
            _gemManager.Clear();

            _skillSelectionPresenter.Dispose();
            _resultPresenter.Dispose();
            _resultPanelView.Destroy();
            _mapCtrl.ReleaseAssets();

            _disposables.Dispose();
        }

        private void OnDestroy()
        {
            Destroy();
        }
    }
}

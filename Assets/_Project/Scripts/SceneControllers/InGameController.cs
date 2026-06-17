using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using R3;


namespace SurvivorsLike
{
    public class InGameController : MonoBehaviour
    {
        [Header("게임플레이")]
        [SerializeField] private PlayerSpawner _playerSpawner;
        [SerializeField] private CameraController _cameraCtrl;
        [SerializeField] private MapController _mapCtrl;
        [SerializeField] private WaveSystemController _waveSystemCtrl;
        

        [Header("결과창")]
        [SerializeField] private Canvas _resultPanelCanvas;
        [SerializeField] private GameResultPanelView _resultPanelView;

        [Header("Development")]
        [SerializeField] private DevManager _devManager;

        private GameResultPanelModel _resultModel;
        private GameResultPanelPresenter _resultPresenter;

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

            //플레이어 캐릭터 스킬 프리팹들 로드
            await PoolManager.Instance.CreatePoolAsync("projectile/kunai", 50, 100, ct);
            await PoolManager.Instance.PreCreateAsync("projectile/kunai", 50, 10, ct);

            if(!DataManager.Instance.WaveDataSODic.TryGetValue(sessionData.ChapterData.WaveId, out WaveDataSO waveDta))
            {
                Debug.LogError($"{nameof(InGameController)}::InitAsync=> Failed to load WaveDataSO. - WaveId: {sessionData.ChapterData.WaveId})");
                return;
            }
            await _waveSystemCtrl.InitAsync(waveDta, _playerSpawner.SpawnPlayerController.transform, ct);

            await CreateEnemyAssetsPool(sessionData, ct);

            InitResultPanelAsync(ct);

            //3초 후 실행
            await UniTask.Delay(3000, cancellationToken: ct);

            //이벤트 발송~
            InGameStateManager.Instance.StartBattle();
        }

        private async UniTask CreatePlayerAssetsPool(GameSessionData sessionData, CancellationToken ct)
        {
            if (!DataManager.Instance.SkillDataSODic.TryGetValue(sessionData.PlayerData.DefaultSkillId, out SkillDataSO skillDataSO))
            {
                Debug.LogError($"{nameof(InGameController)}::CreatePlayerAssetsPool=> Failed to load DefaultSkillId. - DefaultSkillId: {sessionData.PlayerData.DefaultSkillId})");
                return;
            }

            ////쿠나이를 발사하는 스킬일 경우 몇개의 풀링이 필요할까?
            //await PoolManager.Instance.CreatePoolAsync(skillDataSO.PrefabKey, skillDataSO., 100, ct);
            //await PoolManager.Instance.PreCreateAsync(skillDataSO.PrefabKey, 50, 10, ct);
        }

        private async UniTask CreateEnemyAssetsPool(GameSessionData sessionData, CancellationToken ct)
        {
            if (!DataManager.Instance.WaveDataSODic.TryGetValue(sessionData.ChapterData.WaveId, out WaveDataSO waveDataSO))
            {
                Debug.LogError($"{nameof(InGameController)}::CreateAssetsPool=> Failed to load WaveDataSO. - WaveId: {sessionData.ChapterData.WaveId})");
                return;
            }

            for(int ii = 0; ii < waveDataSO.WaveDataList.Count; ++ii)
            {                
                if (!DataManager.Instance.EnemyDataDic.TryGetValue(waveDataSO.WaveDataList[ii].EnemyId, out EnemyData enemyData))
                {
                    Debug.LogError($"{nameof(InGameController)}::CreateAssetsPool=> Failed to load EnemyData. - EnemyId: {waveDataSO.WaveDataList[ii].EnemyId})");
                    continue;
                }

                await PoolManager.Instance.CreatePoolAsync(enemyData.PrefabKey, enemyData.PoolInitSize, enemyData.PoolMaxSize, ct);
                await PoolManager.Instance.PreCreateAsync(enemyData.PrefabKey, enemyData.PoolInitSize, ct: ct);

                if (!DataManager.Instance.VfxDataDic.TryGetValue(enemyData.DeathVfxId, out VfxData vfxData))
                {
                    Debug.LogError($"{nameof(InGameController)}::CreateAssetsPool=> Failed to load DeathVfxId. - DeathVfxId: {enemyData.DeathVfxId})");
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

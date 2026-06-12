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
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private MapController _mapController;


        [Header("결과창")]
        [SerializeField] private Canvas _resultPanelCanvas;
        [SerializeField] private GameResultPanelView _resultPanelView;

        [Header("Development")]
        [SerializeField] private DevManager _devManager;

        private GameResultPanelModel _resultModel;
        private GameResultPanelPresenter _resultPresenter;

        private void Awake()
        {
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

            MapDataSO mapData = GameManager.Instance.GameSessionData.ChapterData.MapData;
            // 모든 시스템 병렬 로드 — 각 시스템이 자신의 에셋만 책임
            await UniTask.WhenAll(
                _mapController.LoadAssetsAsync(mapData, ct)
            );
            await _mapController.SetupMapAsync(mapData, ct);

            await _playerSpawner.SpawnAsync(ct);
            _cameraController.SetTarget(_playerSpawner.SpawnPlayerController.transform);

            await PoolManager.Instance.CreatePoolAsync("character/enemy/spiderbot", 100, 300, ct);
            await PoolManager.Instance.PreCreateAsync("character/enemy/spiderbot", 1, 10, ct);

            if (!DataManager.Instance.EnemyDataDic.TryGetValue(2002, out EnemyData enemyData))
            {
                Debug.LogError($"{nameof(InGameController)}::InitAsync — EnemyData(2002) 로드 실패");
                return;
            }

            EnemyController enemyCtrl = PoolManager.Instance.Get<EnemyController>(enemyData.PrefabKey);
            if (enemyCtrl == null)
            {
                Debug.LogError($"{nameof(InGameController)}::InitAsync — EnemyController 풀 취득 실패");
                return;
            }

            enemyCtrl.transform.SetPositionAndRotation(new Vector3(0f, 0f, 20f), Quaternion.identity);
            enemyCtrl.Init(enemyData, _playerSpawner.SpawnPlayerController.transform);

            await PoolManager.Instance.CreatePoolAsync("vfx/explosion/explosion01", 100, 300, ct);
            await PoolManager.Instance.PreCreateAsync("vfx/explosion/explosion01", 1, 10, ct);

            //플레이어 캐릭터 스킬 프리팹들 로드
            await PoolManager.Instance.CreatePoolAsync("projectile/kunai", 50, 100, ct);
            await PoolManager.Instance.PreCreateAsync("projectile/kunai", 50, 10, ct);

            InitResultPanelAsync(ct);

            //3초 후 실행
            await UniTask.Delay(3000, cancellationToken: ct);


            //이벤트 발송~
            InGameEventBus.OnInGameStart.OnNext(Unit.Default);
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

        //스테이지 종료 콜백
        private void OnStageCleared()
        {
            ShowGameResultPanel();
        }

        // EnemyBase.OnDead() → 이 메서드 호출로 패배 처리
        public void OnPlayerDead()
        {
            ShowGameResultPanel();
        }

        private void ShowGameResultPanel()
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

            _mapController.ReleaseAssets();
        }

        private void OnDestroy()
        {
            Destroy();
        }
    }
}

using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace SurvivorsLike
{
    public class InGameController : MonoBehaviour
    {
        [Header("게임플레이")]
        [SerializeField] private MapController _mapController;
        [SerializeField] private Transform _playerTransform;

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

#if UNITY_EDITOR
            var activeScene = SceneManager.GetActiveScene();
            bool isDirectLaunch = activeScene.buildIndex != 0;
            if (isDirectLaunch)
            {
                await _devManager.PrepareInGameAsync(ct);
            }
#endif

            GameManager.Instance.SetGameState(GameState.InGame);           

            MapDataSO mapData = GameManager.Instance.SessionData.ChapterData.mapData;
            // 모든 시스템 병렬 로드 — 각 시스템이 자신의 에셋만 책임
            await UniTask.WhenAll(
                _mapController.LoadAssetsAsync(mapData, ct)
            );
            await _mapController.SetupMapAsync(mapData, ct);

            await PoolManager.Instance.CreatePoolAsync("enemy/spiderbot", 100, 300, ct);
            await PoolManager.Instance.PreCreateAsync("enemy/spiderbot", 100, 10, ct);

            EnemyController controller = PoolManager.Instance.Get<EnemyController>("enemy/spiderbot");
            controller.gameObject.SetActive(true);
            controller.transform.SetPositionAndRotation(new Vector3(0f, 0f, 20f), Quaternion.identity);
            controller.Init(_playerTransform);

            InitResultPanelAsync(ct);
        }

        private void OnDestroy()
        {      
            Destroy();
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

        // ─── 스테이지 종료 콜백 ──────────────────────────────
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

            if(PoolManager.Instance != null)
                PoolManager.Instance.ReleasePool("enemy/spiderbot");
        }
    }
}

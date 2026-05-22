using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;


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

        private GameResultPanelModel _resultModel;
        private GameResultPanelPresenter _resultPresenter;

        private void Awake()
        {
        }

        private async UniTaskVoid Start()
        {
            GameManager.Instance.SetGameState(GameState.InGame);

            CancellationToken ct = this.GetCancellationTokenOnDestroy();

            MapDataSO mapData = GameManager.Instance.SessionData.ChapterData.mapData;
            // 모든 시스템 병렬 로드 — 각 시스템이 자신의 에셋만 책임
            await UniTask.WhenAll(
                _mapController.LoadAssetsAsync(mapData, ct)
            );

            await _mapController.SetupMapAsync(mapData, ct);

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
        }
    }
}

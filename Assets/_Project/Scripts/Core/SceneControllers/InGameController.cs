using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using Tayx.Graphy.Audio;
using UnityEngine;
using UnityEngine.UIElements;


namespace SurvivorsLike
{
    public class InGameController : MonoBehaviour
    {
        [Header("게임플레이")]
        [SerializeField] private MapController _mapController;

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
            // 모든 시스템 병렬 로드 — 각 시스템이 자신의 에셋만 책임
            await UniTask.WhenAll(
                _mapController.LoadAssetsAsync(GameManager.Instance.SessionData.ChapterData.mapData, ct)
            );

            await _mapController.SetupMapAsync(ct);

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

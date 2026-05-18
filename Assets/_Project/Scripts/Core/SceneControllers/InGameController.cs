using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;


namespace SurvivorsLike
{
    public class InGameController : MonoBehaviour
    {
        [Header("결과창")]
        [SerializeField] private Canvas _resultPanelCanvas;
        [SerializeField] private GameResultPanelView _resultPanelView;

        private GameResultPanelModel _resultModel;
        private GameResultPanelPresenter _resultPresenter;

        private void Awake()
        {
            Init();
        }

        void Start()
        {
            GameManager.Instance.SetGameState(GameState.InGame);
        }

        private void OnDestroy()
        {
            Destroy();
        }

        private void Init()
        {
            CancellationToken ct = this.GetCancellationTokenOnDestroy();

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

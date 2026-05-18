using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;


namespace SurvivorsLike
{
    public class GameResultPanelPresenter : IDisposable
    {
        private readonly GameResultPanelModel _model;
        private readonly GameResultPanelView _view;

        private readonly Func<string, UniTask> _loadScene;

        private readonly CancellationToken _ct;

        public GameResultPanelPresenter(
            GameResultPanelModel model,
            GameResultPanelView view,
            Func<string, UniTask> loadScene,
            CancellationToken ct)
        {
            _model = model;
            _view = view;
            _loadScene = loadScene;
            _ct = ct;

            _view.OnResultConfirmed += OnResultConfirmed;
        }


        private void OnResultConfirmed()
        {
            StartGameAsync().Forget();
        }

        private async UniTask StartGameAsync()
        {
            //버튼의 연속 클릭 방지~
            _view.SetInteractable(false);

            try
            {
                await _loadScene(GameResultPanelModel.GameSceneName);
            }
            catch (OperationCanceledException)
            {
                _view.SetInteractable(true);
            }
            catch (Exception e)
            {
                Debug.LogException(e);

                _view.SetInteractable(true);
            }
        }

        public void Show()
        {
            _view.Show();
        }

        public void Hide()
        {
            _view.Hide();
        }

        public void Dispose()
        {
            _view.OnResultConfirmed -= OnResultConfirmed;
        }
    }
}

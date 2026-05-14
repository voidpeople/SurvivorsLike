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

        private readonly Func<string, CancellationToken, UniTask> _loadScene;

        private readonly CancellationToken _ct;

        public GameResultPanelPresenter(
            GameResultPanelModel model,
            GameResultPanelView view,
            Func<string, CancellationToken, UniTask> loadScene,
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
            StartGameAsync(_ct).Forget();
        }

        private async UniTask StartGameAsync(CancellationToken ct)
        {
            //버튼의 연속 클릭 방지~
            _view.SetInteractable(false);

            try
            {
                await _loadScene(GameResultPanelModel.GameSceneName, ct);
            }
            catch (OperationCanceledException)
            {
                _view.SetInteractable(true);
            }
        }

        public void Dispose()
        {
            _view.OnResultConfirmed -= OnResultConfirmed;
        }
    }
}

using Cysharp.Threading.Tasks;
using System;
using UnityEngine;


namespace SurvivorsLike
{
    public class GameResultPanelPresenter : IDisposable
    {
        private readonly GameResultPanelModel _model;
        private readonly GameResultPanelView _view;

        private readonly Func<string, UniTask> _loadScene;

        public GameResultPanelPresenter(
            GameResultPanelModel model,
            GameResultPanelView view,
            Func<string, UniTask> loadScene)
        {
            _model = model;
            _view = view;
            _loadScene = loadScene;

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

            await _loadScene(GameResultPanelModel.GameSceneName);
        }

        public void Dispose()
        {
        }
    }
}

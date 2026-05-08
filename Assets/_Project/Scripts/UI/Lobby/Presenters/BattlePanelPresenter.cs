using System;
using Cysharp.Threading.Tasks;

namespace SurvivorsLike
{
    public class BattlePanelPresenter : IDisposable
    {
        private readonly BattlePanelModel        _model;
        private readonly BattlePanelView         _view;
        private readonly Func<string, UniTask>   _loadScene;

        public BattlePanelPresenter(BattlePanelView view, BattlePanelModel model, Func<string, UniTask> loadScene)
        {
            _view      = view;
            _model     = model;
            _loadScene = loadScene;

            _view.OnGameStartClicked += OnGameStartClicked;
        }

        private void OnGameStartClicked()
        {
            if (!_model.CanStart) return;

            StartGameAsync().Forget();
        }

        private async UniTask StartGameAsync()
        {
            _model.SetCanStart(false);
            _view.SetInteractable(false);

            await _loadScene(BattlePanelModel.GameSceneName);
        }

        public void Dispose()
        {
            _view.OnGameStartClicked -= OnGameStartClicked;
        }
    }
}

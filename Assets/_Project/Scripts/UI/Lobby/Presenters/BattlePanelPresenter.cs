using System;
using Cysharp.Threading.Tasks;

namespace SurvivorsLike
{
    public class BattlePanelPresenter : IDisposable
    {
        private readonly BattlePanelModel _model;
        private readonly BattlePanelView  _view;

        public BattlePanelPresenter(BattlePanelView view, BattlePanelModel model)
        {
            _view  = view;
            _model = model;

            _view.OnGameStartClicked += OnGameStartClicked;
        }

        private void OnGameStartClicked()
        {
            if (!_model.CanStart) return;

            StartGameAsync().Forget();
        }

        private async UniTaskVoid StartGameAsync()
        {
            _model.SetCanStart(false);
            _view.SetInteractable(false);

            await GameManager.Instance.LoadScene(BattlePanelModel.GameSceneName);
        }

        public void Dispose()
        {
            _view.OnGameStartClicked -= OnGameStartClicked;
        }
    }
}

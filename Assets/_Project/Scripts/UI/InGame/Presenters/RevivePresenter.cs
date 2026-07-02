using System;


namespace SurvivorsLike
{
    public class RevivePresenter : IDisposable
    {
        private readonly ReviveView _view;
        private readonly Action _onUseEnergy;
        private readonly Action _onWatchAd;
        private readonly Action _onClose;

        public RevivePresenter(
            ReviveView view,
            Action onUseEnergy,
            Action onWatchAd,
            Action onClose)
        {
            _view = view;
            _onUseEnergy = onUseEnergy;
            _onWatchAd = onWatchAd;
            _onClose = onClose;

            _view.Init();
            _view.OnUseEnergyClicked += OnUseEnergyClicked;
            _view.OnWatchAdClicked += OnWatchAdClicked;
            _view.OnCloseClicked += OnCloseClicked;
        }

        public void Show()
        {
            _view.Show();
        }

        public void Hide()
        {
            _view.Hide();
        }

        private void OnUseEnergyClicked()
        {
            Hide();
            _onUseEnergy?.Invoke();
        }

        private void OnWatchAdClicked()
        {
            Hide();
            _onWatchAd?.Invoke();
        }

        private void OnCloseClicked()
        {
            Hide();
            _onClose?.Invoke();
        }

        public void Dispose()
        {
            _view.OnUseEnergyClicked -= OnUseEnergyClicked;
            _view.OnWatchAdClicked -= OnWatchAdClicked;
            _view.OnCloseClicked -= OnCloseClicked;
        }
    }
}

using R3;
using UnityEngine;


namespace SurvivorsLike
{
    public class SkillSelectionPresenter
    {
        private readonly PlayerLevelSystem _levelSystem;
        private readonly SkillSelectionView _view;
        private readonly CompositeDisposable _disposables = new();

        // 가짜 데이터 — 나중에 SkillSelectionSystem으로 교체
        private static readonly SkillOptionData[] _fakeOptions = new[]
        {
            new SkillOptionData(1001, "쿠나이", "", "직선으로 날아가는 쿠나이", false, 1),
            new SkillOptionData(1002, "근접 공격", "", "주변 적을 공격한다", false, 1),
            new SkillOptionData(1001, "쿠나이", "", "직선으로 날아가는 쿠나이", true,  2),
        };

        public SkillSelectionPresenter(PlayerLevelSystem levelSystem, SkillSelectionView view)
        {
            _levelSystem = levelSystem;
            _view = view;

            _levelSystem.OnLevelUp
                .Subscribe(_ => OnLevelUp())
                .AddTo(_disposables);
        }

        private void OnLevelUp()
        {
            InGameStateManager.Instance.EnterLevelingUp();
            _view.Show(_fakeOptions);
            RegisterCardButtons();
        }

        private void OnCardSelected()
        {
            UnregisterCardButtons();
            _view.Hide();
            InGameStateManager.Instance.ExitLevelingUp();
            Debug.Log("=== 스킬 선택됨 (가짜) ===");
        }

        private void RegisterCardButtons()
        {
            foreach (var card in _view.Cards)
                card.Button.onClick.AddListener(OnCardSelected);
        }

        private void UnregisterCardButtons()
        {
            foreach (var card in _view.Cards)
                card.Button.onClick.RemoveListener(OnCardSelected);
        }

        public void Dispose() => _disposables.Dispose();
    }
}

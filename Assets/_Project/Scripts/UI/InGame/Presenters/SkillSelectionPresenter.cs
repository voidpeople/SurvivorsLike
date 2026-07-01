using Cysharp.Threading.Tasks;
using R3;
using System.Threading;


namespace SurvivorsLike
{
    public class SkillSelectionPresenter
    {
        private readonly PlayerLevelSystem _levelSystem;
        private readonly SkillController _skillController;
        private readonly SkillSelectionView _view;
        private readonly CancellationToken _ct;

        private readonly CompositeDisposable _disposables = new();

        public SkillSelectionPresenter(
            PlayerLevelSystem levelSystem,
            SkillController skillController,
            SkillSelectionView view,
            CancellationToken ct)
        {
            _levelSystem = levelSystem;
            _skillController = skillController;
            _view = view;
            _ct = ct;

            _levelSystem.OnLevelUp
                .Subscribe(_ => OnLevelUpAsync().Forget())
                .AddTo(_disposables);
        }

        private async UniTaskVoid OnLevelUpAsync()
        {
            SkillOptionData[] optionDatas = SkillSelectionSystem.GetOptions(
                _skillController.OwnedSkills,
                DataManager.Instance.SkillDataSODic,
                _skillController.IsSlotFull);

            if (optionDatas.Length == 0)
                return;

            InGameStateManager.Instance.EnterLevelingUp();
            await _view.ShowAsync(optionDatas, _ct);
            RegisterCardButtons();
        }

        private void OnCardSelected(SkillOptionData option)
        {
            UnregisterCardButtons();
            _view.Hide();
            InGameStateManager.Instance.ExitLevelingUp();

            if (option.IsUpgrade)
                _skillController.UpgradeSkill(option.SkillId, option.NextLevel);
            else
                _skillController.AddSkill(DataManager.Instance.SkillDataSODic[option.SkillId]);
        }

        private void RegisterCardButtons()
        {
            SkillOptionData[] options = _view.CurrentOptions;
            for (int ii = 0; ii < _view.Cards.Length; ++ii)
            {
                int captured = ii;
                _view.Cards[captured].Button.onClick.AddListener(
                    () => OnCardSelected(options[captured]));
            }
        }

        private void UnregisterCardButtons()
        {
            foreach (var card in _view.Cards)
                card.Button.onClick.RemoveAllListeners();
        }

        public void Dispose() => _disposables.Dispose();
    }
}

using R3;
using System.Linq;
using UnityEngine;


namespace SurvivorsLike
{
    public class SkillSelectionPresenter
    {
        private readonly PlayerLevelSystem _levelSystem;
        private readonly SkillController _skillController;
        private readonly SkillSelectionView _view;

        private readonly CompositeDisposable _disposables = new();

        public SkillSelectionPresenter(
            PlayerLevelSystem levelSystem,
            SkillController skillController,
            SkillSelectionView view)
        {
            _levelSystem = levelSystem;
            _skillController = skillController;
            _view = view;

            _levelSystem.OnLevelUp
                .Subscribe(_ => OnLevelUp())
                .AddTo(_disposables);
        }

        private void OnLevelUp()
        {
            SkillOptionData[] optionDatas = SkillSelectionSystem.GetOptions(
                _skillController.OwnedSkills,
                DataManager.Instance.SkillDataSODic,
                _skillController.IsSlotFull);

            InGameStateManager.Instance.EnterLevelingUp();
            _view.Show(optionDatas);
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
            SkillOptionData[] options = _view.CurrentOptions; // View에서 현재 옵션 보관
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
            {
                card.Button.onClick.RemoveAllListeners();
            }
        }

        public void Dispose() => _disposables.Dispose();
    }
}

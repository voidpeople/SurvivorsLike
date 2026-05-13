using System;
using Cysharp.Threading.Tasks;

namespace SurvivorsLike
{
    public class BattlePanelPresenter : IDisposable
    {
        private readonly BattlePanelModel        _model;
        private readonly BattlePanelView         _view;
        private readonly ChapterSelectPanelPresenter _chapterSelectPanelPresenter;

        private readonly Func<string, UniTask>   _loadScene;

        //씬의 하이라키 상에서는 BattlePanel와 ChapterSelectPanel은 종속 관계가 아니지만
        //개념적으로는 ChapterSelectPanel가 BattlePanel에 종속 되므로
        //BattlePanel의 챕터 버튼이 클릭될 때 ChapterSelectPanelPresenter을 통해 해당 패널의 오브젝트를 활성화 해 줘야 한다.
        public BattlePanelPresenter(
            BattlePanelView view,
            BattlePanelModel model,
            ChapterSelectPanelPresenter chapterSelectPanelPresenter,
            Func<string, UniTask> loadScene)
        {
            _view                        = view;
            _model                       = model;
            _chapterSelectPanelPresenter = chapterSelectPanelPresenter;
            _loadScene                   = loadScene;

            _view.OnOpenChapterSelectPanel += OnOpenChapterSelectPanel;
            _view.OnGameStart += OnGameStartClicked;

            chapterSelectPanelPresenter?.HiewView();
        }

        private void OnOpenChapterSelectPanel()
        {
            _chapterSelectPanelPresenter?.ShowView();
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
            _view.OnGameStart -= OnGameStartClicked;
        }
    }
}

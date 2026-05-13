using System;
using UnityEngine;


namespace SurvivorsLike
{
    //서로 다른 View와 View간에 혹은 Presenter와 Presenter간에 직접 호출하거나 참조하는 건 좋지 않다.
    //이런 경우 MVP클래스들의 부모 클래스를 통해 작업이 이루어 져야 한다.

    public class ChapterSelectPanelPresenter : IDisposable
    {
        private readonly ChapterSelectPanelModel _model;
        private readonly ChapterSelectPanelView  _view;

        private readonly Action<ChapterDataSO>   _onSelectChapter;

        //onSelectChapter은 챕터가 선택되면 GameSessionData에 선택된 챕터의 데이터를 저장하기 위해~
        public ChapterSelectPanelPresenter(
            ChapterSelectPanelModel model,
            ChapterSelectPanelView view,
            Action<ChapterDataSO> onSelectedChapter)
        {
            _view = view;
            _model = model;
            _onSelectChapter = onSelectedChapter;

            _view.OnFinishScrollChapter += OnFinishScrollChapter;
            _view.OnSelectChapter += OnSelectChapter;
            _view.OnExitPanel += OnExitPanel;

            _view.SetupChapterCards(_model.ChapterList);            
        }

        public void ShowView()
        {
            _view.ScrollToChapter(_model.SelectedIndex);
            _view?.Show();
        }

        public void HiewView()
        {
            _view?.Hide();
        }

        //챕터 카드들이 스크롤이 끝나면 멈출 경우 현재 보여지는 카드의 인덱스를 반환
        public void OnFinishScrollChapter(int index)
        {
            ChapterDataSO chapterData = _model.GetChapterData(index);
            if (chapterData != null)
            {
                _view.SetChapterName(chapterData.displayName);
            }
        }

        //선택 버튼을 클릭할 경우 호출되는 함수
        public void OnSelectChapter()
        {
            _model.SetSelectedIndex(_view.GetCurrentChapterIndex());            
            //다른 외부 객체들이 챕터 선택에 대한 통보를 받게 하기 위해~
            _onSelectChapter?.Invoke(_model.SelectedChapter);

            _view?.Hide();
        }

        //나가기 버튼을 클릭할 경우 호출되는 함수
        public void OnExitPanel()
        {
            _view?.Hide();
        }

        public void Dispose()
        {
            _view.OnFinishScrollChapter -= OnFinishScrollChapter;
            _view.OnSelectChapter -= OnSelectChapter;
            _view.OnExitPanel -= OnExitPanel;
        }
    }
}

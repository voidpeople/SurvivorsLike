using System;
using UnityEngine;


namespace SurvivorsLike
{
    public class ChapterSelectPanelPresenter : IDisposable
    {
        private readonly ChapterSelectPanelModel _model;
        private readonly ChapterSelectPanelView  _view;
        private readonly Action<ChapterDataSO>   _onSelectChapter;

        //onSelectChapter은 챕터가 선택되면 GameSessionData에 선택된 챕터의 데이터를 저장하기 위해~
        public ChapterSelectPanelPresenter(
            ChapterSelectPanelModel model,
            ChapterSelectPanelView view,
            Action<ChapterDataSO> onSelectChapter)
        {
            _view = view;
            _model = model;
            _onSelectChapter = onSelectChapter;

            _view.OnFinishScrollChapter += OnFinishScrollChapter;
            _view.OnSelectChapter += OnSelectChapter;
            _view.OnExitPanel += OnExitPanel;

            _view.SetupChapterCards(_model.ChapterList);            
        }

        //챕터 카드들이 스크롤이 끝나면 멈출 경우 현재 보여지는 카드의 인덱스를 반환
        public void OnFinishScrollChapter(int index)
        {            
            _onSelectChapter?.Invoke(_model.SelectedChapter);
        }

        //선택 버튼을 클릭할 경우 호출되는 함수
        public void OnSelectChapter()
        {
        }

        //나가기 버튼을 클릭할 경우 호출되는 함수
        public void OnExitPanel()
        { 
        }

        public void Dispose()
        {
            _view.OnFinishScrollChapter -= OnFinishScrollChapter;
            _view.OnSelectChapter -= OnSelectChapter;
            _view.OnExitPanel -= OnExitPanel;
        }
    }
}

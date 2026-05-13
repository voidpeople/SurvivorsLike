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
            ChapterSelectPanelView view)
        {
            _view = view;
            _model = model;

            _view.OnFinishScrollChapter += OnFinishScrollChapter;
            _view.OnSelectChapter += OnSelectChapter;
            _view.OnExitPanel += OnExitPanel;

            _view.SetupChapterCards(_model.ChapterList);            
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

        ////TODO : 아래 함수는 ChapterSelectPanelPresenter에게 위임 하는게 맞는 것 같다.
        //private void OnSelectChapter(ChapterDataSO chapData)
        //{
        //    //1.챕터 선택 패널을 오픈하는 버튼의 챕터 이미지를 새로 선택한 챕터 이미지로 설정
        //    //_battlePanelView.

        //    //2.GameSessionData 클래스에 챕터 데이터 저장
        //}

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

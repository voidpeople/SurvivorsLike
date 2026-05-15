using System.Collections.Generic;
using UnityEngine;


namespace SurvivorsLike
{
    //MVP 패턴은 책임의 분리이지 구조적 분리가 아니다.

    public class ChapterSelectPanelModel
    {
        public IReadOnlyList<ChapterDataSO> ChapterList { get; }

        //선택 버튼을 클릭하여 선택한 챕터의 인덱스
        public int SelectedIndex { get; private set; }
        public ChapterDataSO SelectedChapter => ChapterList[SelectedIndex];

        public void SetSelectedIndex(int index)
        {
            if (index < 0 || index >= ChapterList.Count)
                return;

            SelectedIndex = index;
        }

        //현재 선택된 챕터 아이디
        private int _selectedChapterId;
        //마지막 클리어 챕터 아이디
        private int _lastClearedChapterId;

        public int SelectedChapterId => _selectedChapterId;
        public int LastClearedChapterId => _lastClearedChapterId;

        public ChapterDataSO GetChapterData(int index)
        {
            if (index < 0 || index >= ChapterList.Count)
                return null ;

            return ChapterList[index];
        }

        public ChapterSelectPanelModel(IReadOnlyList<ChapterDataSO> chapterList, UserData userData)
        {
            ChapterList = chapterList;

            _selectedChapterId    = userData.selectedChapterId;
            _lastClearedChapterId = userData.lastClearedChapterId;
        }
    }
}

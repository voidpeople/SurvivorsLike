using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace SurvivorsLike
{
    //MVP 패턴은 책임의 분리이지 구조적 분리가 아니다.

    public class ChapterSelectPanelModel
    {
        public IReadOnlyList<ChapterDataSO> ChapterDataList { get; }
        private Dictionary<int, ChapterDataSO> _chapterDataDic = new Dictionary<int, ChapterDataSO>();
        private Dictionary<int, int> _chapterCardIndexDic = new Dictionary<int, int>();

        //선택 버튼을 클릭하여 SelectedIndex에 해당 챕터 카드의 인덱스가 저장된다.
        public int SelectedCardIndex { get; private set; }
        public ChapterDataSO SelectedChapterData
        {
            get
            {
                return _chapterDataDic[SelectedCardIndex];
            }
        }

        public void SetSelectedCardIndex(int index)
        {
            if (index < 0 || index >= ChapterDataList.Count)
                return;

            SelectedCardIndex = index;
        }

        //마지막 클리어 챕터 아이디
        private int _lastClearedChapterId;
        public int LastClearedChapterId => _lastClearedChapterId;

        public ChapterDataSO GetChapterData(int index)
        {
            if (index < 0 || index >= ChapterDataList.Count)
                return null ;

            return ChapterDataList[index];
        }

        public ChapterSelectPanelModel(IReadOnlyList<ChapterDataSO> chapterList, UserData userData)
        {
            ChapterDataList = chapterList;
            CreateChapterDics();
            SetSelectedCardIndex(GetChapterCardIndex(userData.selectedChapterId)); 

            _lastClearedChapterId = userData.lastClearedChapterId;
        }

        private void CreateChapterDics()
        {
            for (int ii = 0; ii < ChapterDataList.Count; ii++)
            {
                _chapterDataDic.Add(ii, ChapterDataList[ii]);
            }

            for (int ii = 0; ii < ChapterDataList.Count; ii++)
            {
                _chapterCardIndexDic.Add(ChapterDataList[ii].chapterID, ii);
            }
        }

        private int GetChapterCardIndex(int chapterId)
        {
            int index = -1;
            if(_chapterCardIndexDic.TryGetValue(chapterId, out index) == false)
            {
                Debug.LogError($"존재하지 않는 챕터 아이디 : {chapterId}");
            }

            return index;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;


namespace SurvivorsLike
{
    public class ChapterSelectPanelModel
    {
        public IReadOnlyList<ChapterDataSO> ChapterList { get; }

        //선택 버튼을 클릭하여 선택한 챕터의 인덱스
        public int SelectedIndex { get; private set; }
        public ChapterDataSO SelectedChapter => ChapterList[SelectedIndex];

        public ChapterSelectPanelModel(IReadOnlyList<ChapterDataSO> chapterList)
        {
            ChapterList = chapterList;
        }
    }
}

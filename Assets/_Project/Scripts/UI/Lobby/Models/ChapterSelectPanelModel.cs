using System.Collections.Generic;
using UnityEngine;


namespace SurvivorsLike
{
    public class ChapterSelectPanelModel
    {
        public IReadOnlyList<ChapterDataSO> ChapterList { get; }
        public ChapterSelectPanelModel(IReadOnlyList<ChapterDataSO> chapterList)
        {
            ChapterList = chapterList;
        }
    }
}

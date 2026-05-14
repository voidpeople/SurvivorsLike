using UnityEngine;


namespace SurvivorsLike
{
    public class GameSessionData
    {
        public ChapterDataSO ChapterData { get; private set; }

        public void Init(ChapterDataSO chapterdata)
        {
            ChapterData = chapterdata;
        }

        public void Clear()
        {
            ChapterData = null;
        }
    }
}

using UnityEngine;


namespace SurvivorsLike
{
    public class GameSessionData
    {
        public ChapterDataSO ChapterData { get; private set; }

        public string PlayerModelAddressKey { get; private set; }

        public void Init(ChapterDataSO chapterdata)
        {
            ChapterData = chapterdata;

            PlayerModelAddressKey = "character/player/robotkyle";
        }

        public void Clear()
        {
            ChapterData = null;
        }
    }
}

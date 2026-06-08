using UnityEngine;


namespace SurvivorsLike
{
    public class GameSessionData
    {
        public ChapterDataSO ChapterData { get; private set; }

        public PlayerData PlayerData { get; private set; }

        public void Init(ChapterDataSO chapterdata, PlayerData playerData)
        {
            ChapterData = chapterdata;
            PlayerData = playerData;
        }

        public void Clear()
        {
            ChapterData = null;
            PlayerData = null;
        }
    }
}

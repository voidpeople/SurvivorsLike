using UnityEngine;


namespace SurvivorsLike
{
    public class GameSessionData
    {
        public ChapterDataSO ChapterData { get; private set; }
        public PlayerData PlayerData { get; private set; }
        public PlayerLevelSystem LevelSystem { get; private set; }

        public void Init(ChapterDataSO chapterdata, PlayerData playerData)
        {
            Debug.Assert(chapterdata != null, $"{nameof(GameSessionData)}::Init=> chapterdata is null");
            Debug.Assert(playerData  != null, $"{nameof(GameSessionData)}::Init=> playerData is null");

            ChapterData = chapterdata;
            PlayerData  = playerData;

            LevelSystem = new PlayerLevelSystem();
        }

        public void Clear()
        {
            ChapterData = null;
            PlayerData = null;
            LevelSystem = null;
        }
    }
}

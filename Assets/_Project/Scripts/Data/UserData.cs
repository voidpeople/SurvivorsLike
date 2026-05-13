using System;



namespace SurvivorsLike
{
    [Serializable]
    public class UserData
    {
        public string userId;
        public string nickname;
        public int level;
        public int gold;
        public int gem;
        public int[] clearedChapterIds;
    }
}


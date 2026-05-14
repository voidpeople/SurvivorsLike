using NUnit.Framework;
using System;



namespace SurvivorsLike
{
    [Serializable]
    public class UserData
    {
        public string userID;
        public string nickName;
        public int    level;
        public int    gold;
        public int    gem;
        public int    selectedChapterID;    //현재 선택된 챕터의 아이디
        public int    lastClearedChapterID; //마지막으로 클리어 한 챕터의 아이디
    }
}


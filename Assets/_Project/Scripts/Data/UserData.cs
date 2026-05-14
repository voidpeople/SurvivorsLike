using Firebase.Firestore;
using NUnit.Framework;
using System;



namespace SurvivorsLike
{
    //[FirestoreData]     - "이 클래스를 Firestore와 변환할 수 있다"고 등록
    //[FirestoreProperty] - "이 필드를 Firestore 문서의 필드로 매핑한다"고 지정

    [FirestoreData]
    public class UserData
    {
        [FirestoreProperty] public string userID { get; set; }
        [FirestoreProperty] public string nickName { get; set; }
        [FirestoreProperty] public int level { get; set; }
        [FirestoreProperty] public int gold { get; set; }
        [FirestoreProperty] public int gem { get; set; }
        [FirestoreProperty] public int selectedChapterID { get; set; }    //현재 선택된 챕터의 아이디
        [FirestoreProperty] public int lastClearedChapterID { get; set; } //마지막으로 클리어 한 챕터의 아이디
    }
}


using Firebase.Firestore;


namespace SurvivorsLike
{
    //[FirestoreData]     - "이 클래스를 Firestore와 변환할 수 있다"고 등록
    //[FirestoreProperty] - "이 필드를 Firestore 문서의 필드로 매핑한다"고 지정

    [FirestoreData]
    public class UserData
    {
        [FirestoreProperty("userId")] public string UserId { get; set; }
        [FirestoreProperty("nickName")] public string NickName { get; set; }
        [FirestoreProperty("level")] public int Level { get; set; }
        [FirestoreProperty("gold")] public int Gold { get; set; }
        [FirestoreProperty("gem")] public int Gem { get; set; }
        [FirestoreProperty("selectedChapterId")] public int SelectedChapterId { get; set; }    //현재 선택된 챕터의 아이디
        [FirestoreProperty("lastClearedChapterId")] public int LastClearedChapterId { get; set; } //마지막으로 클리어 한 챕터의 아이디
    }
}


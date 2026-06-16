using UnityEngine;

namespace SurvivorsLike
{
    //스테이지 테이블 정보
    [CreateAssetMenu(fileName = "ChapterDataSO", menuName = "SurvivorsLike/Data/ChapterDataSO")]
    public class ChapterDataSO : ScriptableObject
    {
        [Header("기본 정보")]
        public int Id;
        public string DisplayName;       //"버려진 도시"
        public string DisplaySpriteName; //UI애 출력 될 스프라이트 이름
        public int MapId;                //맵 아이디
        public int WaveId;               //웨이브 데이터 아이디 

        [Header("전투 조건")]
        public int RecommendedCP;        //권장 전투력
        public int EnergyCost = 5;       //에너지 소모
        public float DurationSec = 900f; //15분

        [Header("보상")]
        public int RewardGold;
        public int RewardGem;

        //맵 데이터와 썸 네일 스프라이트는 런타임에 해당 항목들 찾아서 설정됨~
        [Header("맵")]
        public MapDataSO MapData;
        [Header("챕터 스프라이트")]
        public Sprite ThumbnailSprite;
    }
}

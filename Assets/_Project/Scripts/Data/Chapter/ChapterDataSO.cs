using UnityEngine;

namespace SurvivorsLike
{
    //스테이지 테이블 정보
    [CreateAssetMenu(fileName = "ChapterDataSO", menuName = "SurvivorsLike/Data/ChapterDataSO")]
    public class ChapterDataSO : ScriptableObject
    {
        [Header("기본 정보")]
        public int chapterID;
        public string displayName;       // "버려진 도시"
        public string difficulty;        // "초급" / "중급" / "고급"
        public string displaySpriteName; // UI애 출력 될 스프라이트 이름

        [Header("전투 조건")]
        public int recommendedCP;        // 권장 전투력
        public int energyCost = 5;       // 에너지 소모
        public float durationSec = 900f; // 15분

        [Header("보상")]
        public int rewardGold;
        public int rewardGem;

        [Header("에셋 참조 (나중에 연결)")]
        public Sprite thumbnail;
        //public GameObject mapPrefab;

        // WaveDataSO / EnemyDataSO 는 해당 클래스 생성 후 추가 예정    
    }
}

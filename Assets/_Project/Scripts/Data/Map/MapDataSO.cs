using UnityEngine;
using TriInspector;


namespace SurvivorsLike
{

    //맵 데이터
    [CreateAssetMenu(fileName = "MapDataSO", menuName = "SurvivorsLike/Data/MapDataSO")]
    public class MapDataSO : ScriptableObject
    {
        [Title("맵 아이디")]
        public int mapId;

        [Title("그라운드")]
        public string groundMaterialKey;        // Addressables 키. 공유 Ground Mesh의 머티리얼 교체에 사용.
        [Range(20f, 200f)]
        public float playAreaRadius = 60f;      // 플레이어 이동 가능 원형 반경(m). 초과 시 안쪽으로 밀쳐냄.

        [Title("장식물")]
        public string decorationGroupKey;       // Addressables 레이블 키. 챕터 진입 시 그룹 전체 로드, 종료 시 일괄 해제.
        [Range(0, 200)]
        public int decorationCount = 40;        // 맵 전체에 배치할 장식물 총 개수.
        public int decorationSeed = 1;          // 배치 랜덤 시드. 같은 값이면 항상 동일 위치에 배치.

        [Title("조명")]
        public Color ambientLightColor = new Color(0.4f, 0.4f, 0.5f, 1f);  // 환경광 색상. 챕터 분위기의 핵심.
        public Color mainLightColor = Color.white;                           // 디렉셔널 라이트 색상.

        [Title("오디오")]
        public string bgmKey;                   // 배경음 Addressables 키.
    }

}

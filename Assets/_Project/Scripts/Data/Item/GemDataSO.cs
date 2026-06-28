using System;
using System.Collections.Generic;
using System.Linq;
using TriInspector;
using UnityEngine;


namespace SurvivorsLike
{
    public enum GemType
    {
        Green = 0,  // 초록 — 일반 잡몹, XP 낮음
        Blue = 1,   // 파랑 — 중급 적, XP 중간
        Yellow = 2, // 노랑 — 엘리트/보스, XP 높음
        None = 3,
    }

    [Serializable]
    public class GemData
    {
        public GemType GemType;
        public int ExpReward;     //수집 시 지급 경험치
        public string PrefabKey;  //어드레서블 키
        public int PoolInitSize;
        public int PoolMaxSize;
    }

    [CreateAssetMenu(fileName = "GemDataSO", menuName = "SurvivorsLike/Data/GemDataSO")]
    public class GemDataSO : ScriptableObject
    {
        public int ProjectileId;

        //[TableList]은 Tri-Inspector의 명령어
        //역할: List<T> 를 Unity Inspector에서 테이블 형태로 표시
        [TableList]
        public List<GemData> DataList;
    }
}
